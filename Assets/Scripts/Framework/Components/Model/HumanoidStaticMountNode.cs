using System;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(HumanoidStaticMountNode))]
public class HumanoidStaticMountNodeInspector : EnumComponentListInspector<Transform, EHumanoidStaticMountNode> {
}
#endif

public enum EHumanoidStaticMountNode {
    Center,
}

[DisallowMultipleComponent]
public class HumanoidStaticMountNode : EnumComponentList<Transform, EHumanoidStaticMountNode> {
}


