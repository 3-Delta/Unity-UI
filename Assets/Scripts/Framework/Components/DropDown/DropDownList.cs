using System;
using UnityEngine;
using UnityEngine.UI;

// 下拉菜单
[DisallowMultipleComponent]
public class DropDownList : MonoBehaviour {
    public bool isExpand { get; private set; } = false;

    public Transform unExpandArrow;
    public Transform expandArrow;
    public Button btnExpand;

    public DropDownItem optionProto;
    public Transform optionParent;

    public Transform expandRoot;
    public Text text;
    public ScrollRect scrollList;

    private COWGo<DropDownItem> cowLoader;

    public Action<DropDownItem> onSelect;

    private float verticalNormalizedPosition = 1f;

    private void Awake() {
        this.btnExpand.onClick.AddListener(this.OnBtnExpandClicked);
        this.Expand(false);
    }

    private void OnBtnExpandClicked() {
        this.Expand(!this.isExpand);
    }

    public void Expand(bool toExpand) {
        this.expandRoot.gameObject.SetActive(toExpand);
        this.isExpand = toExpand;
        this.unExpandArrow.gameObject.SetActive(!toExpand);
        this.expandArrow.gameObject.SetActive(toExpand);

        if (toExpand) {
            this.scrollList.verticalNormalizedPosition = this.verticalNormalizedPosition;
        }
    }

    // -1表示全部取消选中
    public void Select(int optionId) {
        for (int i = 0, length = this.cowLoader.RealCount; i < length; ++i) {
            var one = this.cowLoader[i];
            if (one.optionId == optionId) {
                one.SetHighlight(true);

                this.text.text = one.text.text;
                this.onSelect?.Invoke(one);
            }
            else {
                one.SetHighlight(false);
            }
        }

        this.Expand(false);
    }

    public void MoveTo(float normalization) {
        this.scrollList.verticalNormalizedPosition = this.verticalNormalizedPosition = 1 - normalization;
    }

    public DropDownList TryBuild(int count, Action<DropDownItem, int /* index */> onInit, Action<DropDownItem, int /* index */> onRefresh, Action<DropDownItem> onSelect) {
        if (this.cowLoader == null) {
            this.cowLoader = new COWGo<DropDownItem>();
            this.cowLoader.parent = this.optionParent;
            this.cowLoader.proto = this.optionProto;
        }

        this.cowLoader.TryBuildOrRefresh(count, onInit, onRefresh);
        this.onSelect = onSelect;

        return this;
    }
}
