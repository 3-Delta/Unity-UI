using UnityEngine;

[DisallowMultipleComponent]
public class COWTransform : MonoBehaviour {
    public COWGo cow;
}

[DisallowMultipleComponent]
public class COWCp<T> : MonoBehaviour where T : Component {
    public COWGo<T> cow;
}
