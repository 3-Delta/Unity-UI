using System;
using UnityEngine;
using UnityEngine.UI;

// button附带该脚本，然后在逻辑层刷新的时候设置reason到该脚本保存
// 然后在按钮的点击回调中判断该脚本的reason, 执行操作
// 主要是为了有个可视化的地方 快速的手动勾选改变reason，做到开发期间快捷测试
/*
    public enum EReason {
        Nil = 0, // 成功

        LackItem, // 缺少道具
        InvalidLevel, // 等级没达到
        NotInTimeRange, // 不在活动期间
        NoSelectTarget, // 没有选中物品
        HaveGot, // 已经领取
    }

    void Refresh() {
        EReason r = EReason.Nil;
        int level = 45;
        int limitLevel = 60;
        if (level < limitLevel) {
            r = EReason.InvalidLevel;
        }
        else if () {
            r = EReason.LackItem;
        }
        else if () {
            r = EReason.NotInTimeRange;
        }
        else if () {
            r = EReason.NoSelectTarget;
        }
        else if () {
            r = EReason.HaveGot;
        }

        void DoRefreshBtn(int rr) {
            EReason rrr = (EReason)rr;
            if (rrr == EReason.Nil) {
                // 不置灰, 可交互
            }
            else if (rrr == EReason.InvalidLevel) {
                // 置灰, 不可交互
            }
            else if (rrr == EReason.LackItem) {
                // 置灰, 不可交互
            }
            else if (rrr == EReason.NotInTimeRange) {
                // 置灰, 可交互, 点击提示
            }
            else if (rrr == EReason.NoSelectTarget) {
                // 置灰, 不可交互
            }
            else if (rrr == EReason.HaveGot) {
                // 置灰, 不可交互
            }
        }

        void OnCan() {
            // do success
        }

        void OnCant(int rr) {
            EReason rrr = (EReason)rr;
            if (rrr == EReason.InvalidLevel) {
                // 提示 等级不合理
            }
            else if (rrr == EReason.LackItem) {
                // 提示 缺少道具
            }
            else if (rrr == EReason.NotInTimeRange) {
                // 提示 不在活动期间
            }
            else if (rrr == EReason.NoSelectTarget) {
                // 提示 没有选中物品
            }
            else if (rrr == EReason.HaveGot) {
                // 提示 已经领取
            }
        }

        ButtonWithReason btnReason = null;
        btnReason.Refresh((int)r, OnCan, OnCant, DoRefreshBtn);
    }
*/
[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
public class ButtonWithReason : MonoBehaviour {
    [SerializeField] private Button _button;

    public Button button {
        get {
            if (this._button == null) {
                this._button = this.GetComponent<Button>();
            }

            return _button;
        }
    }

    // reason == 0 表示正常状态
    public int reason = 0;

    public Action OnSuccess;
    public Action<int> OnFail;

    // 根据reason刷新button的状态，比如 置灰，比如 取消点击交互功能, 比如 隐藏 按钮
    public Action<int> onDoRefresh;

    private void Awake() {
        this.button.onClick.AddListener(this.OnBtnClicked);
    }

    // 手动调整reason, 然后点击测试：各种reason下的点击逻辑
    [ContextMenu(nameof(OnBtnClicked))]
    private void OnBtnClicked() {
        if (this.reason == 0) {
            // 成功
            this.OnSuccess?.Invoke();
        }
        else {
            this.OnFail?.Invoke(this.reason);
        }
    }

    // 手动调整reason, 然后测试：各种reason下的按钮状态
    [ContextMenu(nameof(DoRefresh))]
    public void DoRefresh() {
        this.onDoRefresh?.Invoke(this.reason);
    }

    // 逻辑层调用
    public void Refresh(int reason, Action onSuccess, Action<int> onFail, Action<int> doRefresh) {
        this.reason = reason;
        this.OnSuccess = onSuccess;
        this.OnFail = onFail;
        this.onDoRefresh = doRefresh;
    }
}
