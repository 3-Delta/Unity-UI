using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum ERewardStatus {
    Locked = 0, // 未达标
    UnGot = 1, // 未获取
    Got = 2, // 已获取
}

[DisallowMultipleComponent]
public class RewardStatus : MonoBehaviour {
    public ActiverGroup lockedGroup;
    public ActiverGroup unGotGroup;
    public ActiverGroup gotGroup;

    public Button btn;
    public Action<ERewardStatus> onClicked;

    public ERewardStatus status { get; private set; } = ERewardStatus.Locked;

    public void Awake() {
        if (btn != null) {
            btn.onClick.AddListener(OnBtnClciked);
        }
    }

    public void Refresh(int targetStatus) {
        Refresh((ERewardStatus)targetStatus);
    }

    public void Refresh(ERewardStatus targetStatus) {
        status = targetStatus;

        if (lockedGroup != null) {
            lockedGroup.SetActive(status == ERewardStatus.Locked);
        }

        if (unGotGroup != null) {
            unGotGroup.SetActive(status == ERewardStatus.UnGot);
        }

        if (gotGroup != null) {
            gotGroup.SetActive(status == ERewardStatus.Got);
        }
    }

    private void OnBtnClciked() {
        onClicked?.Invoke(status);
    }
}
