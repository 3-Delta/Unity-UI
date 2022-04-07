using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(FrameScaler))]
public class FrameScalerInspector : BaseInspector {
    public static readonly int[] scale = new[] { 15, 30, 45, 60, 90, 120 };
    public static readonly string[] scaleDisplay = new[] { "15", "30", "45", "60", "90", "120" };

    private FrameScaler component;
    private SerializedProperty frameRateProp = null;

    private void OnEnable() {
        component = (FrameScaler)target;

        frameRateProp = serializedObject.FindProperty("_frameRate");
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.BeginVertical("box");
        {
            float t = EditorGUILayout.Slider("FrameRate", frameRateProp.intValue, 1, 200);
            int frame = (int)t;
            
            int selectedFrame = GUILayout.SelectionGrid(GetSelectedGameSpeed(frame), scaleDisplay, 6);
            if (selectedFrame >= 0) {
                frame = GetGameSpeed(selectedFrame);
            }

            if (EditorApplication.isPlaying) {
                component.FrameRate = frame;
            }
            else {
                frameRateProp.intValue = frame;
            }
        }
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    private int GetGameSpeed(int selectedFrame) {
        if (selectedFrame < 0) {
            return scale[0];
        }

        if (selectedFrame >= scale.Length) {
            return scale[scale.Length - 1];
        }

        return scale[selectedFrame];
    }

    private int GetSelectedGameSpeed(float frame) {
        for (int i = 0; i < scale.Length; i++) {
            if (frame.CompareTo(scale[i]) == 0) {
                return i;
            }
        }

        return -1;
    }
}
#endif

[DisallowMultipleComponent]
public class FrameScaler : MonoBehaviour {
    [SerializeField] private int _frameRate = 30;

    public int FrameRate {
        get { return _frameRate; }
        set { Application.targetFrameRate = _frameRate = value; }
    }
}
