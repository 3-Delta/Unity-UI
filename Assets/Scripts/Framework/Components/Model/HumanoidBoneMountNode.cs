using System;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(HumanoidBoneMountNode))]
public class HumanoidBoneMountNodeInspector : EnumComponentListInspector<Transform, EHumanoidBoneMountNode> {
}
#endif

// 挂点类型
public enum EHumanoidBoneMountNode {
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
public class HumanoidBoneMountNode : EnumComponentList<Transform, EHumanoidBoneMountNode> {
}


