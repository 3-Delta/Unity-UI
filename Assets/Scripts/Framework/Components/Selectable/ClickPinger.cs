using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

// https://zhuanlan.zhihu.com/p/144098931
// 最近拼ui烦躁得很，经常button被各种遮挡=。=
[DisallowMultipleComponent]
public class ClickPinger : MonoBehaviour {
    [ReadOnly] [SerializeField] private List<RaycastResult> list = new List<RaycastResult>();

    private void Update() {
        /// 鼠标左键没有点击，就不执行判断逻辑
        if (!Input.GetMouseButtonDown(0)) {
            return;
        }

        ///相应的GameObject对象
        GameObject go = null;

        ///判断是否点再ui上
        if (EventSystem.current.IsPointerOverGameObject()) {
            go = ClickUI();
        }
        else {
            go = ClickScene();
        }

        if (go == null) {
            Debug.Log("Click Nothing");
        }
        else {
            // 高亮点中GameObject
            EditorGUIUtility.PingObject(go);
            Selection.activeObject = go;
            Debug.Log(go, go);
        }
    }

    /// <summary>
    /// 点中ui
    /// </summary>
    private GameObject ClickUI() {
        //场景中的EventSystem

        PointerEventData eventData = new PointerEventData(EventSystem.current);

        //鼠标位置
        eventData.position = Input.mousePosition;

        //调用所有GraphicsRacaster里面的Raycast，然后内部会进行排序，
        //直接拿出来，取第一个就可以用了
        EventSystem.current.RaycastAll(eventData, list);

        //这个函数抄的unity源码的，就是取第一个值
        var raycast = FindFirstRaycast(list);

        //获取父类中事件注册接口
        //如Button，Toggle之类的，毕竟我们想知道哪个Button被点击了，而不是哪张Image被点击了
        //当然可以细分为IPointerClickHandler, IBeginDragHandler之类细节一点的，各位可以自己取尝试
        var go = ExecuteEvents.GetEventHandler<IEventSystemHandler>(raycast.gameObject);

        //既然没拿到button之类的，说明只有Image挡住了，取点中结果即可
        if (go == null) {
            go = raycast.gameObject;
        }

        return go;
    }

    private RaycastResult FindFirstRaycast(List<RaycastResult> candidates) {
        for (var i = 0; i < candidates.Count; ++i) {
            if (candidates[i].gameObject == null)
                continue;

            return candidates[i];
        }

        return new RaycastResult();
    }

    /// <summary>
    /// 点中场景中对象
    /// 然后无聊嘛，顺便把点场景的也顺手做了，不过这部分网上介绍挺多的，就不展开说了。
    /// </summary>
    private GameObject ClickScene() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            GameObject go = hit.collider.gameObject;
            return go;
        }

        return null;
    }
}
