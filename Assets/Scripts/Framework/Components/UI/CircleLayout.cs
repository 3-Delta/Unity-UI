using UnityEngine;
using UnityEngine.UI;

// 圆形布局
[DisallowMultipleComponent]
public class UICircleLayoutGroup : LayoutGroup {
    public int radius = 20;
    public float space = 10;
    public float startAxis = 0.5f;

    public override void CalculateLayoutInputVertical() {
        Calc();
    }

    public override void SetLayoutHorizontal() { }

    public override void SetLayoutVertical() { }

    private void Calc() {
        Vector3 pos = Vector3.zero;
        Vector3 starVec = Vector3.up * radius + pos;

        for (int i = 0; i < rectChildren.Count; i++) {
            Quaternion qua = Quaternion.AngleAxis(i * space + startAxis * 360, Vector3.back);
            Vector3 point = qua * starVec;
            rectChildren[i].anchoredPosition = point;
        }

        for (int i = 0; i < rectChildren.Count; i++) {
            rectChildren[i].anchorMin = new Vector2(0.5f, 0.5f);
            rectChildren[i].anchorMax = new Vector2(0.5f, 0.5f);
        }
    }
}
