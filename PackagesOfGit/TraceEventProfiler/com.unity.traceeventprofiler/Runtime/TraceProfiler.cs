using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace TraceEventProfiler
{
    /// <summary>
    /// Static class implementing API for starting and stopping a profile capture.
    /// </summary>
    public static class TraceProfiler
    {
        static int m_ActiveCaptureId = 0;

        static internal int ActiveCaptureId { get { return m_ActiveCaptureId; } }

        private static void LogLastError(string functionName)
        {
            Debug.LogErrorFormat("{0}: {1}", functionName, GetLastError());
        }

        /// <summary>
        /// Begin a capturing profile data.
        /// </summary>
        /// <remarks>
        /// Captured profile data will be save to the file provided. The capture will end when you call EndCapture or when the maximum allowed memory is exceeded.
        /// </remarks>
        /// <param name="filename">Output file path. If the file path is not rooted, the path will be prepended by the player's persistent data path.</param>
        /// <param name="maxMemoryMB">Maximum amount of scratch memory that the profiler is allowed to use.</param>
        public static void BeginCapture(string filename, int maxMemoryMB = 500)
        {
            if (!IsPlatformSupported(Application.platform))
            {
                Debug.LogErrorFormat("The TraceEventProfiler does not support the current platform: {0}", Application.platform);
                return;
            }

            if (!Path.IsPathRooted(filename))
                filename = Path.Combine(Application.persistentDataPath, filename);

            Debug.Log("Writing Capture to " + filename);

            m_ActiveCaptureId = TraceDLLAPI.BeginCapture(filename, maxMemoryMB);
            if (m_ActiveCaptureId == 0)
                LogLastError("TraceProfiler::BeginCapture");
        }

        /// <summary>
        /// Ends the profile capture
        /// </summary>
        public static void EndCapture()
        {
            if (TraceDLLAPI.EndCapture() == 0)
                LogLastError("TraceProfiler::EndCapture");

            Debug.Log("Capture Ended");
        }

        /// <summary>
        /// Checks if a specified platform is supported
        /// </summary>
        /// <param name="platform">Platform to check</param>
        /// <returns>Returns true if the specified platform is support, otherwise false.</returns>
        public static bool IsPlatformSupported(RuntimePlatform platform)
        {
            switch(platform)
            {
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.Android:
                    return true;
            }
            return false;
        }

        private static string GetLastError()
        {
            byte[] errorData = new byte[4096];
            TraceDLLAPI.GetLastProfilerError(errorData, errorData.Length);
            return System.Text.Encoding.ASCII.GetString(errorData);
        }
    }
}
