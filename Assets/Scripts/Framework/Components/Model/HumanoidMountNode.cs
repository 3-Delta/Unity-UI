using System;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(HumanoidMountNode))]
public class HumanoidMountNodeInspector : EnumComponentListInspector<Transform, EHumanoidMountNode> {
}
#endif

// 挂点类型
public enum EHumanoidMountNode {
    LeftEye = 0,
    LeftEar,
    LeftShoulder,
    LeftArm,
    LeftHand,
    LeftLeg,
    LeftKnee,
    LeftFoot,

    RightEye,
    RightEar,
    RightShoulder,
    RightArm,
    RightHand,
    RightLeg,
    RightKnee,
    RightFoot,

    Nose,
    Mouth,
    Neck,
    Chest,
    Back,
    Belly,
    Ass,
}

[DisallowMultipleComponent]
public class HumanoidMountNode : EnumComponentList<Transform, EHumanoidMountNode> {
}


