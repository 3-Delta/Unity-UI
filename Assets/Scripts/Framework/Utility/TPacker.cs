using UnityEngine;

public class TPacker<T> {
    public T value;
}

// https://segmentfault.com/a/1190000015291516
// Unity中解决JsonUtility转换数组失败的BUG
public class JsonUtilityArrayPacker {
    public static string ToJson<T>(T target) {
        if (typeof(T).GetInterface("IList") != null) {
            TPacker<T> pack = new TPacker<T>();
            pack.value = target;
            string json = JsonUtility.ToJson(pack);
            return json.Substring(8, json.Length - 9);
        }

        return JsonUtility.ToJson(target);
    }

    public static T FromJson<T>(string json) {
        if (typeof(T).GetInterface("IList") != null) {
            json = "{\"value\":{value}}".Replace("{value}", json);
            TPacker<T> Pack = JsonUtility.FromJson<TPacker<T>>(json);
            return Pack.value;
        }

        return JsonUtility.FromJson<T>(json);
    }
}
