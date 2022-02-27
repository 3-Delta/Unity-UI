using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

public class SAO_AudioSource : SpeedAdjustObserver {
    public AudioSource audioSource;
    
    protected override void ParseComponent() {
        audioSource = GetComponent<AudioSource>();
    }
    
    protected override void Restore() {
        originalSpeed = audioSource.pitch;
    }
    protected override void Recovery() {
        audioSource.pitch = originalSpeed;
    }
    
    protected override void DoAdjust() {
        audioSource.pitch = originalSpeed * speedMultiple;
    }
}
