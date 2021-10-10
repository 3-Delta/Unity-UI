using System;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.DebugWatch.Visualizers
{

    [UnityEditor.InitializeOnLoad]
    [Serializable]
    public class Float4x4PlotVisualizerFactory : WorldVisualizerFactory<Float4x4PlotVisualizerFactory, Float4x4PlotVisualizerFactory.Float4x4Visualizer, float4x4>
    {
        static Float4x4PlotVisualizerFactory()
        {
#if !DEBUGWATCH_NODEFAULTREGISTRATION
            WatchTypeRegistry.Instance.RegisterVisualizer(typeof(float4x4), new Visualizers.Float4x4PlotVisualizerFactory());
#endif
        }
        public override string GetName()
        {
            return "Plot";
        }
        public override string GetDescription()
        {
            return "Render values for all frames while this visualizer is active";
        }
        public override bool TryCreate(IWatch w, IAccessor<float4x4> a, out IVisualizer vis)
        {
            vis = new Float4x4Visualizer(w, a);
            return true;
        }
        [Serializable]
        public class Float4x4Visualizer : Visualizer<float4x4>
        {
            [SerializeField]
            public float Scale = 0.2f;
            [SerializeField]
            ScopedGameObject VisGo = new ScopedGameObject();
            [SerializeField]
            InternalDetails.DebugWatchFloat4x4 visualizer;
            int lastFrame = -1;
            public Float4x4Visualizer(IWatch w, IAccessor<float4x4> a)
            {
                Watch = w;
                Accessor = a;
                var prefab = Resources.Load("DebugWatchVisualizer") as GameObject;
                VisGo.GameObject = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
                VisGo.GameObject.name = $"Plot {w.GetName()} {w.GetContextName()}";
                VisGo.Hide();
                visualizer = VisGo.GameObject.GetComponent<InternalDetails.DebugWatchFloat4x4>();
                visualizer.Accessor = a;
                visualizer.Scale = Scale;
                AddCurrentValue();
                visualizer.Refresh();

            }
            public override void Dispose()
            {
                VisGo.GameObject = null;
            }
            void AddCurrentValue()
            {
                lastFrame = Time.frameCount;
                if (Accessor.TryGet(out var v))
                {
                    visualizer.Values.Add(v);
                }
            }
            public override void Update()
            {
                if (lastFrame != Time.frameCount)
                {
                    AddCurrentValue();
                }
                visualizer.Refresh();
            }
        }
    }
}