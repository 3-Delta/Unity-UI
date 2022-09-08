using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SceneNodeService {
    public static readonly Dictionary<ESceneNode, Component> nodes = new Dictionary<ESceneNode, Component>();

    public static void Replace(ESceneNode type, Transform node) {
        nodes[type] = node;
    }

    public static bool TryGet<T>(ESceneNode type, out T cp) where T : Component{
        bool has = nodes.TryGetValue(type, out Component t);
        cp = null;
        if (has) {
            return t.TryGetComponent<T>(out cp);
        }

        return false;
    }

    public static int Count {
        get { return nodes.Count; }
    }

    public static void Clear() {
        nodes.Clear();
    }
}
