using UnityEngine;

public class VD {
    public Transform transform { get; private set; }

    public void Init(Transform transform) {
        this.transform = transform;
        OnLoaded(transform);
    }

    protected virtual void OnLoaded(Transform transform) {
    }
    public virtual void OnDestroy() {
        transform = null;
    }

    public void ShowHide(bool toShow) {
        if (transform != null) {
            transform.gameObject.SetActive(toShow);
        }
    }
}

// VDL ：VDListener
public class VDL : VD {
    private AwakeDestroy listener;

    protected override void OnLoaded(Transform transform) {
        if (!transform.TryGetComponent<AwakeDestroy>(out listener)) {
            listener = transform.gameObject.AddComponent<AwakeDestroy>();
        }

        if (listener) {
            listener.onTrigger += ProcessEvent;
        }

        base.OnLoaded(transform);
    }
    public override void OnDestroy() {
        if (listener) {
            listener.onTrigger -= ProcessEvent;
        }

        base.OnDestroy();
    }

    protected virtual void ProcessEvent(bool toListen) {
    }
}

// VDLL ：VDListenerLayout
public class VDLL<TLayout> : VDL where TLayout : UILayoutBase, new() {
    [SerializeField] protected TLayout layout;

    protected override void OnLoaded(Transform transform) {
        base.OnLoaded(transform);

        this.layout = new TLayout();
        this.layout.TryBind(transform);
    }
}
