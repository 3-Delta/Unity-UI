using System.IO;

using UnityEngine;

public interface IJsonSerialize {
    void FromJson(string filePath);
    void FromJsonContent(string content);
    void ToJson(string filePath);
    string ToJson();
}

public class DefaultJsonSerialize : IJsonSerialize {
    public virtual void FromJson(string filePath) {
        if (File.Exists(filePath)) {
            string json = File.ReadAllText(filePath);
            FromJsonContent(json);
        }
    }

    public virtual void FromJsonContent(string json) {
        JsonUtility.FromJsonOverwrite(json, this);
    }

    public virtual void ToJson(string filePath) {
        string json = ToJson();
        if (!File.Exists(filePath)) {
            using (File.Create(filePath)) { }
        }
        File.WriteAllText(filePath, json);
    }

    public virtual string ToJson() {
        string json = JsonUtility.ToJson(this);
        return json;
    }
}
