using System.Collections.Generic;

public class ActorMgr<TActor> where TActor : Actor {
    private readonly Dictionary<ulong, TActor> actors = new Dictionary<ulong, TActor>();

    public bool TryGet(ulong guid, out TActor actor) {
        bool ret = this.actors.TryGetValue(guid, out actor);
        return ret;
    }
    
    public bool Add(ulong guid, TActor actorData) {
        bool exist = this.TryGet(guid, out actorData);
        if (!exist) {
            this.actors.Add(guid, actorData);
        }

        return exist;
    }
    public void Replace(ulong guid, TActor actorData) {
        this.actors[guid] = actorData;
    }
    
    public bool Remove(ulong guid) {
        return this.actors.Remove(guid);
    }
}
