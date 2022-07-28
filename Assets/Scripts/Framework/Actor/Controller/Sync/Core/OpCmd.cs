using System;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum EOpFlag {
    UnExecute = 0, // 未执行
    Executed, // 已经执行
    Verfied, // 经过server校验
}

[Serializable]
public struct OpCmd {
    public uint sequence;
    public EOpFlag opFlag;
    public OpInput input;
    public OpOutput output;

    public bool HasMoveInput {
        get { return this.input.move != EMoveKey.Nil; }
    }

    public bool HasSkillInput {
        get { return this.input.skill != ESkillKey.Nil; }
    }

    public bool HasInput {
        get { return this.input.move != EMoveKey.Nil && this.input.skill != ESkillKey.Nil; }
    }
}

// 移动按键控制
[Flags]
public enum EMoveKey {
    Nil = 0,

    Forward = 1,
    Backward = 2,
    Left = 4,
    Right = 8,
    Up = 16,
    Down = 32,

    Jump = 64,
}

// 技能按键控制,fire也属于技能
public enum ESkillKey {
    Nil = 0,

    NAttack1, // 普攻
    NAttack2,
    NAttack3,
    NAttack4,
    NAttack5,

    SAttack1, // 大招
    SAttack2,
    SAttack3,
    SAttack4,
    SAttack5,
}

[Serializable]
public struct OpInput {
    public EMoveKey move;
    public ESkillKey skill;

    public OpInput(EMoveKey move, ESkillKey skill) {
        this.move = move;
        this.skill = skill;
    }
}

[Serializable]
public struct OpOutput {
    public Vector3 position;
    public Vector3 eulerAngles;
    public Vector3 scale;
}

// 游戏实体
public class Go { }

public class GoProperty { }

public class GoStatus : IReset {
    public Go go;
    public int frame;
    public Lazy<List<GoProperty>> properties = new Lazy<List<GoProperty>>(() => { return new List<GoProperty>(); });

    public Vector3 position;
    public Vector3 erlurANgle;
    public Vector3 scale;
    public Vector3 velocity;

    public GoStatus() { }

    public GoStatus(Go go, int frame, IList<GoProperty> properties) {
        this.go = go;
        this.frame = frame;

        if (properties != null) {
            this.properties.Value.AddRange(properties);
        }
    }

    public GoStatus CopyFrom(GoStatus target) {
        this.go = target.go;
        this.frame = target.frame;

        this.properties.Value.Clear();
        this.properties.Value.AddRange(target.properties.Value);
        return this;
    }

    public void Reset() { }
}
