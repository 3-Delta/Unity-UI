using System.Collections.Generic;

namespace FSM {
    public class State : IState {
        // ×´Ì¬ÊÂ¼þ
        public System.Action<IState> OnEnter;
        public System.Action<IState> OnExit;
        public System.Action<float> OnUpdate;

        public IStateMachine fsm { get; set; } = null;
        public string name { get; protected set; } = null;
        public float time { get; protected set; } = 0f;

        public State(string name) {
            this.name = name;
        }

        public virtual void Enter(IState prev) {
            UnityEngine.Debug.LogError("Enter: " + name + " from: " + (prev == null ? "null" : prev.name));

            time = 0f;
            OnEnter?.Invoke(prev);
        }

        public virtual void Exit(IState next) {
            OnExit?.Invoke(next);
            time = 0f;
        }

        public virtual void Update(float deltaTime) {
            time += deltaTime;
            OnUpdate?.Invoke(deltaTime);
        }

        // ×´Ì¬×ª»»
        public List<ITransition> transitions { get; protected set; } = new List<ITransition>();

        public void AddTransition(ITransition target) {
            if (target != null && !transitions.Contains(target)) {
                transitions.Add(target);
            }
        }

        public void RemoveTransition(ITransition target) {
            if (transitions != null && transitions.Contains(target)) {
                transitions.Remove(target);
            }
        }
    }
}
