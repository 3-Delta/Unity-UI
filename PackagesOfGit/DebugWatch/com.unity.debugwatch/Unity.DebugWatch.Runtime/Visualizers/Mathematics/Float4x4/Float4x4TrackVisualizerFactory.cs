using System;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.DebugWatch.Visualizers
{
    //SceneView.lastActiveSceneView.LookAt(Vector3)
    [Serializable]
    public class Float4x4Visualizer : Visualizer<float4x4>
    {
        [SerializeField]
        public float Scale = 0.2f;
        [SerializeField]
        ScopedGameObject VisGo = new ScopedGameObject();
        [SerializeField]
        InternalDetails.DebugWatchFloat4x4 visualizer;
        public Float4x4Visualizer(IWatch w, IAccessor<float4x4> a)
        {
            Watch = w;
            Accessor = a;
            var prefab = Resources.Load("DebugWatchVisualizer") as GameObject;
            VisGo.GameObject = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            VisGo.GameObject.name = $"Track {w.GetName()} {w.GetContextName()}";
            VisGo.Hide();
            visualizer = VisGo.GameObject.GetComponent<InternalDetails.DebugWatchFloat4x4>();
            visualizer.Accessor = a;
            visualizer.Scale = Scale;
            visualizer.AddCurrentValue();
            visualizer.Refresh();

        }
        public override void Dispose()
        {
            VisGo.GameObject = null;
        }
        public override void Update()
        {
            visualizer.ClearValues();
            visualizer.AddCurrentValue();
            visualizer.Refresh();
        }
    }


    [UnityEditor.InitializeOnLoad]
    [Serializable]
    public class Float4x4TrackVisualizerFactory : WorldVisualizerFactory<Float4x4TrackVisualizerFactory, Float4x4Visualizer, float4x4>
    {
        static Float4x4TrackVisualizerFactory()
        {
#if !DEBUGWATCH_NODEFAULTREGISTRATION
            WatchTypeRegistry.Instance.RegisterVisualizer(typeof(float4x4), new Visualizers.Float4x4TrackVisualizerFactory());
#endif
        }
        public override string GetName()
        {
            return "Track";
        }
        public override string GetDescription()
        {
            return "Render updated value each frame";
        }
        public override bool TryCreate(IWatch w, IAccessor<float4x4> a, out IVisualizer vis)
        {
            vis = new Float4x4Visualizer(w, a);
            return true;
        }
    }
}