
using System;
using Unity.Entities;
using UnityEngine;

namespace Unity.DebugWatch.Visualizers
{

    // Create visualizers that are updated in the DebugWatchSystem system, which is ran in the presentation group
    [Serializable]
    public abstract class WorldVisualizerFactory<TFactory, TVisualizer, TValue> : IVisualizerFactory
        where TFactory : IVisualizerFactory, new()
        where TVisualizer : Visualizer<TValue>
    {
        public enum UpdateSystem
        {
            Presentation,

        }
        UpdateSystem updateSystem;

        public WorldVisualizerFactory() 
        {
            updateSystem = UpdateSystem.Presentation;
        }
        public WorldVisualizerFactory(UpdateSystem updateSystem)
        {
            this.updateSystem = updateSystem;
        }
        public abstract string GetName();
        public abstract string GetDescription();
        public abstract bool TryCreate(IWatch w, IAccessor<TValue> a, out IVisualizer vis);
        public IVisualizer Create(IWatch w, IAccessor a)
        {
            if (a is IAccessor<TValue> aa)
            {
                if (TryCreate(w, aa, out var vis))
                {
                    if (w is IWorldWatch ww)
                    {
                        var world = ww.GetWorld();
                        switch (updateSystem)
                        {
                            case UpdateSystem.Presentation:
                                AddToPresentationSystem(world, vis);
                                break;
                            default:
                                break;
                        }
                    } 
                    return vis;
                }
            }
            return null;
        }
        public void Dispose(IVisualizer v)
        {
            var w = v.GetWatch();
            if (w is IWorldWatch ww)
            {
                var world = ww.GetWorld();
                var sys = world.GetExistingSystem<DebugWatchSystem>();
                if (sys != null)
                {
                    sys.RemoveVisualizer(v);
                }
            }
            v.Dispose();
        }

        protected bool AddToPresentationSystem(World world, IVisualizer vis)
        {
            var sys = world.GetOrCreateSystem<DebugWatchSystem>();
            if (sys != null)
            {
                sys.AddVisualizer(vis);
                return true;
            }
            return false;
        }
    }

}