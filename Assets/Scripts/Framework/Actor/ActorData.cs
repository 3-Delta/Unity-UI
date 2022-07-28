[System.Serializable]
public class ActorData : IDisposer {
    // from server
    public ulong guid { get; private set; }

    // from form
    public uint formId { get; private set; }

    // 各种对于数据的触发器
    // public System.Action<float, float> onHpChanged;
    // public System.Action onHpBeZero;

    public virtual ActorData Reset(ulong guid, uint csvId) {
        this.guid = guid;
        this.formId = csvId;
        return this;
    }
}
