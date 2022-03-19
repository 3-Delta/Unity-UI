using UnityEngine;
using System.Collections.Generic;
using ILRuntime.Other;
using System;
using System.Reflection;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Stack;

public struct InnerStruct {
    public Vector2 dir { get; set; }
    public float distance { get; set; }
}

public unsafe class InnerStructBinder : ValueTypeBinder<InnerStruct> {
    public override unsafe void AssignFromStack(ref InnerStruct ins, StackObject* ptr, IList<object> mStack) {
        // 内置struct从2开始，目的是避开自己的this,this占据第一位
        // 如果dir是第二个成员，这里应该是3
        var v = ILIntepreter.Minus(ptr, 2);
        float x = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 3);
        float y = *(float*)&v->Value;
        
        ins.dir = new Vector2(x, y);
        
        v = ILIntepreter.Minus(ptr, 4);
        ins.distance = *(float*)&v->Value;
    }

    public override unsafe void CopyValueTypeToStack(ref InnerStruct ins, StackObject* ptr, IList<object> mStack) {
        // 内置struct从2开始，目的是避开自己的this,this占据第一位
        // 如果dir是第二个成员，这里应该是3
        var v = ILIntepreter.Minus(ptr, 2);
        *(float*) &v->Value = ins.dir.x;
        
        v = ILIntepreter.Minus(ptr, 3);
        *(float*) &v->Value = ins.dir.y;
        
        v = ILIntepreter.Minus(ptr, 4);
        *(float*) &v->Value = ins.distance;
    }
}