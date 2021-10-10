using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.IO;
using TraceEventProfiler;
using System;
using System.Collections.Generic;
using System.Linq;

public class PlaymodeTests {

    [Serializable]
    class ThreadArg
    {
        public string name;
    }

    [Serializable]
    class ProfileEvent
    {
        public ThreadArg args;
        public string cat;
        public int id;
        public string name;
        public string ph;
        public int pid;
        public int tid;
        public int ts;
    }

    class EventParser
    {

        [Serializable]
        class Wrapper
        {
            public List<ProfileEvent> array;
        }

        public EventParser(string filename)
        {
            string fileText = File.ReadAllText(filename);
            string jsonText = "{ \"array\": " + fileText + "}";
            Wrapper w = JsonUtility.FromJson<Wrapper>(jsonText);
            Events = w.array;
        }

        public List<ProfileEvent> FindWithName(string name)
        {
            return new List<ProfileEvent>(Events.Where(i => i.name == name));
        }

        List<ProfileEvent> Events;
        
    }

    [UnityTest]
    public IEnumerator ThreadNameEvents_PresentInTheCapture()
    {
        var filename = Path.Combine(Application.persistentDataPath, "testfile.json");
        TraceProfiler.BeginCapture(filename);
        yield return new WaitForSeconds(0.1f);
        TraceProfiler.EndCapture();
        
        var parser = new EventParser(filename);
        var threadEvents = parser.FindWithName("thread_name");

        var mainThreadDetected = false;
        foreach (var e in threadEvents)
        {
            Assert.IsNull(e.cat);
            Assert.AreEqual("thread_name", e.name);
            Assert.AreEqual("M", e.ph);
            Assert.IsNotNull(e.args);
            Assert.IsNotNull(e.args.name);
            if (e.args.name.Contains("Main Thread"))
                mainThreadDetected = true;
        }
        Assert.IsTrue(mainThreadDetected);

        File.Delete(filename);
    }

    [UnityTest]
    public IEnumerator Profiler_AsyncEventsCapturePerformance()
    {
        string testFileName = "testfile.json";
        TraceProfiler.BeginCapture(testFileName);
        var e = new TraceEventProfiler.AsyncEvent("MyAsyncEvent");
        e.Begin();
        yield return new WaitForSeconds(0.1f);
        e.End();
        TraceProfiler.EndCapture();

        string filename = Path.Combine(Application.persistentDataPath, testFileName);
        EventParser parser = new EventParser(filename);
        List<ProfileEvent> asyncEvents = parser.FindWithName("MyAsyncEvent");

        // TODO: Add more tests, but for now make sure that async events work
        Assert.AreEqual("b", asyncEvents[0].ph);
        Assert.AreEqual("e", asyncEvents[1].ph);

        string filePath = Path.Combine(Application.persistentDataPath, testFileName);
        File.Delete(filePath);
    }

    [UnityTest]
    public IEnumerator Profiler_OutputsPerformanceDataFile()
    {
        string testFileName = "testfile.json";
        TraceProfiler.BeginCapture(testFileName);
        yield return new WaitForSeconds(0.1f);
        TraceProfiler.EndCapture();

        string filePath = Path.Combine(Application.persistentDataPath, testFileName);
        Assert.IsTrue(File.Exists(filePath));
        File.Delete(filePath);
    }
}
