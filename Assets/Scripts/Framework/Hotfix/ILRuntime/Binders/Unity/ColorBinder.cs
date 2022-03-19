using UnityEngine;
using System.Collections.Generic;
using ILRuntime.Other;
using System;
using System.Reflection;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Stack;

public unsafe class ColorBinder : ValueTypeBinder<Color>
{
    public override unsafe void AssignFromStack(ref Color ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        ins.r = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 2);
        ins.g = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 3);
        ins.b = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 4);
        ins.a = *(float*)&v->Value;
    }

    public override unsafe void CopyValueTypeToStack(ref Color ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        *(float*)&v->Value = ins.r;
        v = ILIntepreter.Minus(ptr, 2);
        *(float*)&v->Value = ins.g;
        v = ILIntepreter.Minus(ptr, 3);
        *(float*)&v->Value = ins.b;
        v = ILIntepreter.Minus(ptr, 5);
        *(float*)&v->Value = ins.a;
    }
    public static void ParseColor(out Color color, ILIntepreter intp, StackObject* ptr, IList<object> mStack)
    {
        var a = ILIntepreter.GetObjectAndResolveReference(ptr);
        if (a->ObjectType == ObjectTypes.ValueTypeObjectReference)
        {
            var src = *(StackObject**)&a->Value;
            color.r = *(float*)&ILIntepreter.Minus(src, 1)->Value;
            color.g = *(float*)&ILIntepreter.Minus(src, 2)->Value;
            color.b = *(float*)&ILIntepreter.Minus(src, 3)->Value;
            color.a = *(float*)&ILIntepreter.Minus(src, 4)->Value;
            intp.FreeStackValueType(ptr);
        }
        else
        {
            color = (Color)StackObject.ToObject(a, intp.AppDomain, mStack);
            intp.Free(ptr);
        }
    }

    public void PushColor(ref Color color, ILIntepreter intp, StackObject* ptr, IList<object> mStack)
    {
        intp.AllocValueType(ptr, CLRType);
        var dst = *((StackObject**)&ptr->Value);
        CopyValueTypeToStack(ref color, dst, mStack);
    }
    public override void RegisterCLRRedirection(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
    {
        BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        MethodBase method;
        Type[] args;
        Type type = typeof(Color);
    }
}
