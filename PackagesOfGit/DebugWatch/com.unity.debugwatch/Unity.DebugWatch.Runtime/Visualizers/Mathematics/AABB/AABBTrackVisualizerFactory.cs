using System;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.DebugWatch.Visualizers
{

    [UnityEditor.InitializeOnLoad]
    [Serializable]
    public class AABBTrackVisualizerFactory : WorldVisualizerFactory<AABBTrackVisualizerFactory, AABBTrackVisualizerFactory.Visualizer, AABB>
    {
        static AABBTrackVisualizerFactory()
        {
#if !DEBUGWATCH_NODEFAULTREGISTRATION
            WatchTypeRegistry.Instance.RegisterVisualizer(typeof(AABB), new Visualizers.AABBTrackVisualizerFactory());
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
        public override bool TryCreate(IWatch w, IAccessor<AABB> a, out IVisualizer vis)
        {
            vis = new Visualizer(w, a);
            return true;
        }
        [Serializable]
        public class Visualizer : Visualizer<AABB>
        {
            [SerializeField]
            public float Scale = 0.2f;
            [SerializeField]
            ScopedGameObject VisGo = new ScopedGameObject();
            [SerializeField]
            InternalDetails.DebugWatchAABB visualizer;
            public Visualizer(IWatch w, IAccessor<AABB> a)
            {
                Watch = w;
                Accessor = a;
                var prefab = Resources.Load("DebugWatchAABB") as GameObject;
                VisGo.GameObject = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
                VisGo.GameObject.name = $"Track {w.GetName()} {w.GetContextName()}";
                VisGo.Hide();
                visualizer = VisGo.GameObject.GetComponent<InternalDetails.DebugWatchAABB>();
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
    }
}