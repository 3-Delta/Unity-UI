using UnityEngine;
using Hook;

#if UNITY_EDITOR
using UnityEditor;

// ref https://github.com/naivetang/HookUnityEngineMethod
[CustomEditor(typeof(UnityMethodHooker))]
public class UnityMethodHookerEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        UnityMethodHooker owner = target as UnityMethodHooker;
        if (GUILayout.Button("Hook")) {
            owner.Hook();
        }
        else if (GUILayout.Button("UnHook")) {
            owner.UnHook();
        }
    }
}
#endif

[DisallowMultipleComponent]
public class UnityMethodHooker : MonoBehaviour {
    public enum EHookType {
        LocalPosition,
        LocalEulerAngles,
        LocalRotation,
        LocalScale,

        WorldPosition,
        WorldEulerAngles,
        WorldRotation,

        SetActive,
        Enable,
    }

    public GameObject target;
    public EHookType hookType = EHookType.LocalPosition;

    private HookBase _hooker;

    [ContextMenu(nameof(Hook))]
    public void Hook() {
        if (hookType == EHookType.LocalPosition) {
            _hooker = new Hook_Transform_LocalPosition();
        }
        else if (hookType == EHookType.LocalEulerAngles) {
            _hooker = new Hook_Transform_LocalEulerAngles();
        }
        else if (hookType == EHookType.LocalRotation) {
            _hooker = new Hook_Transform_LocalRotation();
        }
        else if (hookType == EHookType.LocalScale) {
            _hooker = new Hook_Transform_LocalScale();
        }
        else if (hookType == EHookType.WorldPosition) {
            _hooker = new Hook_Transform_WorldPosition();
        }
        else if (hookType == EHookType.WorldEulerAngles) {
            _hooker = new Hook_Transform_WorldEulerAngles();
        }
        else if (hookType == EHookType.WorldRotation) {
            _hooker = new Hook_Transform_WorldRotation();
        }

        else if (hookType == EHookType.SetActive) {
            _hooker = new Hook_GameObject_SetActive();
        }
        else if (hookType == EHookType.Enable) {
            _hooker = new Hook_Behaviour_Enable();
        }

        _hooker?.TryHook(this.target);
    }

    [ContextMenu(nameof(UnHook))]
    public void UnHook() {
        _hooker?.UnHook();
        _hooker = null;
    }
}
