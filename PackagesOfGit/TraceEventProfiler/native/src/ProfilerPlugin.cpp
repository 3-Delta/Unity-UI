#include "Unity/IUnityInterface.h"
#include "Unity/IUnityProfilerCallbacks.h"

#include <cstring>
#include <string>
#include <chrono>
#include <atomic>
#include <thread>
#include <iostream>
#include <fstream>
#include <map>
#include <algorithm>
#include <vector>
#include <sstream>
#include <mutex>

#ifdef _MSC_VER
#include "windows.h"
#endif

/*

This plugin hooks into the native Unity profile to log events and export them into the Chrome Trace Format.

Each thread owns a block of kEventsPerBlock events. Upon completing the block, ownership is passed to the main thread
EndCapture() cannot safely deallocate memory from threads without using heavier weight atomics, so memory that hasn't passed ownership to
the main thread is not deallocated until the next capture begins.

Partially completed EventNodeBlock's can be examined in the EndCapture event, but because a relaxed atomic operation is used when
adding events to each block, the memory may be invalid. Using proper memory fencing (memory release atomic) would be too slow. Two things are 
done to work around this: 1) The EndCapture thread will sleep briefly after signaling the capture is done. This will 
hopefully result in memory used on threads being flushed to memory. 2) A magic value is written into each
event that is used to validate each node when read from the main thread.

*/

static const int kEventsPerBlock = 4 * 1024;
static const uint16_t kMemValidationMagicData = 0xABCD;
static const int kFrameEventAsyncId = 1;

enum EventType
{
    // Unity built-in event types
    EventType_Begin = kUnityProfilerMarkerEventTypeBegin,
    EventType_End = kUnityProfilerMarkerEventTypeEnd,
    EventType_Single = kUnityProfilerMarkerEventTypeSingle,
	
    EventType_FlowBegin,
    EventType_FlowNext,

    // Events types that are unknown to the Unity profiler
    EventType_AsyncBegin,
	EventType_AsyncEnd,

    // Thread name event
    EventType_ThreadName,

    EventType_Count
};
static const char* const kEventTypePhaseNames[] =
{
    "\"B\"",
    "\"E\"",
    "\"i\"",
    "\"s\"",
    "\"f\"",
    "\"b\"",
    "\"e\"",
    "\"M\""
};
static_assert(sizeof(kEventTypePhaseNames)/sizeof(kEventTypePhaseNames[0]) == EventType_Count, "kEventTypePhaseNames must match EventType enum");

struct FlowEventData
{
    uint32_t flowId;
};

struct AsyncEventData
{
	uint32_t asyncEventNameId;
	uint32_t asyncEventInstanceId;
};

struct ThreadNameData
{
    uint64_t threadId;
    uint32_t threadNameId;
};

struct EventNode
{
	std::chrono::time_point<std::chrono::steady_clock> time;
	union
	{
        // EventType_Flow*
        FlowEventData flowData;
        // For EventType_Async*
        AsyncEventData asyncData;
        // Thread name information
        ThreadNameData threadNameData;
        // For Unity profiler events
        const UnityProfilerMarkerDesc *markerDesc; 
	};
    
    // Either EventType or kUnityProfilerMarkerEventType*
	uint16_t type; 

    // Main thread can check this value to validate the node. Proper memory fencing cannot
    // be used because of the overhead of atomics
	uint16_t memValidationData;
};

struct ThreadLocalState;

struct EventNodeBlock
{
	EventNodeBlock() : pos(0), next(NULL), isFinalized(false)	{ }

	EventNode data[kEventsPerBlock];
	std::atomic<int> pos;
	std::atomic<bool> isFinalized;
	ThreadLocalState *threadState;
	int captureId;

	EventNodeBlock *next;
};

static std::atomic<int> gThreadCount;
struct ThreadLocalState
{
	ThreadLocalState() :
        threadId(0),
        curBlock(nullptr),
		stateInitilaized(false)
    {}

	bool stateInitilaized;
	EventNodeBlock *curBlock;
    uint64_t threadId;
	std::string lastError;
};

struct CaptureState
{
	CaptureState() : 
		activeCaptureId(0), 
		isCaptureActive(false), 
		nextAsyncCaptureId(kFrameEventAsyncId + 1), 
		frameAsyncEventId(-1),
		pendingDeallocList(NULL) 
	{ }

