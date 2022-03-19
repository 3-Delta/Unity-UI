using UnityEngine;
using System.Collections.Generic;
using ILRuntime.Other;
using System;
using System.Reflection;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Stack;

public unsafe class TimeSpanBinder : ValueTypeBinder<TimeSpan>
{
    public override unsafe void AssignFromStack(ref TimeSpan ins, StackObject* ptr, IList<object> mStack)
    {
    }

    public override unsafe void CopyValueTypeToStack(ref TimeSpan ins, StackObject* ptr, IList<object> mStack)
    {
        
    }
    public override void RegisterCLRRedirection(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
    {
        BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        MethodBase method;
        Type[] args;
        Type type = typeof(TimeSpan);
    }
}
