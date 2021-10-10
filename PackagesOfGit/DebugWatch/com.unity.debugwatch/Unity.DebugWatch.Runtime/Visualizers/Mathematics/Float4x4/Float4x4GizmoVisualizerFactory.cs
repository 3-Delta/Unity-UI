using System;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.DebugWatch.Visualizers
{

    [UnityEditor.InitializeOnLoad]
    [Serializable]
    public class Float4x4GizmoVisualizerFactory : WorldVisualizerFactory<Float4x4GizmoVisualizerFactory, Float4x4GizmoVisualizerFactory.Float4x4Visualizer, float4x4>
    {
        static Float4x4GizmoVisualizerFactory()
        {
#if !DEBUGWATCH_NODEFAULTREGISTRATION
            WatchTypeRegistry.Instance.RegisterVisualizer(typeof(float4x4), new Visualizers.Float4x4GizmoVisualizerFactory());
#endif
        }
        public override string GetName()
        {
            return "Gizmo";
        }
        public override string GetDescription()
        {
            return "Create a gizmo object that can manipulate the matrix";
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
            ScopedGameObject VisGo = new ScopedGameObject();
            [SerializeField]
            InternalDetails.DebugWatchFloat4x4Gizmo gizmo;
            public Float4x4Visualizer(IWatch w, IAccessor<float4x4> a)
            {
                Watch = w;
                Accessor = a;

                var prefab = Resources.Load("DebugWatchFloat4x4Gizmo") as GameObject;
                VisGo.GameObject = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
                VisGo.GameObject.name = $"Gizmo {w.GetName()} {w.GetContextName()}";
                gizmo = VisGo.GameObject.GetComponent<InternalDetails.DebugWatchFloat4x4Gizmo>();
                gizmo.Accessor = a;
                gizmo.Refresh();

            }
            public override void Dispose()
            {
                VisGo.GameObject = null;
            }

            public override void Update()
            {
                gizmo.Refresh();
            }
        }
    }
}