using System.Collections.Generic;

namespace FSM {
    // 状态机也是一个状态，可以实现多重嵌套状态机。
    public class StateMachine : State, IStateMachine {
        public Dictionary<string, IState> states { get; protected set; } = new Dictionary<string, IState>();
        public IState currentState { get; protected set; } = null;
        public IState defaultState { get; set; } = null;

        // 是否处于转换过程中
        public bool isInTransition { get; protected set; } = false;
        public ITransition currentTransition { get; protected set; } = null;

        public StateMachine(string name, IState defaultState) : base(name) {
            Reset(name, defaultState);
        }

        public void Reset() {
            currentState = defaultState;
        }

        public void Reset(string name, IState defaultState) {
            this.name = name;
            this.currentState = defaultState;
            this.defaultState = defaultState;
            this.defaultState.fsm = this;
        }

        public void Add(string name, IState target) {
            if (!Contains(name)) {
                states.Add(name, target);
                target.fsm = this;
            }
        }

        public void Remove(string name) {
            if (!Equal(name, currentState.name) && Contains(name)) {
                states[name].fsm = null;
                states.Remove(name);
            }
        }

        public void SwitchTo(IState target, bool force = false) {
            if (target != null) {
                if (force || !Equal(target.name, currentState.name)) {
                    IState oldState = currentState;
                    currentState = target;

                    oldState?.Exit(target);
                    target?.Enter(oldState);
                }
            }
        }

        public void SwitchTo(string name, bool force = false) {
            SwitchTo(Get(name), force);
        }

        public IState Get(string name) {
            states.TryGetValue(name, out IState ret);
            return ret;
        }

        public static bool Equal(string left, string right) {
            // 值类型的GetHashCode()就是值
            return left.Equals(right);
        }

        public bool Contains(string name) {
            return Get(name) != null;
        }

        // 状态相关
        public override void Enter(IState prev = null) {
            base.Enter(prev);
            currentState?.Enter(prev);
        }

        public override void Exit(IState next) {
            currentState?.Exit(next);
            base.Exit(next);
        }

        // 整个状态机的驱动从这里开始
        public override void Update(float deltaTime) {
            base.Update(deltaTime);

            if (isInTransition) {
                if (currentTransition != null) {
                    if (currentTransition.ShouleEnd()) {
                        currentTransition.Exit();
                        DoTransition(currentTransition);
                        isInTransition = false;
                    }
                    else {
                        currentTransition.Update(deltaTime);
                    }
                }
            }
            else {
                if (currentState != null) {
                    currentState.Update(deltaTime);
                    // 只遍历当前状态的转换
                    foreach (ITransition itr in currentState.transitions) {
                        if (itr.ShouldBegin()) {
                            itr.Enter();
                            isInTransition = true;
                            currentTransition = itr;
                            return;
                        }
                    }
                }
            }
        }

        public virtual void DoTransition(ITransition transition) {
            currentState?.Exit(transition.to);
            currentState = transition.to;
            currentState?.Enter(transition.from);
        }

        public virtual void Stop() {
            isInTransition = false;
            currentState = null;
            currentTransition = null;
            states.Clear();
        }
    }
}
