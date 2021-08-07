using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class BundleHistroy {
    [Serializable]
    public class Record {
        public int version = 0;
        public long buildTime;
        public ulong totalSize;
        public string recordPath;
    }

    public List<Record> records = new List<Record>();


    public void FromJson(string json) {

    }

    public void ToJson() {

    }
}
