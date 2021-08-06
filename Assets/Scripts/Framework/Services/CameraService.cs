using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraService {
    public static Camera Camera2d { get; set; }
    // 场景默认的相机
    public static Camera DefaultCamera3d { get; set; }
    // cutscene等相机,会替换DefaultCamera3d
    public static Camera CurrentCamera3d { get; set; }
}