	std::string filename;
	uint64_t maxMemoryUsageBytes;
	std::atomic<unsigned int> curMemUsageBytes;
	int activeCaptureId;
	std::atomic<int> nextAsyncCaptureId;
	int frameAsyncEventId;
	std::atomic<bool> isCaptureActive;
	std::chrono::time_point<std::chrono::steady_clock> captureStartTime;
	std::vector<std::string> categories;
	bool inFrame;

	// User can create async events from any thread so use a mutex to protect. Will usually only be done from the main thread
	std::mutex nameListMutex;
	std::vector<std::string> nameList;

	// Blocks are immediately placed in this list after allocation on a thread
	std::atomic<EventNodeBlock *> activeBlockList;

	// The main thread knows about all the block allocations but cannot free them until the thread
	// flags them as finalized. Until that happens, the blocks are stored in this list
	EventNodeBlock* pendingDeallocList;
};

CaptureState gCaptureState;
thread_local ThreadLocalState gThreadState;
static IUnityProfilerCallbacks* s_UnityProfilerCallbacks = nullptr;
static IUnityProfilerCallbacksV2* s_UnityProfilerCallbacksV2 = nullptr;

inline void InitializeThreadLocal()
{
	if (!gThreadState.stateInitilaized)
    {
		
#ifdef _MSC_VER
		gThreadState.threadId = GetCurrentThreadId();
#else
		std::thread::id currentThreadId = std::this_thread::get_id();
		gThreadState.threadId = *((size_t*)(&currentThreadId));
#endif
        std::atomic_fetch_add_explicit(&gThreadCount, 1, std::memory_order_relaxed);
		gThreadState.stateInitilaized = true;
    }
}

static bool AddEventNodeToBlock(EventNodeBlock &block, EventNode &e)
{
	int pos = block.pos.load(std::memory_order_relaxed);
	if (pos < kEventsPerBlock)
	{
		e.memValidationData = kMemValidationMagicData;
		block.data[pos] = e;
		block.pos.store(block.pos + 1, std::memory_order_relaxed);
		return true;
	}
	return false;
}

static bool AllocateEventBlock(CaptureState &captureState, ThreadLocalState &threadState, int captureId)
{
	// Finalize the previous block. Tells main thread we won't write to the block anymore
	if (threadState.curBlock != NULL)
		threadState.curBlock->isFinalized.store(true);

	if (captureState.curMemUsageBytes.load(std::memory_order_relaxed) + sizeof(EventNodeBlock) < captureState.maxMemoryUsageBytes)
	{
		captureState.curMemUsageBytes.fetch_add(sizeof(EventNodeBlock), std::memory_order_relaxed);
		EventNodeBlock *block = new EventNodeBlock();
		block->threadState = &threadState;
		block->captureId = captureId;
		threadState.curBlock = block;

		// Insert it into the list in the capture state
		block->next = captureState.activeBlockList.load(std::memory_order_relaxed);
		while (!captureState.activeBlockList.compare_exchange_weak(block->next, block, std::memory_order_release, std::memory_order_relaxed));
	}
	else
	{
		threadState.curBlock = NULL;
	}
	return threadState.curBlock != NULL;
}

static void AddEventNode(EventNode &eNode, ThreadLocalState &tState, int captureId)
{
	// The thread has an active block with the same capture ID and there was room for this node... done
	if (tState.curBlock != NULL && tState.curBlock->captureId == captureId && AddEventNodeToBlock(*tState.curBlock, eNode))
		return;
	
	// Alloc new thread local block
	if (AllocateEventBlock(gCaptureState, tState, captureId))
		AddEventNodeToBlock(*tState.curBlock, eNode);
}

uint32_t RegisterStringInternal(const std::string& s)
{
	std::lock_guard<std::mutex>(gCaptureState.nameListMutex);

    uint32_t id = (uint32_t)gCaptureState.nameList.size();
	if (id == 0)
		gCaptureState.nameList.reserve(128);

	gCaptureState.nameList.push_back(s);
	return id;
}

void AddAsyncEventInternal(bool isBeginEvent, int asyncEventNameId, int asyncEventInstanceId)
{
	EventNode node;
	node.time = std::chrono::steady_clock::now();
	node.type = isBeginEvent ? EventType_AsyncBegin : EventType_AsyncEnd;
	node.asyncData.asyncEventNameId = asyncEventNameId;
	node.asyncData.asyncEventInstanceId = asyncEventInstanceId;
	AddEventNode(node, gThreadState, gCaptureState.activeCaptureId);
}

