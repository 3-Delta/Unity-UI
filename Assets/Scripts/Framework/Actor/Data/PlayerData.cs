using System;

[Serializable]
public class PlayerData : ActorData {

    public PlayerData(uint guid, uint csvId) : base(guid, csvId) { }
}
