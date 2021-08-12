using System;
using System.Collections.Generic;
using UnityEngine.Events;

public static class SetValueUtility {
    public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct {
        if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
            return false;

        currentValue = newValue;
        return true;
    }

    public static bool SetClass<T>(ref T currentValue, T newValue) where T : class {
        if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
            return false;

        currentValue = newValue;
        return true;
    }
}