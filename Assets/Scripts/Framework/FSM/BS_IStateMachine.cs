using System.Collections.Generic;

// Created by kaclok at 2017/06/10-17:29:04 Saturday on pc: KACLOK.
// Copyright@nullgame`s testgame. All rights reserved.

namespace FSM
{
    public interface IStateMachine : IName
    {
        // 状态
        Dictionary<string, IState> states { get; }
        IState currentState { get; }
        IState defaultState { get; set; }

        // 转换
        ITransition currentTransition { get; }
        bool isInTransition{ get; }

        void Add(string name, IState target);
        void Remove(string name);
        void SwitchTo(IState target, bool force = false);
        void SwitchTo(string name, bool force = false);
        bool Contains(string name);
        IState Get(string name);
        void Reset();
        void Stop();
    }
}
