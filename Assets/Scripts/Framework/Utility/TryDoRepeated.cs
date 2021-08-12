using System;
using UnityEngine;

public class TryDoRepeated {
    protected int hasTriedCount = 0;
    protected AsyncOperation asyncOperation;

    // 尝试总次数
    public int TryTotalCount { get; set; }

    // 返回一个AsyncOperation
    public Func<int, AsyncOperation> DoAction { get; set; }

    // 参数是成功/失败
    public Action<bool> EndAction { get; set; }

    // 异步操作是否异常
    public Func<AsyncOperation, bool> IsValid { get; set; }

    public TryDoRepeated() {
    }

    public TryDoRepeated(int tryTotalCount, Func<int, AsyncOperation> doAction, Action<bool> endAction,
        Func<AsyncOperation, bool> isValid) {
        this.TryTotalCount = tryTotalCount;
        this.DoAction = doAction;
        this.EndAction = endAction;
        this.IsValid = isValid;
    }

    public void Exec() {
        if (asyncOperation != null) {
            return;
        }

        hasTriedCount = 0;

        /*
         * 假如DoAction操作是
         * UnityWebRequestAsyncOperation op = UnityWebRequest.Get(null).SendWebRequest();
         * op.completed += OnGot;
         */
        asyncOperation = DoAction.Invoke(hasTriedCount);
        asyncOperation.completed += OnGot;
    }

    private void OnGot(AsyncOperation asyOp) {
        asyOp.completed -= OnGot;

        if (IsValid(asyOp)) {
            EndAction(true);
        }
        else {
            if (TryTotalCount > ++hasTriedCount) {
                asyncOperation = DoAction.Invoke(hasTriedCount);
                asyncOperation.completed += OnGot;
            }
            else {
                // 尝试多次之后依然失败
                EndAction(false);
            }
        }
    }
}