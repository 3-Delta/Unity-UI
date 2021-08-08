using System;
using System.Collections.Generic;
using System.IO;

using UnityEditor;

using UnityEngine;

public enum EABPriority {
    // 登录之前必须下载的
    Necessary = 0,
    UnNecessary,
    Max,
}

[Serializable]
public class ABAsset {
    public EABPriority priority = EABPriority.Necessary;
    public string assetPath;
    public string bundlePath;
}

// ab优先级限制
[Serializable]
public class ABAssetGroup {
    public EABPriority priority = EABPriority.Necessary;
    public string bundlePath;
    public string[] assetPaths;
}

// 高中低配置限制
[Serializable]
public class QualityABAssetGroup {
    public EABVariantQuality quality = EABVariantQuality.Mid;
    public ABAssetGroup[] groups;
}

// 语言限制
[Serializable]
public class LanQualityABAssetGroup {
    public SystemLanguage lan = SystemLanguage.ChineseSimplified;
    public QualityABAssetGroup groups;
}

public static class ABBuilder {

    private static readonly List<BundleRecord> NecessaryABs = new List<BundleRecord>();
    private static readonly List<BundleRecord> UnNecessaryABs = new List<BundleRecord>();

    private static List<ABAsset> GetAssets(bool editor) {
        List<ABAsset> assets = new List<ABAsset>();
        if (!editor) {
            // Editor走GetAssetGroups更加方便
        }
        else {
            // 自定义配置
        }
        return assets;
    }

    private static List<ABAssetGroup> AssetToGroup(List<ABAsset> assets) {
        List<ABAssetGroup> groups = new List<ABAssetGroup>();

        Dictionary<string, List<string>> bundle2AssetPaths = new Dictionary<string, List<string>>();
        Dictionary<string, EABPriority> bundle2Priority = new Dictionary<string, EABPriority>();
        for (int i = 0, length = assets.Count; i < length; ++i) {
            ABAsset abAsset = assets[i];
            if (!bundle2AssetPaths.TryGetValue(abAsset.bundlePath, out List<string> assetPaths)) {
                assetPaths = new List<string>();
                bundle2AssetPaths.Add(abAsset.bundlePath, assetPaths);
                bundle2Priority.Add(abAsset.bundlePath, abAsset.priority);
            }

            assetPaths.Add(abAsset.assetPath);
        }

        foreach (var kvp in bundle2AssetPaths) {
            ABAssetGroup group = new ABAssetGroup {
                priority = bundle2Priority[kvp.Key],
                bundlePath = kvp.Key,
                assetPaths = kvp.Value.ToArray()
            };
            groups.Add(group);
        }

        return groups;
    }

    private static List<ABAssetGroup> GetAssetGroups(bool editor) {
        List<ABAssetGroup> groups = new List<ABAssetGroup>();
        if (editor) {
            string[] editorABNames = AssetDatabase.GetAllAssetBundleNames();
            for (int i = 0, length = editorABNames.Length; i < length; i++) {
                string abName = editorABNames[i];
                string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(abName);

                BuildMenuItem.DisplayProgressBar("采集资源", abName, i, length);

                ABAssetGroup group = new ABAssetGroup {
                    // editor测试随便给个优先级
                    priority = (EABPriority)(i % (int)(EABPriority.Max)),
                    bundlePath = abName,
                    assetPaths = assetPaths
                };
                groups.Add(group);
            }
        }
        else {
            // 自定义配置
        }
        return groups;
    }

    public static void Build(bool editor) {
        List<ABAssetGroup> abs = GetAssetGroups(editor);
        Build(abs);

        // 或者：下面方案
        //List<ABAsset> assets = GetAssets(editor);
        //Build(assets);
    }

    public static void Build(List<ABAsset> assets) {
        Build(AssetToGroup(assets));
    }

    // 核心函数

    public static void Build(List<ABAssetGroup> groups) {
        NecessaryABs.Clear();
        UnNecessaryABs.Clear();

        if (groups.Count <= 0) {
            Debug.LogError("Failed to build AssetBundle because assets not enough");
            return;
        }

        List<BundleRecord> allBundles = new List<BundleRecord>();
        for (int i = 0, length = groups.Count; i < length; ++i) {
            ABAssetGroup group = groups[i];
            if (group.priority == EABPriority.Necessary) {
                BundleRecord br = new BundleRecord {
                    id = (uint)i,
                    bundlePath = group.bundlePath,
                    assetPaths = group.assetPaths
                };
                NecessaryABs.Add(br);
                allBundles.Add(br);
            }
            else if (group.priority == EABPriority.UnNecessary) {
                BundleRecord br = new BundleRecord {
                    id = (uint)i,
                    bundlePath = group.bundlePath,
                    assetPaths = group.assetPaths
                };

                UnNecessaryABs.Add(br);
                allBundles.Add(br);
            }
        }

        if (allBundles.Count <= 0) {
            Debug.LogError("Failed to build AssetBundle because bundle not enough");
            return;
        }

        BundleRecords records = BuildSetting.CurrentABRecords;
        if (BuildSetting.ChangeABVersion) {
            ++records.version;
        }
        string outputPath = BuildSetting.GetABOutputPath(records.version);
        BeforeBuild(outputPath);
        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(outputPath, allBundles.ConvertAll(bundle =>
                new AssetBundleBuild {
                    assetNames = bundle.assetPaths,
                    assetBundleName = bundle.bundlePath,
                    // unity会自动给 assetBundleVariant添加一个., 所以我们不需要手动添加.
                    assetBundleVariant = GlobalSetting.ABExtension
                }).ToArray(),
                BuildAssetBundleOptions.ChunkBasedCompression |
                BuildAssetBundleOptions.StrictMode |
                BuildAssetBundleOptions.DeterministicAssetBundle,
            // todo 是否追加hash的方式可以做到Unity内部增量打包ab呢？也就是如果法线资源没有发生变化，就不重新打包？？
            EditorUserBuildSettings.activeBuildTarget); ;

        if (manifest == null) {
            Debug.LogError("Failed to build AssetBundle");
            return;
        }

        AfterBuild(allBundles, manifest, records, outputPath);
    }

