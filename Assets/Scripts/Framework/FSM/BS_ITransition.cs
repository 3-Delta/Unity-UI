using System.Collections.Generic;

// Created by kaclok at 2017/06/11-14:20:25 Sunday on pc: KACLOK.
// Copyright@nullgame`s testgame. All rights reserved.

/// <summary>
/// 状态过渡接口
/// </summary>
namespace FSM
{
    public interface ITransition : IName
    {
        IState from { get;}
        IState to { get; }

        
        // 正在转换，做一些延时性的操作，返回一个进度
        float OnTransitionProgress();
        // 进入转换
        void Enter();
        // 退出转换
        void Exit();
        void Update(float deltaTime);
        // 能否开始过渡
        bool ShouldBegin();
        // 能否结束过渡
        bool ShouleEnd();
    }
}
