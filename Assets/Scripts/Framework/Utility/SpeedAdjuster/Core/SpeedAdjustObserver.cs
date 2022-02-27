using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

// https://ludiq.io/chronos
// https://answer.uwa4d.com/question/5f812d769424416784ef25a4
// 工作原理： 每个可能调整speed的entity都添加SpeedAdjustObserver组件，然后设置自己的所属group以及timescale,同时注册自己到一个容器中。
// local/remote NpcMgr控制所有npc, player也一样， 然后配合着每个npc都挂载SpeedAdjustObserver ， 每个player都挂载SpeedAdjustObserver， 同时也注册到SpeedAdjuster容器中。这样子其实相当于：
// npcMgr引用npc, npc挂载SpeedAdjustObserver，然后SpeedAdjuster引用SpeedAdjustObserver。
public abstract class SpeedAdjustObserver : MonoBehaviour {
    [SerializeField] private string _group = string.Empty;

    public string group {
        get { return _group; }
        set {
            if (value != _group) {
                UnRegister();
                _group = value;
                Register();
            }
        }
    }

    [SerializeField] protected float speedMultiple = 1f;

    private ESpeedStatus SpeedStatus {
        get { return SpeedAdjuster.GetTimeState(speedMultiple); }
    }

    public void Register() {
        SpeedAdjuster.Register(this);
    }

    public void UnRegister() {
        SpeedAdjuster.UnRegister(this);
    }

    protected float originalSpeed = 1f;

    protected virtual void Restore() {
        // 保存原始速度
    }

    protected virtual void Recovery() {
        // 还原原始速度
    }

    private void Awake() {
        ParseComponent();
        Restore();
    }

    protected virtual void ParseComponent() { }

    public void Adjust(float timeScale) {
        this.speedMultiple = timeScale;
        this.DoAdjust();
    }

    protected virtual void DoAdjust() { }
}
