namespace FSM {
    public delegate bool BoolTransitionBool();

    public delegate float FloatTransitionBool();

    public class Transition : ITransition {
        public string name { get; private set; }
        public IState from { get; private set; }
        public IState to { get; private set; }

        public System.Func<float> transitionProgress;
        public System.Func<bool> transitionBegin;

        public Transition(string name, IState from = null, IState to = null) {
            this.name = name;
            this.from = from;
            this.to = to;
        }

        public virtual float OnTransitionProgress() {
            float progress = 1f;
            if (transitionProgress != null) {
                progress = transitionProgress();
            }

            return progress;
        }

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void Update(float deltaTime) { }

        public virtual bool ShouldBegin() {
            bool ret = false;
            if (transitionBegin != null) {
                ret = transitionBegin();
            }

            return ret;
        }

        public virtual bool ShouleEnd() {
            bool ret = OnTransitionProgress() >= 1f;
            return ret;
        }
    }
}
