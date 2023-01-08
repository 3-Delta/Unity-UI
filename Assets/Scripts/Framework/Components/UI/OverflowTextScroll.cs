using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// ref: https://copyfuture.com/blogs-details/202211130534096229
// https://zhuanlan.zhihu.com/p/596800428
[DisallowMultipleComponent]
[RequireComponent(typeof(Text), typeof(ContentSizeFitter))]
public class OverflowTextScroll : MonoBehaviour {
    public enum EScrollMode {
        R2L, // 从右到左
        L2R, // 从左到右

        R2LPingPong, // 从右到左震荡
        L2RPingPong, // 从左到右震荡
    }

    public EScrollMode scrollMode = EScrollMode.R2L;
    public float scrollSpeed = 80f;
    public bool useRealTime = false;
    // 如果显示区域可以显示全，是否强制scroll
    public bool forceScroll = false;

    // parent节点anchor和pivot都是0.5
    private RectTransform parentRect;
    private RectTransform selfRect;
    private Text text;

    // 超框的时候激活
    private bool canMove = false;
    private bool isReverse = false;

    private Vector2 curAnchorPos;
    private float leftEdge;
    private float rightEdge;

    private void Awake() {
        text = this.GetComponent<Text>();
        selfRect = this.GetComponent<RectTransform>();
        parentRect = this.transform.parent.GetComponent<RectTransform>();
    }

    private IEnumerator Start() {
        // ContentSizeFitter需要等一帧才会得到生效后的数据
        // ContentSizeFitter主要是收缩Text的渲染size，cachedTextGeneratorForLayout和rect保持一致
        yield return null;
        this.canMove = this.Judge();
        this.canMove |= this.forceScroll;
    }

    private void Update() {
        if (this.canMove) {
            var d = this.scrollSpeed * (this.useRealTime ? Time.unscaledDeltaTime : Time.deltaTime);
            curAnchorPos = this.selfRect.anchoredPosition;

            if (scrollMode == EScrollMode.R2L) {
                curAnchorPos.x -= d;
                if (curAnchorPos.x < this.leftEdge) {
                    curAnchorPos.x = this.rightEdge;
                }
            }
            else if (scrollMode == EScrollMode.L2R) {
                curAnchorPos.x += d;
                if (curAnchorPos.x > this.rightEdge) {
                    curAnchorPos.x = this.leftEdge;
                }
            }
            else if (scrollMode == EScrollMode.R2LPingPong) {
                if (!this.isReverse) {
                    curAnchorPos.x -= d;
                    if (curAnchorPos.x < this.leftEdge) {
                        this.isReverse = !this.isReverse;
                    }
                }
                else {
                    curAnchorPos.x += d;
                    if (curAnchorPos.x > this.rightEdge) {
                        this.isReverse = !this.isReverse;
                    }
                }
            }
            else if (scrollMode == EScrollMode.L2RPingPong) {
                if (this.isReverse) {
                    curAnchorPos.x -= d;
                    if (curAnchorPos.x < this.leftEdge) {
                        this.isReverse = !this.isReverse;
                    }
                }
                else {
                    curAnchorPos.x += d;
                    if (curAnchorPos.x > this.rightEdge) {
                        this.isReverse = !this.isReverse;
                    }
                }
            }

            this.selfRect.anchoredPosition = this.curAnchorPos;
        }
    }

    // 计算左右边界
    [ContextMenu(nameof(Judge))]
    public bool Judge() {
        var anchorX = (this.selfRect.anchorMin.x + this.selfRect.anchorMax.x) / 2;
        var displayWidth = this.parentRect.rect.width;
        var textWidth = this.selfRect.rect.width;

        // Text的渲染宽度大于显示区域的宽度,表示需要scroll
        bool can = textWidth > displayWidth;
        // 正常情况下这两句已经满足，但是如果初始情况下text不在parent的最左侧，那么这个处理就会出现问题
        // leftX = rectTrans.anchoredPosition.x - textWidth;
        // rightX = rectTrans.anchoredPosition.x + displayWidth;

        // 不管Text在parent下是什么位置，一定要保证在从右往左的时候，text的最右侧在parent的最左侧，text的最左侧要在parent的最右侧。
        this.leftEdge = -anchorX * displayWidth - (1 - selfRect.pivot.x) * textWidth;
        this.rightEdge = (1 - anchorX) * displayWidth + selfRect.pivot.x * textWidth;
        // 或者其实: rightEdge - leftEdge = displayWidth + textWidth;
        // this.rightEdge = this.leftEdge + displayWidth + textWidth;
        
        return can;
    }
}
