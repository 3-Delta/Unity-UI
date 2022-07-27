using System;

[Serializable]
public class NpcData : ActorData {

    public NpcData(uint guid, uint csvId) : base(guid, csvId) { }
}
