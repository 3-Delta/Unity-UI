using System;
using System.Collections.Generic;

[Serializable]
public class BundleHistroy : DefaultJsonSerialize {
    [Serializable]
    public class PrioritySize {
        public EABPriority priority;
        public ulong size;
    }

    [Serializable]
    public class Record : DefaultJsonSerialize {
        #region 序列化数据
        public uint abVersion = 0;
        public ulong buildTime;
        public List<PrioritySize> sizes = new List<PrioritySize>(0);

        public string recordPath;
        #endregion
    }

    #region 序列化数据
    public List<Record> records = new List<Record>(0);
    #endregion

    public void Add(Record record) {
        if (record != null) {
            records.Add(record);
        }
    }
}
