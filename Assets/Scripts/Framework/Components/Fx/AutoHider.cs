using UnityEngine;

public class AutoHider : MonoBehaviour {
    public enum EAutoOp {
        Hide,
        Destroy,
    }

    public EAutoOp op = EAutoOp.Hide;
    public float time = 3f;

    public const string FuncName = "Auto";

    private void OnEnable() {
        CancelInvoke(FuncName);
        Invoke(FuncName, time);
    }

    private void Auto() {
        if (op == EAutoOp.Hide) {
            gameObject.SetActive(false);
        }
        else if (op == EAutoOp.Destroy) {
            Destroy(gameObject);
        }
    }
}
