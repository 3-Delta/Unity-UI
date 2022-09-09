using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorUtils
{
    // 两个方向向量是否是顺时针旋转,或者左右关系
    public static bool IsClockwise(Vector2 fromDir, Vector2 toDir) {
        var crossDir = Vector3.Cross(fromDir, toDir);
        return crossDir.z > 0;
    }
}
