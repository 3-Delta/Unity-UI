using System.Collections.Generic;

public class ActorMgr {
    private Dictionary<ulong, Actor> players = new Dictionary<ulong, Actor>();
    private Dictionary<ulong, Actor> npcs = new Dictionary<ulong, Actor>();

    public bool TryGet(EActorType actorType, ulong guid, out Actor actor) {
        bool ret = false;
        if (actorType == EActorType.Player) {
            ret = this.players.TryGetValue(guid, out actor);
        }
        else if (actorType == EActorType.Npc) {
            ret = this.npcs.TryGetValue(guid, out actor);
        }

        actor = default;
        return ret;
    }
}
