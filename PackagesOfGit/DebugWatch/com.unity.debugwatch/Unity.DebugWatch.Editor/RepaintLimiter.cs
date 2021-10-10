using UnityEditor;
using UnityEngine;

namespace Unity.DebugWatch.Editor
{
    internal class RepaintLimiter
    {
        
        private float lastUpdate;
        private int lastFrame;
        public const float defaultPlayingUpdateFrequency = 0.2f;
        public const float defaultUpdateFrequency = 1f;
        private readonly float playingRepaintFrequency;
        private readonly float repaintFrequency;

        public RepaintLimiter(float playingFrequency = defaultPlayingUpdateFrequency, float frequency = defaultUpdateFrequency)
        {
            playingRepaintFrequency = playingFrequency;
            repaintFrequency = frequency;
        }

        public bool IsRepaintTime()
        {
            return EditorApplication.isPlaying && !EditorApplication.isPaused 
                ? Time.unscaledTime > lastUpdate + playingRepaintFrequency
                : Time.frameCount != lastFrame || Time.unscaledTime > lastUpdate + repaintFrequency;
        }

        public void RecordRepaint()
        {
            lastUpdate = Time.unscaledTime;
            lastFrame = Time.frameCount;
        }
    }
}