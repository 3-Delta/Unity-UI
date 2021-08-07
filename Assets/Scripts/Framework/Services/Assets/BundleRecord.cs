using System;
using System.Collections.Generic;

[Serializable]
public class BundleRecord : DefaultJsonSerialize {
    #region 序列化数据
    public uint id;
    public uint crc;
    public ulong size;

    public string bundlePath;
    public string[] assetPaths = new string[0];
    public uint[] dependencies = new uint[0];

    #endregion

    public static BundleRecord Empty = new BundleRecord();
}

[Serializable]
public class BundleRecords : DefaultJsonSerialize {
    #region 序列化数据
    public uint version;
    public uint crc; // 当前记录文件crc，便于校验是否被手动修改
    public ulong necessarySize;
    public ulong unNecessarySize;

    // 所有bundle数据
    public List<BundleRecord> bundles = new List<BundleRecord>(0);
    // 登录之前必须下载的bundleIds
    public uint[] necessaryBundleIds = new uint[0];
    // 登陆后后台慢慢下载的bundleIds
    public uint[] unNecessaryBundleIds = new uint[0];
    #endregion

    private static Dictionary<string, BundleRecord> path2NecessaryBundle = new Dictionary<string, BundleRecord>();
    private static Dictionary<string, BundleRecord> asset2NecessaryBundle = new Dictionary<string, BundleRecord>();

    private static Dictionary<string, BundleRecord> path2unNecessaryBundle = new Dictionary<string, BundleRecord>();
    private static Dictionary<string, BundleRecord> asset2unNecessaryBundle = new Dictionary<string, BundleRecord>();

    #region 序列化/反序列化
    #endregion

    #region 对外接口
    //public static bool ContainsAsset(string assetPath) {
    //    return asset2Bundle.TryGetValue(assetPath, out var _);
    //}

    //public static bool TryGetBundle(string assetPath, out BundleRecord bundle) {
    //    return asset2Bundle.TryGetValue(assetPath, out bundle);
    //}

    //public BundleRecord[] GetDependencies(string assetPath) {
    //    if (asset2Bundle.TryGetValue(assetPath, out BundleRecord bundle)) {
    //        return GetDependencies(bundle);
    //    }
    //    return ZeroArray<BundleRecord>.Value;
    //}

    //public BundleRecord[] GetDependencies(BundleRecord bundle) {
    //    if (bundle != null) {
    //        return Array.ConvertAll(bundle.dependencies, input => necessaryBundleIds[input]);
    //    }
    //    return ZeroArray<BundleRecord>.Value;
    //}
    #endregion
}
