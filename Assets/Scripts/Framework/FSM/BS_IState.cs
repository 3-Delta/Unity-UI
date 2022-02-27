using System.Collections.Generic;


// Created by kaclok at 2017/06/10-17:28:48 Saturday on pc: KACLOK.
// Copyright@nullgame`s testgame. All rights reserved.
// https://study.163.com/course/courseLearn.htm?courseId=1003000008#/learn/video?lessonId=1003441078&courseId=1003000008

namespace FSM
{
    public interface IState : IName
    {
        // 所属状态机
        IStateMachine fsm { get; set; }
        float time { get; }

        void Enter(IState prev);
        void Exit(IState next);
        void Update(float deltaTime);

        // 状态转换相关接口
        List<ITransition> transitions { get; }
        void AddTransition(ITransition target);
        void RemoveTransition(ITransition target);
    }
}
