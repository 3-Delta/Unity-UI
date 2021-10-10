using System;
using System.Collections.Generic;
using System.Text;
using Unity.Entities;
using Unity.Mathematics;


using UnityEngine;
using Unity.Properties;
using Unity.Assertions;
using UnityEngine.UIElements;
using System.ComponentModel;
//using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

namespace Unity.DebugWatch
{

    //internal sealed class EditorScopeHook : ScriptableObject
    //{
    //    static EditorScopeHook _Instance;

    //    public static EditorScopeHook Instance
    //    {
    //        get
    //        {
    //            if (_Instance == null)
    //            {
    //                do
    //                {
    //                    _Instance = ScriptableObject.FindObjectOfType<EditorScopeHook>();
    //                    if (_Instance != null)
    //                    {
    //                        if (_Instance.WatchRegistry == null)
    //                        {
    //                            Debug.LogWarning("EditorScopeHook was found but WatchRegistry is null");
    //                            DestroyImmediate(_Instance);
    //                            _Instance = null;
    //                        }
    //                        else
    //                        {
    //                            Debug.LogWarning("EditorScopeHook was found with valid WatchRegistry ");
    //                            break;
    //                        }
    //                    }
    //                    else
    //                    {
    //                        Debug.LogWarning("EditorScopeHook was not found ");
    //                        break;
    //                    }

    //                } while (true);
    //            }

    //            if (_Instance == null)
    //            {
    //                _Instance = CreateInstance<EditorScopeHook>();
    //                _Instance.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontUnloadUnusedAsset;
    //                _Instance.WatchRegistry = new WatchRegistry(WatchTypeRegistry.Instance);
    //            }
    //            return _Instance;
    //        }
    //    }
    //    [SerializeField]
    //    public WatchRegistry WatchRegistry;

    //    public System.Action OnDispose;
    //    private void OnDisable()
    //    {
    //        OnDispose?.Invoke();
    //    }
    //}
    


    //public interface IWatchCollectionContext
    //{
    //    int GetCount();
    //    ContextMemberInfo GetInfoAt(int index);
    //}
    namespace WatchContext
    {



        //public class 
        [Serializable]
        public class ContextPathWatch<TValue> : IWatch<TValue>
        {

            [SerializeField]
            public IWatchContext Context;

            [SerializeField]
            public string Path;

            public ContextPathWatch(IWatchContext context, string path)
            {
                Context = context;
                Path = path;
            }
            public virtual string GetContextName()
            {
                return Context.Scope("");
            }
            public virtual string GetName()
            {
                return Path.ToString();
            }
            public Type GetValueType()
            {
                return typeof(TValue);
            }
            bool TryCreateWatch(out IWatch watch)
            {
                var pathRange = Path.Range();
                if (WatchContext.TryParseDeepest(Context, Path, ref pathRange, out var lastCtx))
                {
                    return lastCtx.TryCreateWatch(out watch);
                }
                watch = default;
                return false;
                //return WatchContext.TryCreateWatch(Context, Path, Path.Range(), out watch);
            }
            public bool TryGet(out TValue value)
            {
                if(TryCreateWatch(out var watch))
                {
                    if (watch is IWatch<TValue> w)
                    {
                        return w.TryGet(out value);
                    }
                }
                value = default;
                return false;
            }
            public bool TrySet(TValue value)
            {
                if (TryCreateWatch(out var watch))
                {
                    if (watch is IWatch<TValue> w)
                    {
                        return w.TrySet(value);
                    }
                }
                return false;
            }
        }

