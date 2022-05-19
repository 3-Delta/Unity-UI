[System.Serializable]
public class ActorData : IDisposer {
    public ulong guid;
    public uint csvId;

    // 各种对于数据的触发器
    // public System.Action<float, float> onHpChanged;
    // public System.Action onHpBeZero;

    public ActorData(uint guid, uint csvId) {
        this.guid = guid;
        this.Reset(csvId);
    }

    public virtual void Reset(uint csvId) {
        this.csvId = csvId;
    }
}
