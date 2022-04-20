using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(TimeScaler))]
public class TimeScalerInspector : BaseInspector {
    public static readonly float[] scale = new[] { 0f, 0.01f, 0.1f, 0.25f, 0.5f, 1f, 1.5f, 2f, 4f, 8f };
    public static readonly string[] scaleDisplay = new[] { "0x", "0.01x", "0.1x", "0.25x", "0.5x", "1x", "1.5x", "2x", "4x", "8x" };

    private TimeScaler component;
    private SerializedProperty timeScaleProp = null;
    private SerializedProperty fixedTimeStep = null;

    private void OnEnable() {
        component = (TimeScaler)target;

        timeScaleProp = serializedObject.FindProperty("_timeScale");
        fixedTimeStep = serializedObject.FindProperty("_fixedTimeStep");
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.BeginVertical("box");
        {
            float v = EditorGUILayout.Slider("FixedTimeStep", fixedTimeStep.floatValue, 0.001f, 1f);
            if (EditorApplication.isPlaying) {
                component.FixedTimeStep = v;
            }
            else {
                fixedTimeStep.floatValue = v;
            }

            float gameSpeed = EditorGUILayout.Slider("TimeScale", timeScaleProp.floatValue, 0f, 8f);
            int selectedGameSpeed = GUILayout.SelectionGrid(GetSelectedGameSpeed(gameSpeed), scaleDisplay, 5);
            if (selectedGameSpeed >= 0) {
                gameSpeed = GetGameSpeed(selectedGameSpeed);
            }

            if (EditorApplication.isPlaying) {
                component.TimeScale = gameSpeed;
            }
            else {
                timeScaleProp.floatValue = gameSpeed;
            }
        }
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    private float GetGameSpeed(int selectedGameSpeed) {
        if (selectedGameSpeed < 0) {
            return scale[0];
        }

        if (selectedGameSpeed >= scale.Length) {
            return scale[scale.Length - 1];
        }

        return scale[selectedGameSpeed];
    }

    private int GetSelectedGameSpeed(float gameSpeed) {
        for (int i = 0; i < scale.Length; i++) {
            if (gameSpeed.CompareTo(scale[i]) == 0) {
                return i;
            }
        }

        return -1;
    }
}
#endif

[DisallowMultipleComponent]
public class TimeScaler : MonoBehaviour {
    [SerializeField] private float _timeScale = 1f;

    public float TimeScale {
        get { return _timeScale; }
        set { Time.timeScale = _timeScale = value >= 0f ? value : 0f; }
    }

    [SerializeField] private float _fixedTimeStep = 0.02f;

    public float FixedTimeStep {
        get { return _fixedTimeStep; }
        set { Time.fixedDeltaTime = _fixedTimeStep = value > 0.02f ? value : 0.02f; }
    }
}
