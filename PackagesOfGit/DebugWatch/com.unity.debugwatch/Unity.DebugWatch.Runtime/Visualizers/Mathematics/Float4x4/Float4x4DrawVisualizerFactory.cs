using System;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.DebugWatch.Visualizers
{

    internal sealed class StaticHook : ScriptableObject
    {
        static StaticHook _Instance;

        public static StaticHook Instance
        {
            get
            {
                if(_Instance == null)
                {
                    _Instance = CreateInstance<StaticHook>();
                    _Instance.hideFlags = HideFlags.DontSave;
                }
                return _Instance;
            }
        }
        public System.Action OnDispose;
        private void OnDisable()
        {
            OnDispose?.Invoke();
        }
    }

    [Serializable]
    public class ScopedGameObject
    {
        [SerializeField]
        GameObject gameObject;
        public GameObject GameObject
        {
            get
            {
                return gameObject;
            }
            set
            {
                Dispose();
                gameObject = value;
                if (gameObject != null)
                {
                    gameObject.hideFlags = HideFlags.DontSave;
                    StaticHook.Instance.OnDispose += Dispose;
                }
            }
        }

        void Dispose()
        {

            if (gameObject != null)
            {
                UnityEngine.Object.DestroyImmediate(gameObject);
                gameObject = null;
                StaticHook.Instance.OnDispose -= Dispose;
            }
        }

        ~ScopedGameObject()
        {
            Dispose();
        }

        public void Hide()
        {
#if !DEBUGWATCH_SHOWVISUALIZEROBJECTS
            gameObject.hideFlags = gameObject.hideFlags | HideFlags.HideInHierarchy | HideFlags.NotEditable;
#endif
        }
    }

    [UnityEditor.InitializeOnLoad]
    [Serializable]
    public class Float4x4DrawVisualizerFactory : WorldVisualizerFactory<Float4x4DrawVisualizerFactory, Float4x4DrawVisualizerFactory.Float4x4Visualizer, float4x4>
    {
        static Float4x4DrawVisualizerFactory()
        {
#if !DEBUGWATCH_NODEFAULTREGISTRATION
            WatchTypeRegistry.Instance.RegisterVisualizer(typeof(float4x4), new Visualizers.Float4x4DrawVisualizerFactory());
#endif
        }
        public override string GetName()
        {
            return "Draw";
        }
        public override string GetDescription()
        {
            return "Render current value";
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
            //ScopedGameObject VisGo = new ScopedGameObject();
            [SerializeField]
            GameObject VisGoG;
            public Float4x4Visualizer(IWatch w, IAccessor<float4x4> a)
            {
                Watch = w;
                Accessor = a;
                var prefab = Resources.Load("DebugWatchVisualizer") as GameObject;
                VisGoG = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
                VisGoG.name = $"Draw {w.GetName()} {w.GetContextName()}";
                //VisGo.Hide();
                var visualizer = VisGoG.GetComponent<InternalDetails.DebugWatchFloat4x4>();
                visualizer.Accessor = a;
                visualizer.Scale = Scale;
                visualizer.AddCurrentValue();
                visualizer.Refresh();
            }
            public override void Dispose()
            {
                if(VisGoG != null)
                {
                    GameObject.DestroyImmediate(VisGoG);
                }
                //VisGo.GameObject = null;
            }
            public override void Update()
            {
            }
        }
    }
}