        //public class 
        [Serializable]
        public class PropertyPathWatch<TContainer, TValue> : IWatch<TValue>
            where TContainer : class
        {
            [SerializeField]
            public IWatchContext Context;
            [SerializeField]
            public TContainer Container;

            [SerializeField]
            public PropertyPath PropPath;

            public PropertyPathWatch(IWatchContext context, ref TContainer container, PropertyPath path)
            {
                Context = context;
                PropPath = path;
                Container = container;
            }
            public virtual string GetContextName()
            {
                return Context.Scope("");
            }
            public virtual string GetName()
            {
                return PropPath.ToString();
            }
            public Type GetValueType()
            {
                return typeof(TValue);
            }
            public bool TryGet(out TValue value)
            {
                return PropertyContainer.TryGetValueAtPath(ref Container, PropPath, out value);

            }
            public bool TrySet(TValue value)
            {
                return PropertyContainer.TrySetValueAtPath(ref Container, PropPath, value);
            }
        }















    }
    [Serializable]
    public class WatchTypeRegistry
    {
        [Serializable]
        public class TypeInfo
        {
            public SortedDictionary<string, IVisualizerFactory> Visualizers = new SortedDictionary<string, IVisualizerFactory>();
            public SortedDictionary<string, IAccessorFactory> Accessors = new SortedDictionary<string, IAccessorFactory>();
            public IAccessorFactory StringAccessorFactory;
        }
        [SerializeField]
        public Dictionary<Type, TypeInfo> TypeInfos = new Dictionary<Type, TypeInfo>();


