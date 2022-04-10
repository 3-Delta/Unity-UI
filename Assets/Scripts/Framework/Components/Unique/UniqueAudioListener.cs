using System;
using UnityEngine;

// 因为auolistener全局只能保持一个，所以需要保证这一点
[DisallowMultipleComponent]
[RequireComponent(typeof(AudioListener))]
public class UniqueAudioListener : UniqueDefaultMono<AudioListener> {
    
}
