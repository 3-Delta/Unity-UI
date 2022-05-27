using System;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class DropDownItem : MonoBehaviour {
    public Text text;
    public Button btn;
    public GameObject hightlight;
    public DropDownList dropDownList;

    // data
    public int optionId;

    private void Awake() {
        this.btn.onClick.AddListener(this.OnBtnClicked);
    }

    private void OnBtnClicked() {
        this.dropDownList.Select(this.optionId);
    }

    public void SetHighlight(bool toHighlight) {
        this.hightlight.SetActive(toHighlight);
    }

    public void Refresh(int optionId, string optionName) {
        this.optionId = optionId;

        this.text.text = optionName;
    }
}
