using System;
using System.Collections.Generic;

[Serializable]
public class BundleRecord : DefaultJsonSerialize {
    public static BundleRecord Empty = new BundleRecord();

    #region 序列化数据
    public uint id;
    public uint crc;
    public ulong size;

    public string bundlePath;
    public string[] assetPaths = new string[0];
    public uint[] dependencies = new uint[0];

    #endregion
}

// 这里记录的肯定是非streamassets，也就是在persistent下的ab, 加载资源的时候肯定先判断这个资源是否在persitent中，如果不在就去streamasset中加载
// 也就是说，将ab拷贝到atreamasset下之后，自然就不会在persistent下存在这个record
[Serializable]
public class BundleRecords : DefaultJsonSerialize {
    [Serializable]
    public class Record {
        public EABPriority priority;
        public ulong size;
        public uint[] ids = new uint[0];
    }
    
    #region 序列化数据
    public uint version;
    public uint crc; // 当前记录文件crc，便于校验是否被手动修改

    // 所有bundle数据
    public List<BundleRecord> abRecords = new List<BundleRecord>(0);
    // 登录之前必须下载的bundleIds, 以及 登陆后后台慢慢下载的bundleIds
    public List<Record> abIds = new List<Record>(0);
    #endregion

    #region 对外接口
    #endregion
}
