using UnityEngine;

namespace TraceEventProfiler
{
    /// <summary>
    /// Asynchronous events, unlike other profile blocks are not stack based. You can use them to measure the duration of an ongoing process that could span multiple frames.
    /// </summary>
    public class AsyncEvent
    {
        private int m_EventId;
        private int m_EventInstanceId;
        private int m_ActiveCaptureId;

        /// <param name="asyncEventName">The label that will show up in the profile capture</param>
        public AsyncEvent(string asyncEventName)
        {
            m_EventId = TraceDLLAPI.RegisterAsyncEvent(asyncEventName);
            m_EventInstanceId = TraceDLLAPI.AcquireUniqueAsyncId();
            m_ActiveCaptureId = 0;
        }

        /// <summary>
        /// Signal the beginning of the event
        /// </summary>
        public void Begin()
        {
            if (m_ActiveCaptureId != 0)
                Debug.LogError("AsyncEvent.Begin calls cannot be nested");

            if (TraceProfiler.ActiveCaptureId == 0)
                Debug.LogError("Cannot begin an async event when profiling is not active");

            m_ActiveCaptureId = TraceProfiler.ActiveCaptureId;

            TraceDLLAPI.AddAsyncEvent(true, m_EventId, m_EventInstanceId);
        }

        /// <summary>
        /// Signal the end of the event
        /// </summary>
        public void End()
        {
            if (m_ActiveCaptureId != 0 && m_ActiveCaptureId == TraceProfiler.ActiveCaptureId)
                TraceDLLAPI.AddAsyncEvent(false, m_EventId, m_EventInstanceId);
            m_ActiveCaptureId = 0;
        }
    }
}