static void UNITY_INTERFACE_API EventCallback(const UnityProfilerMarkerDesc* eventDesc, UnityProfilerMarkerEventType eventType, unsigned short eventDataCount, const UnityProfilerMarkerData* eventData, void* userData)
{
    if (gCaptureState.isCaptureActive.load(std::memory_order_relaxed))
    {
        InitializeThreadLocal();
        EventNode node;
        node.time = std::chrono::steady_clock::now();
        node.markerDesc = eventDesc;
        node.type = eventType;
        AddEventNode(node, gThreadState, (int)(intptr_t)userData);
    }
}

static void UNITY_INTERFACE_API FlowEventCallback(UnityProfilerFlowEventType flowEventType, uint32_t flowId, void* userData)
{
    if (gCaptureState.isCaptureActive.load(std::memory_order_relaxed))
    {
        InitializeThreadLocal();
        EventNode node;
        node.time = std::chrono::steady_clock::now();
        node.flowData.flowId = flowId;
        node.type = flowEventType == kUnityProfilerFlowEventTypeBegin ? EventType_FlowBegin : EventType_FlowNext;
        AddEventNode(node, gThreadState, (int)(intptr_t)userData);
    }
}

static void UNITY_INTERFACE_API CreateMarkerCallback(const UnityProfilerMarkerDesc* eventDesc, void* userData)
{
	if (gCaptureState.isCaptureActive.load(std::memory_order_relaxed))
		s_UnityProfilerCallbacks->RegisterMarkerEventCallback(eventDesc, EventCallback, userData);
}

static void UNITY_INTERFACE_API FrameCallback(void* userData)
{
	if (gCaptureState.isCaptureActive)
	{
		InitializeThreadLocal();
		if (gCaptureState.frameAsyncEventId == -1)
			gCaptureState.frameAsyncEventId = RegisterStringInternal("Frame");
		
		if (gCaptureState.inFrame)
			AddAsyncEventInternal(false, gCaptureState.frameAsyncEventId, kFrameEventAsyncId);
		
		AddAsyncEventInternal(true, gCaptureState.frameAsyncEventId, kFrameEventAsyncId);
		
		gCaptureState.inFrame = true;
	}
}

static void UNITY_INTERFACE_API CreateCategoryCallback(const UnityProfilerCategoryDesc* categoryDesc, void* userData)
{
	if (gCaptureState.categories.size() == 0)
		gCaptureState.categories.reserve(128);
	gCaptureState.categories.emplace_back(categoryDesc->name);
}

static void UNITY_INTERFACE_API CreateThreadCallback(const UnityProfilerThreadDesc* threadDesc, void* userData)
{
    if (!gCaptureState.isCaptureActive.load(std::memory_order_relaxed))
        return;

    InitializeThreadLocal();

    std::string threadName = threadDesc->groupName;
    threadName += '.';
    threadName += threadDesc->name;

    EventNode node;
    node.time = std::chrono::steady_clock::now();
    node.threadNameData.threadId = threadDesc->threadId;
    node.threadNameData.threadNameId = RegisterStringInternal(threadName);
    node.type = EventType_ThreadName;
    AddEventNode(node, gThreadState, (int)(intptr_t)userData);
}

static EventNodeBlock *ReverseListOrder(EventNodeBlock *list)
{
	EventNodeBlock *prev = NULL, *next = NULL;
	EventNodeBlock *cur = list;
	while (cur != NULL)
	{
		next = cur->next;
		cur->next = prev;
		prev = cur;
		cur = next;
	}
	return prev;
}

// Deletes all the blocks that are finalized by the thread. That means the owning thread will
// not try to write to it anymore. Return a new list of the remaining blocks
static EventNodeBlock *DeallocFinalized(EventNodeBlock *list)
{
	EventNodeBlock *ret = NULL;
	EventNodeBlock *iter = list;
	while (iter != NULL) {
		
		EventNodeBlock *nextIter = iter->next;

		if (iter->isFinalized)
			delete iter;
		else
		{
			iter->next = ret;
			ret = iter;
		}
		iter = nextIter;
	}
	return ret;
}

static EventNodeBlock *CombineLists(EventNodeBlock *l1, EventNodeBlock *l2)
{
	EventNodeBlock *next = l1;
	while (next != NULL)
	{
		EventNodeBlock *cur = next;
		next = cur->next;
		cur->next = l2;
		l2 = cur;
	}
	return l2;
}

