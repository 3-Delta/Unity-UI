using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 系统内存监控器
[DisallowMultipleComponent]
public class MemoryTracer : MonoBehaviour
{
    private void Awake() {
        Application.lowMemory += OnLowMemory;
    }

    private void OnLowMemory() {
        
    }
}
