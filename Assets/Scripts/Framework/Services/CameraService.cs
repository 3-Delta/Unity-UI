using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CameraService {
    public static Camera Camera2d { get; set; }
    // 场景默认的相机
    public static Camera SceneCamera3d { get; private set; }

    // cutscene等相机,会替换DefaultCamera3d
    public static Camera _currentCamera3d = null;
    public static Camera CurrentCamera3d {
        get {
            return _currentCamera3d;
        }
        set {
            _currentCamera3d = value;
        }
    }
}
