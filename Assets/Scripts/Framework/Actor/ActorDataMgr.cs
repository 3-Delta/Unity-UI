using System.Collections.Generic;

public class ActorDataMgr {
    private Dictionary<ulong, ActorData> players = new Dictionary<ulong, ActorData>();
    private Dictionary<ulong, ActorData> npcs = new Dictionary<ulong, ActorData>();

    public bool TryGet(EActorType actorType, ulong guid, out ActorData data) {
        bool ret = false;
        if (actorType == EActorType.Player) {
            ret = this.players.TryGetValue(guid, out data);
        }
        else if (actorType == EActorType.Npc) {
            ret = this.npcs.TryGetValue(guid, out data);
        }

        data = default;
        return ret;
    }
}
