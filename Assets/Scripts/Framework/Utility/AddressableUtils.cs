using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using SO = System.Object;

public static class AddressableUtils {
    public static bool LoadAssetAsync<TObject>(ref AsyncOperationHandle<TObject> handler, SO key, Action<AsyncOperationHandle<TObject>> onCompleted) {
        ReleaseAsset(ref handler, onCompleted);
        handler = Addressables.LoadAssetAsync<TObject>(key);
        if (handler.IsValid()) {
            if (handler.IsDone) {
                onCompleted?.Invoke(handler);
            }
            else {
                handler.Completed += onCompleted;
            }

            return true;
        }

        return false;
    }

    public static void ReleaseAsset<TObject>(ref AsyncOperationHandle<TObject> handler, Action<AsyncOperationHandle<TObject>> onCompleted) {
        if (handler.IsValid()) {
            handler.Completed -= onCompleted;

            Addressables.Release<TObject>(handler);
            handler = default;
        }
    }

    public static void ReleaseAsset(ref AsyncOperationHandle handler, Action<AsyncOperationHandle> onCompleted) {
        if (handler.IsValid()) {
            handler.Completed -= onCompleted;

            Addressables.Release(handler);
            handler = default;
        }
    }

    // 实例化
    public static void InstantiateAsync(ref AsyncOperationHandle<GameObject> handler, SO key, Action<AsyncOperationHandle<GameObject>> onCompleted, Transform parent, bool trackHandle = true) {
        ReleaseInstance(ref handler, onCompleted);
        handler = Addressables.InstantiateAsync(key, parent, trackHandle);
        if (handler.IsValid()) {
            if (handler.IsDone) {
                onCompleted?.Invoke(handler);
            }
            else {
                handler.Completed += onCompleted;
            }
        }
    }

    public static void InstantiateAsync(ref AsyncOperationHandle<GameObject> handler, SO key, Action<AsyncOperationHandle<GameObject>> onCompleted, InstantiationParameters insParms, bool trackHandle = true) {
        ReleaseInstance(ref handler, onCompleted);
        handler = Addressables.InstantiateAsync(key, insParms, trackHandle);
        if (handler.IsValid()) {
            if (handler.IsDone) {
                onCompleted?.Invoke(handler);
            }
            else {
                handler.Completed += onCompleted;
            }
        }
    }

    public static bool ReleaseInstance(ref AsyncOperationHandle handler, Action<AsyncOperationHandle> onCompleted) {
        if (handler.IsValid()) {
            handler.Completed -= onCompleted;

            bool ret = Addressables.ReleaseInstance(handler);
            handler = default;
            return ret;
        }

        return false;
    }

    public static bool ReleaseInstance(ref AsyncOperationHandle<GameObject> handler, Action<AsyncOperationHandle<GameObject>> onCompleted) {
        if (handler.IsValid()) {
            handler.Completed -= onCompleted;

            bool ret = Addressables.ReleaseInstance(handler);
            handler = default;
            return ret;
        }

        return false;
    }
}
