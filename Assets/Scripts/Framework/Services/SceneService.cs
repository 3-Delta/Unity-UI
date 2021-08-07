using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 底层调用AssetService执行操作
public class SceneService : MonoBehaviour
{
    public class Scene : IEnumerator {
        public object Current => null;

        public bool MoveNext() {
            throw new System.NotImplementedException();
        }

        public void Reset() {
            throw new System.NotImplementedException();
        }
    }
}
