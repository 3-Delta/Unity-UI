using System;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ActorMountNode))]
public class ActorMountNodeInspector : EnumComponentListInspector<Transform, EActorMountNode> {
}
#endif

// 挂点类型
public enum EActorMountNode {
    Root, // actor位置
    Model, // 控制model的逻辑
    Local, // 控制local的behaviour和controller的逻辑
    Remote, // 控制remote的behaviour和controller的逻辑
}

[DisallowMultipleComponent]
public class ActorMountNode : EnumComponentList<Transform, EActorMountNode> {
}


