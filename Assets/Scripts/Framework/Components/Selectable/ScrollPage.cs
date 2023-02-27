using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 作用于：有左右箭头，并且两个箭头中间有可滑动的scrollrect的一些item，item还可以点击，并且有centeron的效果
[RequireComponent(typeof(ScrollRect))]
[DisallowMultipleComponent]
public class ScrollPage : MonoBehaviour {
    public LRArrowSwitcher arrowSwitcher;
    public CenterOnChild centerOnChild;
    [SerializeField] private COWCp<ToggleEx> cow;

    // 点击箭头触发 和 点击vd触发 和 手动滑动居中触发
    public Action<int /*index*/> onPageSelected;

    private void Awake() {
        // 强制1
        arrowSwitcher.mode = LRArrowSwitcher.ETravelMode.NoCircleKeepArrow;
        arrowSwitcher.countPerPage = 1;
        arrowSwitcher.onSwitch += _OnSwitchPage;

        centerOnChild.onCenter += _OnCenter;
    }

    // 点击箭头，箭头组件内部处理了UI的逻辑，需要vd居中，vd选中，以及
    // 外部逻辑更新
    private void _OnSwitchPage(int pageIndex, int startIndex, int rangeCount) {
        // 滚动scroll到currentIndex, 只处理UI，不抛事件
        this.centerOnChild.CenterOn(pageIndex); // 某个下标为index的vd节点居中
        
        // 某个page被选中, 只处理UI，不抛事件
        // this.cow.cow[pageIndex].SetSelected(true, false);
        
        // 通知外部进行逻辑以及UI处理
        this.onPageSelected?.Invoke(pageIndex);
    }
    
    // 点击vd, vd内部处理了选中的逻辑，还需要居中 和 箭头的配合
    private void _OnClickPage(int pageIndex) {
        // 滚动scroll到currentIndex, 只处理UI，不抛事件
        this.centerOnChild.CenterOn(pageIndex); // 某个下标为index的vd节点居中
        
        // 箭头UI配合
        arrowSwitcher.Switch(ref pageIndex, false);
        
        // 通知外部进行逻辑以及UI处理
        this.onPageSelected?.Invoke(pageIndex);
    }
    
    private void _OnCenter(Transform t, int pageIndex) {
        // 某个page被选中, 只处理UI，不抛事件
        // this.cow.cow[pageIndex].SetSelected(true, false);
        
        // 箭头UI配合
        arrowSwitcher.Switch(ref pageIndex, false);
        
        // 通知外部进行逻辑以及UI处理
        this.onPageSelected?.Invoke(pageIndex);
    }

    // 方便编辑器测试
    [ContextMenu(nameof(_InitAndSwicth))]
    private void _InitAndSwicth() {
        Init(centerOnChild.transform.childCount);
        int index = 0;
        SwitchTo(ref index);
    }

    public ScrollPage Init(int targetCount, Action<ToggleEx, int /* index */> onInitVd = null, Action<ToggleEx, int /* index */> onRefreshVd = null) {
        arrowSwitcher.SetCount((uint)targetCount);

        cow.cow.TryBuildOrRefresh(targetCount, onInitVd, onRefreshVd);
        centerOnChild.CollectChildren();
        return this;
    }

    public bool SwitchTo(ref int currentIndex) {
        // arrowSwitcher控制
        return arrowSwitcher.Switch(ref currentIndex, true);
    }
}
