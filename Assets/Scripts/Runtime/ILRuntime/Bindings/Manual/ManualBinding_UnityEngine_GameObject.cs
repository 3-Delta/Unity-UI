
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;
using UnityEngine;

namespace ILRuntime.Runtime.Generated
{
    // 这里必须手动注册重定向，因为对于热更的mono类型，ilruntime都认为是IlTypeInstance。
    // 对于Addcomponent来说，自动生成的绑定代码不会调用awake
    // 对于Getcomponet系列以及TryGetComponent来说，自动生成的绑定代码都是Getcomponent<global::MonoBehaviourAdapter.Adaptor>,而热更所有mono类型全部都是global::MonoBehaviourAdapter.Adaptor，那么此时无法区分
    // 具体是哪个热更的mono类型，所以还需要判定type是否匹配
    // 而自动生成的绑定文件不会判定类型 UnityEngine_GameObject_Binding.cs
    unsafe class ManualBinding_UnityEngine_GameObject
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain appDomain)
        {
            MethodBase method;
            Type[] args;
            Type type = typeof(UnityEngine.GameObject);
            Dictionary<string, List<MethodInfo>> genericMethods = new Dictionary<string, List<MethodInfo>>();
            List<MethodInfo> lst = null;                    
            foreach(var m in type.GetMethods())
            {
                if(m.IsGenericMethodDefinition)
                {
                    if (!genericMethods.TryGetValue(m.Name, out lst))
                    {
                        lst = new List<MethodInfo>();
                        genericMethods[m.Name] = lst;
                    }
                    lst.Add(m);
                }
            }
            args = new Type[]{typeof(global::MonoBehaviourAdapter.Adaptor)};
            if (genericMethods.TryGetValue("AddComponent", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(global::MonoBehaviourAdapter.Adaptor)))
                    {
                        method = m.MakeGenericMethod(args);
                        appDomain.RegisterCLRMethodRedirection(method, AddComponent_0);
            
                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::MonoBehaviourAdapter.Adaptor)};
            if (genericMethods.TryGetValue("GetComponent", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(global::MonoBehaviourAdapter.Adaptor)))
                    {
                        method = m.MakeGenericMethod(args);
                        appDomain.RegisterCLRMethodRedirection(method, GetComponent_1);
            
                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::MonoBehaviourAdapter.Adaptor)};
            if (genericMethods.TryGetValue("GetComponentInParent", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(global::MonoBehaviourAdapter.Adaptor)))
                    {
                        method = m.MakeGenericMethod(args);
                        appDomain.RegisterCLRMethodRedirection(method, GetComponentInParent_2);
            
                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::MonoBehaviourAdapter.Adaptor)};
            if (genericMethods.TryGetValue("GetComponentInChildren", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(global::MonoBehaviourAdapter.Adaptor)))
                    {
                        method = m.MakeGenericMethod(args);
                        appDomain.RegisterCLRMethodRedirection(method, GetComponentInChildren_3);
            
                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::MonoBehaviourAdapter.Adaptor)};
            if (genericMethods.TryGetValue("GetComponents", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(global::MonoBehaviourAdapter.Adaptor[])))
                    {
                        method = m.MakeGenericMethod(args);
                        // 参考：ENum.GetValues是如何处理数组的
                        appDomain.RegisterCLRMethodRedirection(method, GetComponents_4);
            
                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::MonoBehaviourAdapter.Adaptor)};
            if (genericMethods.TryGetValue("GetComponentsInParent", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(global::MonoBehaviourAdapter.Adaptor[]), typeof(System.Boolean)))
                    {
                        method = m.MakeGenericMethod(args);
                        appDomain.RegisterCLRMethodRedirection(method, GetComponentsInParent_5);
            
                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::MonoBehaviourAdapter.Adaptor)};
            if (genericMethods.TryGetValue("GetComponentsInChildren", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(global::MonoBehaviourAdapter.Adaptor[]), typeof(System.Boolean)))
                    {
                        method = m.MakeGenericMethod(args);
                        appDomain.RegisterCLRMethodRedirection(method, GetComponentsInChildren_6);
            
                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::MonoBehaviourAdapter.Adaptor)};
            if (genericMethods.TryGetValue("GetComponentsInParent", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(global::MonoBehaviourAdapter.Adaptor[])))
                    {
                        method = m.MakeGenericMethod(args);
                        appDomain.RegisterCLRMethodRedirection(method, GetComponentsInParent_7);
            
                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::MonoBehaviourAdapter.Adaptor)};
            if (genericMethods.TryGetValue("GetComponentsInChildren", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(global::MonoBehaviourAdapter.Adaptor[])))
                    {
                        method = m.MakeGenericMethod(args);
                        appDomain.RegisterCLRMethodRedirection(method, GetComponentsInChildren_8);
            
                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::MonoBehaviourAdapter.Adaptor)};
            if (genericMethods.TryGetValue("TryGetComponent", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(System.Boolean), typeof(global::MonoBehaviourAdapter.Adaptor).MakeByRefType()))
                    {
                        method = m.MakeGenericMethod(args);
                        appDomain.RegisterCLRMethodRedirection(method, TryGetComponent_9);
            
                        break;
                    }
                }
            }
        }
        
        static StackObject* AddComponent_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            // 成员方法的第一个参数为this
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.GameObject instance_of_this_method = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            if (instance_of_this_method == null)
                throw new System.NullReferenceException();
            __intp.Free(ptr_of_this_method);

            object result_of_this_method;
            var genericArgument = __method.GenericArguments;
            if (genericArgument != null && genericArgument.Length == 1) 
            {
                // AddComponent应该有且只有1个泛型参数
                var type = __method.GenericArguments[0];
                if (type is CLRType) 
                {
                    result_of_this_method = instance_of_this_method.AddComponent(type.TypeForCLR);  
                }
                else 
                {
                    // 热更DLL内的类型比较麻烦。首先我们得自己手动创建实例
                    // 手动创建实例是因为默认方式会new MonoBehaviour，这在Unity里不允许
                    var ilInstance = new ILTypeInstance(type as ILType, false);
                    //接下来创建Adapter实例
                    var clrInstance = instance_of_this_method.AddComponent<global::MonoBehaviourAdapter.Adaptor>();
                    // unity创建的实例并没有热更DLL里面的实例，所以需要手动赋值
                    // 没办法调用MonoBehaviourAdapter.CreateCLRInstance导致
                    clrInstance.ILInstance = ilInstance;
                    clrInstance.AppDomain = __domain;
                    // 这个实例默认创建的CLRInstance不是通过AddComponent出来的有效实例，所以得手动替换
                    ilInstance.CLRInstance = clrInstance;

                    // 交给ILRuntime的实例应该为ILInstance
                    result_of_this_method = clrInstance.ILInstance;

                    // 因为Unity调用这个方法时还没准备好所以这里补调一次  
                    clrInstance.Awake();
                }
                
                return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
            
            return __esp;
        }

        static StackObject* GetComponent_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.GameObject instance_of_this_method = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            if (instance_of_this_method == null)
                throw new System.NullReferenceException();
            __intp.Free(ptr_of_this_method);

            var genericArgument = __method.GenericArguments;
            // GetComponent应该有且只有1个泛型参数
            if (genericArgument != null && genericArgument.Length == 1) 
            {
                var type = genericArgument[0];
                object result_of_this_method = null;
                if (type is CLRType)
                {
                    // Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                    result_of_this_method = instance_of_this_method.GetComponent(type.TypeForCLR);
                }
                else
                {
                    // 因为所有DLL里面的MonoBehaviour实际都是这个MonoBehaviourAdapter.Adaptor，所以我们只能全取出来遍历查找
                    var clrInstances = instance_of_this_method.GetComponents<MonoBehaviourAdapter.Adaptor>();
                    for(int i = 0; i < clrInstances.Length; ++i)
                    {
                        var clrInstance = clrInstances[i];
                        // ILInstance为null, 表示是无效的MonoBehaviour，要略过
                        if (clrInstance.ILInstance != null)
                        {
                            if (clrInstance.ILInstance.Type == type)
                            {
                                // 交给ILRuntime的实例应该为ILInstance
                                result_of_this_method = clrInstance.ILInstance;
                                break;
                            }
                        }
                    }
                }
                return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
            return __esp;
        }

        static StackObject* GetComponentInParent_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.GameObject instance_of_this_method = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            if (instance_of_this_method == null)
                throw new System.NullReferenceException();
            __intp.Free(ptr_of_this_method);

            var genericArgument = __method.GenericArguments;
            // GetComponentInParent应该有且只有1个泛型参数
            if (genericArgument != null && genericArgument.Length == 1) 
            {
                var type = genericArgument[0];
                object result_of_this_method = null;
                if (type is CLRType)
                {
                    // Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                    result_of_this_method = instance_of_this_method.GetComponentInParent(type.TypeForCLR);
                }
                else
                {
                    // 因为所有DLL里面的MonoBehaviour实际都是这个MonoBehaviourAdapter.Adaptor，所以我们只能全取出来遍历查找
                    var clrInstances = instance_of_this_method.GetComponentsInParent<MonoBehaviourAdapter.Adaptor>();
                    for(int i = 0; i < clrInstances.Length; ++i)
                    {
                        var clrInstance = clrInstances[i];
                        // ILInstance为null, 表示是无效的MonoBehaviour，要略过
                        if (clrInstance.ILInstance != null)
                        {
                            if (clrInstance.ILInstance.Type == type)
                            {
                                // 交给ILRuntime的实例应该为ILInstance
                                result_of_this_method = clrInstance.ILInstance;
                                break;
                            }
                        }
                    }
                }
                return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
            return __esp;
        }

        static StackObject* GetComponentInChildren_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.GameObject instance_of_this_method = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            if (instance_of_this_method == null)
                throw new System.NullReferenceException();
            __intp.Free(ptr_of_this_method);

            var genericArgument = __method.GenericArguments;
            // 应该有且只有1个泛型参数
            if (genericArgument != null && genericArgument.Length == 1) 
            {
                var type = genericArgument[0];
                object result_of_this_method = null;
                if (type is CLRType)
                {
                    // Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                    result_of_this_method = instance_of_this_method.GetComponentInChildren(type.TypeForCLR);
                }
                else
                {
                    // 因为所有DLL里面的MonoBehaviour实际都是这个MonoBehaviourAdapter.Adaptor，所以我们只能全取出来遍历查找
                    var clrInstances = instance_of_this_method.GetComponentsInChildren<MonoBehaviourAdapter.Adaptor>();
                    for(int i = 0; i < clrInstances.Length; ++i)
                    {
                        var clrInstance = clrInstances[i];
                        // ILInstance为null, 表示是无效的MonoBehaviour，要略过
                        if (clrInstance.ILInstance != null)
                        {
                            if (clrInstance.ILInstance.Type == type)
                            {
                                // 交给ILRuntime的实例应该为ILInstance
                                result_of_this_method = clrInstance.ILInstance;
                                break;
                            }
                        }
                    }
                }
                return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
            return __esp;
        }

        static StackObject* GetComponents_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.GameObject instance_of_this_method = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            if (instance_of_this_method == null)
                throw new System.NullReferenceException();
            __intp.Free(ptr_of_this_method);

            var genericArgument = __method.GenericArguments;
            if (genericArgument != null && genericArgument.Length == 1) 
            {
                var type = genericArgument[0];
                object ret;
                if (type is CLRType)
                {
                    // Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                    ret = instance_of_this_method.GetComponents(type.TypeForCLR);
                }
                else 
                {
                    // 在stack中推入数组，参考自：CLRRedirections.EnumGetValues, 这里有推入数组的示例
                    List<ILTypeInstance> list = null;
                    // 因为所有DLL里面的MonoBehaviour实际都是这个MonoBehaviourAdapter.Adaptor，所以我们只能全取出来遍历查找
                    var clrInstances = instance_of_this_method.GetComponents<MonoBehaviourAdapter.Adaptor>();
                    for(int i = 0; i < clrInstances.Length; ++i)
                    {
                        var clrInstance = clrInstances[i];
                        // ILInstance为null, 表示是无效的MonoBehaviour，要略过
                        if (clrInstance.ILInstance != null)
                        {
                            if (clrInstance.ILInstance.Type == type)
                            {
                                if (list == null) {
                                    list = new List<ILTypeInstance>();
                                }
                                // 交给ILRuntime的实例应该为ILInstance
                                list.Add(clrInstance.ILInstance);
                            }
                        }
                    }
                    ret = list != null ? list.ToArray() : new ILTypeInstance[0];
                }
                return ILIntepreter.PushObject(__ret, __mStack, ret);
            }
            return __esp;
        }

        static StackObject* GetComponentsInParent_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.GameObject instance_of_this_method = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            if (instance_of_this_method == null)
                throw new System.NullReferenceException();
            __intp.Free(ptr_of_this_method);

            var genericArgument = __method.GenericArguments;
            // GetComponentInParent应该有且只有1个泛型参数
            if (genericArgument != null && genericArgument.Length == 1) 
            {
                var type = genericArgument[0];
                object ret;
                ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
                bool includeInactive = (bool)typeof(bool).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
                if (type is CLRType)
                {
                    // Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                    ret = instance_of_this_method.GetComponentsInParent(type.TypeForCLR, includeInactive);
                }
                else 
                {
                    List<ILTypeInstance> list = null;
                    // 因为所有DLL里面的MonoBehaviour实际都是这个MonoBehaviourAdapter.Adaptor，所以我们只能全取出来遍历查找
                    var clrInstances = instance_of_this_method.GetComponentsInParent<MonoBehaviourAdapter.Adaptor>(includeInactive);
                    for(int i = 0; i < clrInstances.Length; ++i)
                    {
                        var clrInstance = clrInstances[i];
                        // ILInstance为null, 表示是无效的MonoBehaviour，要略过
                        if (clrInstance.ILInstance != null)
                        {
                            if (clrInstance.ILInstance.Type == type)
                            {
                                if (list == null) {
                                    list = new List<ILTypeInstance>();
                                }
                                // 交给ILRuntime的实例应该为ILInstance
                                list.Add(clrInstance.ILInstance);
                            }
                        }
                    }
                    ret = list != null ? list.ToArray() : new ILTypeInstance[0];
                }
                return ILIntepreter.PushObject(__ret, __mStack, ret);
            }
            return __esp;
        }

        static unsafe StackObject* GetComponentsInChildren_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.GameObject instance_of_this_method = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            if (instance_of_this_method == null)
                throw new System.NullReferenceException();
            __intp.Free(ptr_of_this_method);

            var genericArgument = __method.GenericArguments;
            if (genericArgument != null && genericArgument.Length == 1) 
            {
                var type = genericArgument[0];
                object ret;
                ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
                bool includeInactive = (bool)typeof(bool).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
                if (type is CLRType)
                {
                    // Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                    ret = instance_of_this_method.GetComponentsInChildren(type.TypeForCLR, includeInactive);
                }
                else 
                {
                    List<ILTypeInstance> list = null;
                    // 因为所有DLL里面的MonoBehaviour实际都是这个MonoBehaviourAdapter.Adaptor，所以我们只能全取出来遍历查找
                    var clrInstances = instance_of_this_method.GetComponentsInChildren<MonoBehaviourAdapter.Adaptor>(includeInactive);
                    for(int i = 0; i < clrInstances.Length; ++i)
                    {
                        var clrInstance = clrInstances[i];
                        // ILInstance为null, 表示是无效的MonoBehaviour，要略过
                        if (clrInstance.ILInstance != null)
                        {
                            if (clrInstance.ILInstance.Type == type)
                            {
                                if (list == null) {
                                    list = new List<ILTypeInstance>();
                                }
                                // 交给ILRuntime的实例应该为ILInstance
                                list.Add(clrInstance.ILInstance);
                            }
                        }
                    }
                    ret = list != null ? list.ToArray() : new ILTypeInstance[0];
                }
                return ILIntepreter.PushObject(__ret, __mStack, ret);
            }
            return __esp;
        }
        static StackObject* GetComponentsInParent_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.GameObject instance_of_this_method = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            if (instance_of_this_method == null)
                throw new System.NullReferenceException();
            __intp.Free(ptr_of_this_method);

            var genericArgument = __method.GenericArguments;
            if (genericArgument != null && genericArgument.Length == 1) 
            {
                var type = genericArgument[0];
                object ret;
                if (type is CLRType)
                {
                    // Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                    ret = instance_of_this_method.GetComponentsInParent(type.TypeForCLR);
                }
                else 
                {
                    List<ILTypeInstance> list = null;
                    // 因为所有DLL里面的MonoBehaviour实际都是这个MonoBehaviourAdapter.Adaptor，所以我们只能全取出来遍历查找
                    var clrInstances = instance_of_this_method.GetComponentsInParent<MonoBehaviourAdapter.Adaptor>();
                    for(int i = 0; i < clrInstances.Length; ++i)
                    {
                        var clrInstance = clrInstances[i];
                        // ILInstance为null, 表示是无效的MonoBehaviour，要略过
                        if (clrInstance.ILInstance != null)
                        {
                            if (clrInstance.ILInstance.Type == type)
                            {
                                if (list == null) {
                                    list = new List<ILTypeInstance>();
                                }
                                // 交给ILRuntime的实例应该为ILInstance
                                list.Add(clrInstance.ILInstance);
                            }
                        }
                    }
                    ret = list != null ? list.ToArray() : new ILTypeInstance[0];
                }
                return ILIntepreter.PushObject(__ret, __mStack, ret);
            }
            return __esp;
        }

        static unsafe StackObject* GetComponentsInChildren_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.GameObject instance_of_this_method = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            if (instance_of_this_method == null)
                throw new System.NullReferenceException();
            __intp.Free(ptr_of_this_method);

            var genericArgument = __method.GenericArguments;
            // GetComponentInParent应该有且只有1个泛型参数
            if (genericArgument != null && genericArgument.Length == 1) 
            {
                var type = genericArgument[0];
                object ret;
                if (type is CLRType)
                {
                    // Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                    ret = instance_of_this_method.GetComponentsInChildren(type.TypeForCLR);
                }
                else 
                {
                    List<ILTypeInstance> list = null;
                    // 因为所有DLL里面的MonoBehaviour实际都是这个MonoBehaviourAdapter.Adaptor，所以我们只能全取出来遍历查找
                    var clrInstances = instance_of_this_method.GetComponentsInChildren<MonoBehaviourAdapter.Adaptor>();
                    for(int i = 0; i < clrInstances.Length; ++i)
                    {
                        var clrInstance = clrInstances[i];
                        // ILInstance为null, 表示是无效的MonoBehaviour，要略过
                        if (clrInstance.ILInstance != null)
                        {
                            if (clrInstance.ILInstance.Type == type)
                            {
                                if (list == null) {
                                    list = new List<ILTypeInstance>();
                                }
                                // 交给ILRuntime的实例应该为ILInstance
                                list.Add(clrInstance.ILInstance);
                            }
                        }
                    }
                    ret = list != null ? list.ToArray() : new ILTypeInstance[0];
                }
                return ILIntepreter.PushObject(__ret, __mStack, ret);
            }
            return __esp;
        }

        static unsafe StackObject* TryGetComponent_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.GameObject instance_of_this_method = (UnityEngine.GameObject) typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            if (instance_of_this_method == null)
                throw new System.NullReferenceException();
            var genericArgument = __method.GenericArguments;
            if (genericArgument != null && genericArgument.Length == 1) 
            {
                var type = genericArgument[0];
                bool result_of_this_method = false;
                object component = null;
                if (type is CLRType) {
                    // Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                    result_of_this_method = instance_of_this_method.TryGetComponent(type.TypeForCLR, out UnityEngine.Component unityComponent);
                    component = unityComponent;
                }
                else {
                    // 因为所有DLL里面的MonoBehaviour实际都是这个MonoBehaviourAdapter.Adaptor，所以我们只能全取出来遍历查找
                    var clrInstances = instance_of_this_method.GetComponentsInChildren<MonoBehaviourAdapter.Adaptor>();
                    for (int i = 0; i < clrInstances.Length; ++i) {
                        var clrInstance = clrInstances[i];
                        // ILInstance为null, 表示是无效的MonoBehaviour，要略过
                        if (clrInstance.ILInstance != null) {
                            if (clrInstance.ILInstance.Type == type) {
                                component = clrInstance.ILInstance;
                                result_of_this_method = true;
                                break;
                            }
                        }
                    }
                }

                ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
                switch (ptr_of_this_method->ObjectType) {
                    case ObjectTypes.StackObjectReference: {
                        var ___dst = ILIntepreter.ResolveReference(ptr_of_this_method);
                        object ___obj = component;
                        if (___dst->ObjectType >= ObjectTypes.Object) {
                            if (___obj is CrossBindingAdaptorType)
                                ___obj = ((CrossBindingAdaptorType) ___obj).ILInstance;
                            __mStack[___dst->Value] = ___obj;
                        }
                        else {
                            ILIntepreter.UnboxObject(___dst, ___obj, __mStack, __domain);
                        }
                    }
                        break;
                    case ObjectTypes.FieldReference: {
                        var ___obj = __mStack[ptr_of_this_method->Value];
                        if (___obj is ILTypeInstance) {
                            ((ILTypeInstance) ___obj)[ptr_of_this_method->ValueLow] = component;
                        }
                        else {
                            var ___type = __domain.GetType(___obj.GetType()) as CLRType;
                            ___type.SetFieldValue(ptr_of_this_method->ValueLow, ref ___obj, component);
                        }
                    }
                        break;
                    case ObjectTypes.StaticFieldReference: {
                        var ___type = __domain.GetType(ptr_of_this_method->Value);
                        if (___type is ILType) {
                            ((ILType) ___type).StaticInstance[ptr_of_this_method->ValueLow] = component;
                        }
                        else {
                            ((CLRType) ___type).SetStaticFieldValue(ptr_of_this_method->ValueLow, component);
                        }
                    }
                        break;
                    // case ObjectTypes.ArrayReference: {
                    //     var instance_of_arrayReference = __mStack[ptr_of_this_method->Value] as global::MonoBehaviourAdapter.Adaptor[];
                    //     instance_of_arrayReference[ptr_of_this_method->ValueLow] = component;
                    // }
                        break;
                }

                __intp.Free(ptr_of_this_method);
                ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
                __intp.Free(ptr_of_this_method);
                __ret->ObjectType = ObjectTypes.Integer;
                __ret->Value = result_of_this_method ? 1 : 0;
                return __ret + 1;
            }
            
            return __esp;
        }
    }
}
