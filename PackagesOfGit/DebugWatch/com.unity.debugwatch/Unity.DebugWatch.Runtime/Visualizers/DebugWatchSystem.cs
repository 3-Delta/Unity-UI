using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Unity.DebugWatch.Visualizers
{


    [ExecuteAlways]
    [AlwaysUpdateSystem]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class DebugWatchSystem : SystemBase
    {

        List<IVisualizer> visualizers = new List<IVisualizer>();

        public void AddVisualizer(IVisualizer v)
        {
            visualizers.Add(v);
            Refresh();
        }

        public void RemoveVisualizer(IVisualizer v)
        {
            visualizers.Remove(v);
            Refresh();
        }
        public void Refresh()
        {

            foreach (var v in visualizers)
            {
                v.Update();
            }
        }
        protected override void OnUpdate()
        {
            Refresh();
        }

    }
}