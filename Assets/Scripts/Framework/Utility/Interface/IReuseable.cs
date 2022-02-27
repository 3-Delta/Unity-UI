using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReuseable<T_Proto, T_Data> : IReset {
    // T_Data data { get; set; }
    void Init(T_Proto arg);
    void Refresh(T_Data data, int index);
    void Use();
    void Unuse();
}
