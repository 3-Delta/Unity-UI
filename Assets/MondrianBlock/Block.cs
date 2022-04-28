using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public enum ESelectType {
    Red,
    Normal
}

public class Block : MonoBehaviour {
    public int x;
    public int y;
    public bool used = false;
    public BlockSetter setter;

    private void Awake() {
        this.GetComponent<Button>().onClick.AddListener(OnBtnClicked);
    }

    public void SetSelect(bool toSelect, ESelectType selectType, Color color) {
        var img = this.GetComponent<Image>();
        if (selectType == ESelectType.Red) {
            img.color = Color.red;
            used = true;
        }
        else {
            used = toSelect;
            img.color = toSelect ? color : Color.white;
        }
    }

    private void OnBtnClicked() {
        if (this.setter.status == EPlayTStatus.Set) {
            SetSelect(true, ESelectType.Red, Color.white);
        }
    }
}
