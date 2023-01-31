using System;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class LRArrowSwitcher : MonoBehaviour {
    public enum ETravelMode {
        Circle, // 循环
        CircleRightOnly, // 右侧循环
        CircleLeftOnly, // 右侧循环
        NoCircle, // 两侧边界模式.即到达边界的时候控制对应边按钮的显隐
    }

    public ETravelMode mode = ETravelMode.Circle;
    public Button leftArrow;
    public Button rightArrow;

    [Range(1, int.MaxValue)]
    public int countPerPage = 1;

    // index:startIndex:rangeCount
    public Action<int, int, int> onSwitch;

    public int DataCount { get; private set; }

    public int PageCount {
        get { return Mathf.CeilToInt(1f * DataCount / countPerPage); }
    }

    public int CurrentPageIndex { get; private set; } = 0;

    protected void Awake() {
        if (leftArrow != null) {
            leftArrow.onClick.AddListener(OnBtnLeftClicked);
        }

        if (rightArrow != null) {
            rightArrow.onClick.AddListener(OnBtnRightClicked);
        }
    }

    private void OnBtnLeftClicked() {
        --CurrentPageIndex;
        Switch();
    }

    private void OnBtnRightClicked() {
        ++CurrentPageIndex;
        Switch();
    }

    // 设置个数
    public LRArrowSwitcher SetCount(uint count) {
        DataCount = (int)count;
        return this;
    }

    // 设置currentIndex
    public bool Switch(ref int currentIndex) {
        if (0 <= currentIndex && currentIndex < PageCount) {
            this.CurrentPageIndex = currentIndex;
            this.Switch();
            return true;
        }
        else {
            this.CurrentPageIndex = currentIndex = -1;
            return false;
        }
    }

    private void Switch() {
        CurrentPageIndex = (CurrentPageIndex + PageCount) % PageCount;

        if (mode == ETravelMode.NoCircle) {
            leftArrow.gameObject.SetActive(CurrentPageIndex != 0);
            rightArrow.gameObject.SetActive(CurrentPageIndex != PageCount - 1);
        }
        else if (mode == ETravelMode.CircleRightOnly) {
            leftArrow.gameObject.SetActive(CurrentPageIndex != 0);
        }
        else if (mode == ETravelMode.CircleLeftOnly) {
            rightArrow.gameObject.SetActive(CurrentPageIndex != PageCount - 1);
        }

        int startIndex = CurrentPageIndex * countPerPage;
        int rangeCount = Math.Min(countPerPage, DataCount - startIndex);
        onSwitch?.Invoke(CurrentPageIndex, startIndex, rangeCount);
    }
}
