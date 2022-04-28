using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EPlayTStatus {
    Reset,
    Set,
    Play,
}

public class BlockSetter : MonoBehaviour {
    public EPlayTStatus status = EPlayTStatus.Set;
    public Button btnReset;
    public Button btnSet;
    public Button btnStart;

    public MondrianBlocks mondrianBlocks;

    private void Awake() {
        this.btnReset.onClick.AddListener(OnBtnClickedReset);
        this.btnSet.onClick.AddListener(OnBtnClickedSet);
        this.btnStart.onClick.AddListener(OnBtnClickedStart);

        status = EPlayTStatus.Set;
    }

    private void OnBtnClickedReset() {
        foreach (var block in mondrianBlocks.blocks) {
            block.SetSelect(false, ESelectType.Normal, Color.white);
        }

        foreach (var rct in this.mondrianBlocks.rcts) {
            rct.used = false;
        }

        status = EPlayTStatus.Reset;
    }

    private void OnBtnClickedSet() {
        if (status == EPlayTStatus.Reset) {
            this.status = EPlayTStatus.Set;
        }
    }

    private void OnBtnClickedStart() {
        if (status == EPlayTStatus.Set) {
            this.status = EPlayTStatus.Play;

            mondrianBlocks.Find(0, 0);
        }
    }
}