        [SerializeField]
        static WatchTypeRegistry _Instance = null;
        public static WatchTypeRegistry Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new WatchTypeRegistry();
                }
                return _Instance;
            }
        }

        public WatchTypeRegistry()
        {
#if !DEBUGWATCH_NODEFAULTREGISTRATION
            RegisterStringAccessors(typeof(float4x4), new DefaultStringAccessor<float4x4>.Factory());
#endif
        }
        public void RegisterVisualizer(Type t, IVisualizerFactory fac)
        {
            if (!TypeInfos.TryGetValue(t, out var ti))
            {
                TypeInfos.Add(t, ti = new TypeInfo());
            }
            ti.Visualizers.Remove(fac.GetName());
            ti.Visualizers.Add(fac.GetName(), fac);
        }

        public void RegisterAccessor(Type t, string accessorName, IAccessorFactory accessorFactory)
        {
            if (!TypeInfos.TryGetValue(t, out var ti))
            {
                TypeInfos.Add(t, ti = new TypeInfo());
            }
            ti.Accessors.Remove(accessorName);
            ti.Accessors.Add(accessorName, accessorFactory);
        }
        public void RegisterStringAccessors(Type t, IAccessorFactory stringAccessorFactory)
        {
            if (!TypeInfos.TryGetValue(t, out var ti))
            {
                TypeInfos.Add(t, ti = new TypeInfo());
            }
            ti.StringAccessorFactory = stringAccessorFactory;
        }
        public bool TryGetTypeInfo(Type t, out WatchTypeRegistry.TypeInfo ti)
        {
            return TypeInfos.TryGetValue(t, out ti);
        }
    }

    [Serializable]
    public class WatchRegistry
    {


        [Serializable]
        public class WatchInfo
        {
            public IWatch Watch;
            public IAccessor<string> StringAccessor;
            public List<(IVisualizerFactory, IVisualizer)> Visualizers = new List<(IVisualizerFactory, IVisualizer)>();

            public void RemoveAllVisualizers()
            {
                foreach (var vv in Visualizers)
                {
                    vv.Item1.Dispose(vv.Item2);
                }
                Visualizers.Clear();
            }
            public bool TryAddVisualizer(IVisualizerFactory visFactory, IAccessor accessor, out IVisualizer vis)
            {
                vis = visFactory.Create(Watch, accessor);
                if (vis != null)
                {
                    Visualizers.Add((visFactory, vis));
                    return true;
                }
                return false;
            }
            public bool TryGetWatchVisualizer(string visualizerName, out IVisualizer vis)
            {
                foreach (var vv in Visualizers)
                {
                    if (vv.Item1.GetName() == visualizerName)
                    {
                        vis = vv.Item2;
                        return true;
                    }
                }
                vis = default;
                return false;
            }

            public int TryRemoveWatchVisualizer(string visualizerName)
            {
                int count = 0;
                for (int i = 0; i != Visualizers.Count;)
                {
                    var vv = Visualizers[i];
                    if (vv.Item1.GetName() == visualizerName)
                    {
                        vv.Item1.Dispose(vv.Item2);
                        Visualizers.RemoveAt(i);
                        ++count;
                    }
                    else
                    {
                        ++i;
                    }
                }
                return count;
            }
            public bool TryRemoveWatchVisualizer(IVisualizer vis)
            {
                foreach (var vv in Visualizers)
                {
                    if (vv.Item2 == vis)
                    {
                        vv.Item1.Dispose(vv.Item2);
                        Visualizers.Remove(vv);
                        return true;
                    }
                }
                return false;
            }

        }
        [SerializeField]
        public List<WatchInfo> Watches = new List<WatchInfo>();
        public WatchTypeRegistry WatchTypeRegistry;
        static WatchRegistry _Instance = null;
        public static WatchRegistry Instance
        {
            get
            {
                //if (_Instance == null)
                //{
                    
                //    _Instance = CreateInstance<WatchRegistry>();
                //    _Instance.hideFlags = HideFlags.DontSaveInBuild;
                //    //_Instance.WatchRegistry = new WatchRegistry();
                //}
                //return _Instance;
                //return EditorScopeHook.Instance.WatchRegistry;
                if (_Instance == null)
                {
                    _Instance = new WatchRegistry(WatchTypeRegistry.Instance);
                }
                return _Instance;
            }
        }
        public WatchRegistry(WatchTypeRegistry watchTypeRegistry)
        {
            WatchTypeRegistry = watchTypeRegistry;
        }
        public IAccessor<string> CreateStringAccessor(IWatch w)
        {
            return CreateStringAccessor(w, w);
        }
        public IAccessor<string> CreateStringAccessor(IWatch w, IAccessor a)
        {
            if (a == null) a = w;
            var valueType = a.GetValueType();
            if (WatchTypeRegistry.TypeInfos.TryGetValue(valueType, out var ti))
            {
                if (ti.StringAccessorFactory != null)
                {
                    return ti.StringAccessorFactory.Create(w, a) as IAccessor<string>;
                }
#if !DEBUGWATCH_NODEFAULTSTRINGACCESSOR
                Type genType = typeof(DefaultStringAccessor<>);
                Type[] typeParams = new Type[] { valueType };
                Type genType2 = genType.MakeGenericType(typeParams);
                return (IAccessor<string>)Activator.CreateInstance(genType2, a);
#endif
            }
            return null;
        }
        public bool TryRemoveWatch(WatchInfo wi)
        {
            wi.RemoveAllVisualizers();
            return Watches.Remove(wi);

        }
        public bool TryAddWatch(IWatch watch, out WatchInfo wi)
        {
            wi = AddWatch(watch);
            return true;
        }
        public bool TryAddWatch(IWatch watch, string visualizerName, IAccessor accessor, out WatchInfo wi)
        {
            wi = AddWatch(watch);
            if (wi != null)
            {
                if (!TryAddWatchVisualizer(wi, visualizerName, accessor, out var vis))
                {
                    TryRemoveWatch(wi);
                    return false;
                }
                return true;
            }
            return false;
        }
        public bool TryAddWatch(IWatch watch, string visualizerName, out WatchInfo vis)
        {
            return TryAddWatch(watch, visualizerName, watch, out vis);
        }
        public bool TryGetWatchInfo(string name, out WatchInfo watchInfo)
        {
            foreach (var wi in Watches)
            {
                if (wi.Watch.GetName() == name)
                {
                    watchInfo = wi;
                    return true;
                }
            }
            watchInfo = default;
            return false;
        }
        public WatchInfo AddWatch(IWatch watch)
        {
            var watchInfo = new WatchInfo() { Watch = watch };
            Watches.Add(watchInfo);
            return watchInfo;
        }
        public bool TryAddWatchVisualizer(WatchInfo watchInfo, string visualizerName, IAccessor accessor, out IVisualizer vis)
        {
            var t = watchInfo.Watch.GetValueType();
            if (WatchTypeRegistry.TypeInfos.TryGetValue(t, out var ti))
            {
                if (ti.Visualizers.TryGetValue(visualizerName, out var visFactory))
                {
                    if (watchInfo.TryAddVisualizer(visFactory, accessor, out vis))
                    {
                        return true;
                    }
                }
            }
            vis = default;
            return false;
        }

    }



    [Serializable]
    public class WatchRegistryContainer : ScriptableObject
    {
        [SerializeField]
        public WatchRegistry WatchRegistry = new WatchRegistry(WatchTypeRegistry.Instance);
    }
}