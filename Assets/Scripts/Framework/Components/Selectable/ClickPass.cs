using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

// https://weibo.com/819881121?layerid=4615100947760932
// https://www.xuanyusong.com/archives/4773?continueFlag=7c391e54d66693e4d30f084d1ec0a97f
[DisallowMultipleComponent]
public class ClickPass : MonoBehaviour, IPointerClickHandler {
    // 穿透层数
    public uint passLayerCount = 1;

    private List<RaycastResult> results = new List<RaycastResult>();

    public void OnPointerClick(PointerEventData eventData) {
        PassEvent(eventData, ExecuteEvents.pointerClickHandler);
    }

    // 把事件传递下去
    public void PassEvent<T>(PointerEventData eventData, ExecuteEvents.EventFunction<T> function) where T : IEventSystemHandler {
        results.Clear();
        EventSystem.current.RaycastAll(eventData, results);
        GameObject current = eventData.pointerCurrentRaycast.gameObject;
        int hasPassCount = 0;
        for (int i = 0, length = results.Count; i < length; i++) {
            if (current != results[i].gameObject) {
                if (hasPassCount >= passLayerCount) {
                    break;
                }

                ++hasPassCount;
                ExecuteEvents.Execute(results[i].gameObject, eventData, function);
            }
        }
    }
}