    private static void BeforeBuild(string outputPath) {
        if (!Directory.Exists(outputPath)) {
            Directory.CreateDirectory(outputPath);
        }
    }

    private static void AfterBuild(List<BundleRecord> bundles, AssetBundleManifest manifest, BundleRecords records, string abOutputPath) {
        var path2Bundle = new Dictionary<string, BundleRecord>();
        for (int i = 0, length = bundles.Count; i < length; ++i) {
            var bundle = bundles[i];
            path2Bundle[bundle.bundlePath] = bundle;
        }

        string[] abPaths = manifest.GetAllAssetBundles();
        foreach (var abRelativePathWithExtension in abPaths) {
            string abRelativePathNoExtension = ConvertAssetBundleName(abRelativePathWithExtension);
            string[] dependencys = Array.ConvertAll(manifest.GetAllDependencies(abRelativePathWithExtension), ConvertAssetBundleName);
            if (path2Bundle.TryGetValue(abRelativePathNoExtension, out var record)) {
                record.dependencies = Array.ConvertAll(dependencys, input => path2Bundle[input].id);
                string abFullPath = $"{abOutputPath}/{abRelativePathWithExtension}";
                if (File.Exists(abFullPath)) {
                    using (var stream = File.OpenRead(abFullPath)) {
                        record.size = (ulong)stream.Length;

                        record.crc = new CRC32().Compute(stream);
                        // AssetBundle.LoadFromFileAsync(abFullPath, record.crc);
                        // BuildPipeline.GetCRCForAssetBundle(abFullPath, out record.crc);
                    }
                }
                else {
                    Debug.LogErrorFormat("AssetBundle not found: {0}", abFullPath);
                }
            }
            else {
                Debug.LogErrorFormat("AssetBundle not exist: {0}", abRelativePathWithExtension);
            }
        }

        RecordBundle(bundles, records, abOutputPath);
    }

    public static void RecordBundle(List<BundleRecord> bundles, BundleRecords records, string abOutputPath) {
        // 1. abList
        records.bundles = bundles;
        records.necessaryBundleIds = Array.ConvertAll(NecessaryABs.ToArray(), input => input.id);
        records.unNecessaryBundleIds = Array.ConvertAll(UnNecessaryABs.ToArray(), input => input.id);

        ulong GetSize(List<BundleRecord> ls) {
            ulong size = 0;
            foreach (var bundle in ls) {
                size += bundle.size;
            }
            return size;
        }

        records.necessarySize = GetSize(NecessaryABs);
        records.unNecessarySize = GetSize(UnNecessaryABs);

        string versionPath = BuildSetting.GetABOutputVersionPath(records.version);
        records.ToJson(versionPath);
        // 从文件计算crc, 然后重新写入文件
        records.crc = new CRC32().Compute(versionPath);
        records.ToJson(versionPath);

        records.ToJson(BuildSetting.CurrentABVersionPath);

        // 2. ab构建历史记录
        BundleHistroy abHistroy = BuildSetting.CurrentABHistroy;
        TimeSpan ts = DateTime.Now.ToLocalTime() - DateTime.Parse("1970-1-1");
        BundleHistroy.Record rd = new BundleHistroy.Record {
            abVersion = records.version,

            necessarySize = records.necessarySize,
            unNecessarySize = records.unNecessarySize,

            recordPath = versionPath,
            buildTime = (ulong)ts.TotalSeconds
        };
        abHistroy.Add(rd);
        abHistroy.ToJson(BuildSetting.ABBuildHistroyPath);

        string outputPath = BuildSetting.GetABOutputPath(records.version);
        Debug.Log($"Build AssetBundle success at: {outputPath}");
        EditorUtility.RevealInFinder(outputPath);

        // 清理操作
        NecessaryABs.Clear();
        UnNecessaryABs.Clear();
        GC.Collect();
    }

    public static string ConvertAssetBundleName(string abName) {
        // 剔除后缀名 .unity3d
        string name = abName.Substring(0, abName.Length - GlobalSetting.ABExtension.Length - 1);

        // 剔除高中低配置
        // name = name.Substring(0, name.Length - GlobalSetting.ABVariantQualityNames[(int)GlobalSetting.ABVariantQuality].Length - 1);

        // 如果追加了hash, 还需要剔除hash
        return name;
    }
}
