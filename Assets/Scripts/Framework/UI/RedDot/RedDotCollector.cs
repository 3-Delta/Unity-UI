using System;
using System.Collections.Generic;
using UnityEngine;

public enum ERedDotType {
    Pure,
    WithNumber,
    New,
}

// 每个UI根节点挂载的红点的配置
// 如果是动态生成的节点上需要红点，则使用proto的配置
[DisallowMultipleComponent]
public class RedDotCollector : MonoBehaviour {
    public List<RedDotIndexer> items = new List<RedDotIndexer>(2);

#region 动态注册
    public void Register(in RedDotIndexer indexer) {
        if (!this.items.Contains(indexer)) {
            this.items.Add(indexer);
        }
    }

    public void UnRegister(in RedDotIndexer indexer) {
        this.items.Remove(indexer);
    }
#endregion
}
