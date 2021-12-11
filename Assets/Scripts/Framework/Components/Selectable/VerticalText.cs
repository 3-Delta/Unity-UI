using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// https://www.jianshu.com/p/dda58ed48623
// http://www.manew.com/thread-141645-1-1.html
// unity的ugui中文竖排
[RequireComponent(typeof(Text), typeof(ContentSizeFitter))]
[DisallowMultipleComponent]
public class VerticalText : MonoBehaviour {
    public Text text;
    public ContentSizeFitter sizeFitter;

    private void Awake() {
        if (text == null) {
            text = GetComponent<Text>();
        }

        if (sizeFitter == null) {
            sizeFitter = GetComponent<ContentSizeFitter>();
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }

    public void SetText(string content) {
        text.text = content;
    }
}
