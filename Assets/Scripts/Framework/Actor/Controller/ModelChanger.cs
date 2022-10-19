using UnityEngine;

// 专门替换模型，用于复用，或者变身
[DisallowMultipleComponent]
public class ModelChanger : HumanoidActorController {
    public Transform model { get; protected set; }

    public virtual void Load(string path) { }

    public void Unload() { }

    private void _OnLoaded() { }
}
