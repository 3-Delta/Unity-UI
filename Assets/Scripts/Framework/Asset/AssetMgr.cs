using System;
using System.Collections.Generic;
using UnityEngine;

using SO = System.Object;
using UO = UnityEngine.Object;

public class AssetMgr {
    public class AssetRequest {
        private ResourceRequest _request;
        private Action<UO> _onLoaded;

        private UO _asset;
        private int refCount = 0;

        public bool isCompleted { get; private set; }

        public T Get<T>() where T : UO {
            return _asset as T;
        }

        public AssetRequest Load(Action<UO> onLoaded) {
            _onLoaded += onLoaded;
            ++refCount;

            if (this._request == null) {
                this._request = Resources.LoadAsync(null);
                this._request.completed += _OnLoaded;
            }

            if (isCompleted) {
                this._onLoaded?.Invoke(_asset);
            }

            return this;
        }

        public void Unload(Action<UO> onLoaded) {
            _onLoaded -= onLoaded;
            --refCount;

            // 引用计数为0即可删除
            if (this._asset != null) {
                Resources.UnloadAsset(this._asset);
            }
            
            _asset = null;
            this._request = null;
        }

        private void _OnLoaded(AsyncOperation op) {
            ResourceRequest r = op as ResourceRequest;
            r.completed -= this._OnLoaded;

            // 加载过程中没有被取消
            if (this._request != null) {
                _asset = r.asset;
                this.isCompleted = true;
                this._onLoaded?.Invoke(this._asset);
            }
            else {
                _asset = null;
            }

            this._request = null;
        }
    }

    private static Dictionary<string, AssetRequest> loading = new Dictionary<string, AssetRequest>();

    public static AssetRequest Load(string path, Action<UO> onLoaded) {
        if (!loading.TryGetValue(path, out AssetRequest r)) {
            r = new AssetRequest();
            loading.Add(path, r);
        }

        r.Load(onLoaded);
        return r;
    }
    
    public static void Unload(ref AssetRequest request, Action<UO> onLoaded) {
        request.Unload(onLoaded);
    }
}
