using System;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DummyNodes))]
public class DummyNodesInspector : EnumComponentListInspector<Transform, EDummyNode> {
}
#endif

// 挂点类型
public enum EDummyNode {
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

    LeftWeapon_1,
    LeftWeapon_2,
    LeftWeapon_3,
    RightWeapon_1,
    RightWeapon_2,
    RightWeapon_3,

    Skill_1,
    Skill_2,
    Skill_3,

    Count,
}

[DisallowMultipleComponent]
public class DummyNodes : EnumComponentList<Transform, EDummyNode> {
}


