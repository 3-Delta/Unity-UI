using System.Collections.Generic;

public class ActorDataMgr<TActorData> where TActorData : ActorData {
    private Dictionary<ulong, TActorData> datas = new Dictionary<ulong, TActorData>();
    
    public bool TryGet(ulong guid, out TActorData data) {
        bool ret = this.datas.TryGetValue(guid, out data);
        return ret;
    }

    public bool Add(ulong guid, TActorData actorData) {
        bool exist = this.TryGet(guid, out actorData);
        if (!exist) {
            this.datas.Add(guid, actorData);
        }

        return exist;
    }
    public void Replace(ulong guid, TActorData actorData) {
        this.datas[guid] = actorData;
    }
    
    public bool Remove(ulong guid) {
        return this.datas.Remove(guid);
    }
}
