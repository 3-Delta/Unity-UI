using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.DebugWatch
{
    [Serializable]
    public class DebugWatchRegistryComponent : MonoBehaviour
    {
        [SerializeField]
        public WatchRegistryContainer WatchRegistry;

        public void CreateRegistry()
        {
            if (WatchRegistry == null)
            {
                WatchRegistry = WatchRegistryContainer.CreateInstance<WatchRegistryContainer>();
            }
        }
    }




}