static std::string ToString(long value)
{
	std::ostringstream ss;
	ss << value;
	return ss.str();
}

static std::string UInt64ToString(uint64_t value)
{
	std::ostringstream ss;
	ss << value;
	return ss.str();
}

static bool WriteTraceFile(std::string &filename, EventNodeBlock *list, int captureId)
{
	std::ofstream file;
	file.open(filename.c_str());

	if (!file.is_open())
	{
		// TODO: make sure memory gets freed still
		gThreadState.lastError = "Failed to open file '" + filename + "' for writing";
		return false;
	}

	file << "[" << std::endl;

	std::map<std::string, std::string> eventValues;
    std::string eventName;
    std::string argStr;
	bool first = true;
	
	for(EventNodeBlock *block = list; block != NULL; block = block->next)
	{
		// This block is from a previous capture. ignore it
		if (block->captureId != captureId)
			continue;


		int eventCount = block->pos;
		for (int i = 0; i < eventCount; i++)
		{
			EventNode &node = block->data[i];

			// If this doesn't match, the memory written by the thread hasn't been flushed yet.
			// This should be very rare race condition, but handling this case anyway.. just skip the event
			if (node.memValidationData != kMemValidationMagicData)
				continue;

			eventValues.clear();

            eventValues["ph"] = kEventTypePhaseNames[node.type];
            long usTime = (long)std::chrono::duration_cast<std::chrono::microseconds>(node.time - gCaptureState.captureStartTime).count();
            eventValues["ts"] = ToString(usTime);
            eventValues["tid"] = UInt64ToString(block->threadState->threadId);
            eventValues["pid"] = "1";

            switch (node.type)
            {
            case EventType_Begin:
            case EventType_End:
            case EventType_Single:
            {
                eventName = node.markerDesc->name != NULL ? std::string(node.markerDesc->name) : "Unknown Event Name";
                eventValues["cat"] = std::string("\"") + gCaptureState.categories[node.markerDesc->categoryId] + "\"";
            } break;

            case EventType_FlowBegin:
            case EventType_FlowNext:
            {
                eventName = "flow";
                eventValues["id"] = ToString(node.flowData.flowId);
                eventValues["cat"] = "\"flowevent\"";
            } break;

            case EventType_AsyncBegin:
            case EventType_AsyncEnd:
            {
                eventValues["id"] = ToString(node.asyncData.asyncEventInstanceId);
                eventValues["cat"] = "\"asyncevent\"";

                std::lock_guard<std::mutex>(gCaptureState.nameListMutex);
                eventName = gCaptureState.nameList[node.asyncData.asyncEventNameId];
            } break;

            case EventType_ThreadName:
            {
                eventName = "thread_name";

                argStr = "{\"name\":\"";
                {
                    std::lock_guard<std::mutex>(gCaptureState.nameListMutex);
                    argStr += gCaptureState.nameList[node.threadNameData.threadNameId];
                }
                argStr += "\"}";
                eventValues["args"] = argStr;
                eventValues["tid"] = UInt64ToString(node.threadNameData.threadId);
            } break;
            }

            if (!eventName.empty())
            {
                eventName.erase(std::remove(eventName.begin(), eventName.end(), '\n'), eventName.end());
                eventValues["name"] = "\"" + eventName + "\"";
            }

			if (first)
				first = false;
			else
				file << ",";

			file << "{";
			for (auto iter = eventValues.begin(); iter != eventValues.end(); iter++)
			{
				if (iter != eventValues.begin())
					file << ", ";
				file << "\"" << iter->first << "\": " << iter->second;
			}
			file << "}" << std::endl;
		}
	}

	file << "]" << std::endl;
	file.close();

	return true;
}

extern "C" int UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API BeginCapture(const char *filename, int maxMemUsageMB)
{
	if (gCaptureState.isCaptureActive)
	{
		gThreadState.lastError = "BeginCapture called but a capture is already active";
		return 0;
	}

	gCaptureState.maxMemoryUsageBytes = maxMemUsageMB * 1024 * 1024; // MB to B
	gCaptureState.captureStartTime = std::chrono::steady_clock::now();
	gCaptureState.filename = filename;
	gCaptureState.inFrame = false;
	gCaptureState.activeCaptureId++;
	gCaptureState.isCaptureActive.store(true);
	s_UnityProfilerCallbacks->RegisterCreateCategoryCallback(&CreateCategoryCallback, NULL);
	s_UnityProfilerCallbacks->RegisterCreateMarkerCallback(&CreateMarkerCallback, (void*)(intptr_t)gCaptureState.activeCaptureId);
    s_UnityProfilerCallbacks->RegisterFrameCallback(&FrameCallback, (void*)(intptr_t)gCaptureState.activeCaptureId);
    s_UnityProfilerCallbacks->RegisterCreateThreadCallback(&CreateThreadCallback, (void*)(intptr_t)gCaptureState.activeCaptureId);
    if (s_UnityProfilerCallbacksV2 != NULL)
        s_UnityProfilerCallbacksV2->RegisterFlowEventCallback(&FlowEventCallback, (void*)(intptr_t)gCaptureState.activeCaptureId);

	return gCaptureState.activeCaptureId;
}

