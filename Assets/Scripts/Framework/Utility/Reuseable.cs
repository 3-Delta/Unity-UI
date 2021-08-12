using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IReuseable<T_Proto, T_Data> : IReset
{
    // T_Data data { get; set; }
    void Init(T_Proto arg);
    void Refresh(T_Data data, int index);
    void Use();
    void Unuse();
}

public interface IGameObjectReuseable<T_Data> : IReuseable<GameObject, T_Data>
{
    void Init(GameObject arg);
}

public interface ITransformReuseable<T_Data> : IReuseable<Transform, T_Data>
{
    void Init(Transform arg);
}

public class ReuseableContainer<T_Proto, T_Vd, T_Data> where T_Vd : IReuseable<T_Proto, T_Data>, new()
{
    protected List<T_Vd> vds = new List<T_Vd>();
    
    public ReuseableContainer<T_Proto, T_Vd, T_Data> Build(int count, System.Func<T_Proto> cloneAction)
    {
        if (cloneAction != null)
        {
            int dataLength = count;
            int vdLength = vds.Count;
            if (dataLength > vdLength)
            {
                for (int i = vdLength; i < dataLength; ++i)
                {
                    T_Proto cloneProto = cloneAction.Invoke();

                    T_Vd vd = new T_Vd();
                    vd.Init(cloneProto);

                    vds.Add(vd);
                }
            }
        }
        return this;
    }
    public ReuseableContainer<T_Proto, T_Vd, T_Data> Refresh(List<T_Data> dataList)
    {
        int dataLength = dataList.Count;
        int vdLength = vds.Count;
        for (int i = 0; i < vdLength; ++i)
        {
            T_Vd vd = vds[i];
            if (i < dataLength)
            {
                T_Data data = dataList[i];
                vd.Use();
                vd.Refresh(data, i);
            }
            else
            {
                vd.Unuse();
            }
        }
        return this;
    }
    public void Clear()
    {
        vds.Clear();
    }
}

public class GameObjectReuseableContainer<T_Vd, T_Data> : ReuseableContainer<GameObject, T_Vd, T_Data> where T_Vd : IReuseable<GameObject, T_Data>, new()
{
}
public class TransformReuseableContainer<T_Vd, T_Data> : ReuseableContainer<Transform, T_Vd, T_Data> where T_Vd : IReuseable<Transform, T_Data>, new()
{
}

