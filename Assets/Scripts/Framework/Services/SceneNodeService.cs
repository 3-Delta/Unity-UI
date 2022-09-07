using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SceneNodeService {
    public static readonly Dictionary<ESceneNode, Transform> nodes = new Dictionary<ESceneNode, Transform>();

    public static void Replace(ESceneNode type, Transform node) {
        nodes[type] = node;
    }

    public static bool TryGet(ESceneNode type, out Transform node) {
        return nodes.TryGetValue(type, out node);
    }

    public static int Count {
        get { return nodes.Count; }
    }

    public static void Clear() {
        nodes.Clear();
    }
}
