using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
public class ReasonButton : MonoBehaviour {
    public Button button;

    [Header("results和reasons要等长")] [SerializeField]
    private List<Func<bool>> results;

    [SerializeField] private List<string> reasons;

    private Action onSuccess;
    private Action<int, string> onFail;

    private void Awake() {
        if (button == null) {
            button = GetComponent<Button>();
        }

        button.onClick.AddListener(OnBtnClicked);
    }

    public void Init(List<Func<bool>> results, List<string> reasons, Action onSuccess, Action<int, string> onFail) {
        this.results = results;
        this.reasons = reasons;
        this.onSuccess = onSuccess;
        this.onFail = onFail;
    }

    private void OnBtnClicked() {
        if (results != null) {
            for (int i = 0; i < results.Count; ++i) {
                if (results[i] != null && !results[i].Invoke()) {
                    onFail?.Invoke(i, reasons[i]);
                    return;
                }
            }
        }

        onSuccess?.Invoke();
    }
}