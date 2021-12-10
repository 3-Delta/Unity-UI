using System;
using UnityEngine;

public interface IFader<T> {
    void Fade(T t);

    void OnBeginFade();
    void OnFading(double timeSinceBegin);
    void OnEndFade();
}

// 可以参考GameFramework的audioAgent的实现
[DisallowMultipleComponent]
public class AudioFader : MonoBehaviour, IFader<AudioClip> {
    public AudioSource audioSource;
    public AudioClip audioClip;
    private IFader<AudioClip> _faderImplementation;

    private void Awake() {
        if (audioSource != null) {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void SetAudioSource(AudioSource target) {
        audioSource = target;
    }

    public void Fade(AudioClip audioClip) {
        if (audioSource == null) {
            return;
        }

        this.audioClip = audioClip;
        if (audioSource.clip == null) {
            // 直接播放，不fade
            OnBeginFade();
            OnEndFade();
        }
        else {
            if (audioClip == audioSource.clip) {
                // do nothing
            }
            else {
                // 借助timer实现fading
            }
        }
    }

    public void OnBeginFade() { }

    public void OnFading(double timeSinceBegin) { }

    public void OnEndFade() { }
}