extern "C" int UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API EndCapture()
{
	if (!gCaptureState.isCaptureActive)
	{
		gThreadState.lastError = "EndCapture called but a capture is not active";
		return 0;
	}

    // signal to threads to stop capturing events
	gCaptureState.isCaptureActive.store(false);

    // Give time for threads to complete active events and also time for thread written memory to be flushed so we can read it
    // See comments at the top of this file for more details
	std::this_thread::sleep_for(std::chrono::milliseconds(1000));

	s_UnityProfilerCallbacks->UnregisterCreateMarkerCallback(&CreateMarkerCallback, (void*)(intptr_t)gCaptureState.activeCaptureId);
    s_UnityProfilerCallbacks->UnregisterMarkerEventCallback(NULL, &EventCallback, (void*)(intptr_t)gCaptureState.activeCaptureId);
	s_UnityProfilerCallbacks->UnregisterFrameCallback(&FrameCallback, (void*)(intptr_t)gCaptureState.activeCaptureId);
    s_UnityProfilerCallbacks->UnregisterCreateCategoryCallback(&CreateCategoryCallback, NULL);
    s_UnityProfilerCallbacks->UnregisterCreateThreadCallback(&CreateThreadCallback, (void*)(intptr_t)gCaptureState.activeCaptureId);
    if (s_UnityProfilerCallbacksV2 != NULL)
        s_UnityProfilerCallbacksV2->UnregisterFlowEventCallback(&FlowEventCallback, (void*)(intptr_t)gCaptureState.activeCaptureId);

	// take the block list
	EventNodeBlock *localList = gCaptureState.activeBlockList.load(std::memory_order_relaxed);
	while (!gCaptureState.activeBlockList.compare_exchange_weak(localList, NULL, std::memory_order_acquire));

	// reverse the order of the block list so we pull them out in order the were submitted
	localList = ReverseListOrder(localList);

	bool result = WriteTraceFile(gCaptureState.filename, localList, gCaptureState.activeCaptureId);

	// add the blocks we just processed to the deallocate list
	gCaptureState.pendingDeallocList = CombineLists(localList, gCaptureState.pendingDeallocList);

	// deallocated all blocks that have been released by their originating thread
	gCaptureState.pendingDeallocList = DeallocFinalized(gCaptureState.pendingDeallocList);

	gCaptureState.categories.clear();

	return result ? 1 : 0;
}

extern "C" void	UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginLoad(IUnityInterfaces* unityInterfaces)
{
	s_UnityProfilerCallbacks = unityInterfaces->Get<IUnityProfilerCallbacks>();
    s_UnityProfilerCallbacksV2 = unityInterfaces->Get<IUnityProfilerCallbacksV2>();
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginUnload()
{
    if (gCaptureState.isCaptureActive)
	    EndCapture();
}

extern "C" int UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API RegisterAsyncEvent(const char *eventName)
{
	return RegisterStringInternal(eventName);
}

extern "C" int UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API AcquireUniqueAsyncId()
{
	return gCaptureState.nextAsyncCaptureId.fetch_add(1, std::memory_order_relaxed);
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API AddAsyncEvent(bool isBeginEvent, int asyncEventId, int asyncEventInstanceId)
{
	if (gCaptureState.isCaptureActive)
	{
		InitializeThreadLocal();
		AddAsyncEventInternal(isBeginEvent, asyncEventId, asyncEventInstanceId);
	}
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API GetLastProfilerError(char *buf, int bufSize)
{
#ifdef _MSC_VER
	strncpy_s(buf, bufSize, gThreadState.lastError.c_str(), gThreadState.lastError.size());
#else
    strncpy(buf, gThreadState.lastError.c_str(), bufSize);
#endif
}
