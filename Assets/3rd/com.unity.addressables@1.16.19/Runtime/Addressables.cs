using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.Util;
using UnityEngine.SceneManagement;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[assembly: InternalsVisibleTo("Unity.Addressables.Tests")]
#if UNITY_EDITOR
[assembly: InternalsVisibleTo("Unity.Addressables.Editor")]
#endif

namespace UnityEngine.AddressableAssets
{
    /// <summary>
    /// Exception to encapsulate invalid key errors.
    /// </summary>
    public class InvalidKeyException : Exception
    {
        /// <summary>
        /// The key used to generate the exception.
        /// </summary>
        public object Key { get; private set; }

        /// <summary>
        /// The type of the key used to generate the exception.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Construct a new InvalidKeyException.
        /// </summary>
        /// <param name="key">The key that caused the exception.</param>
        public InvalidKeyException(object key) : this(key, typeof(object)) {}

        /// <summary>
        /// Construct a new InvalidKeyException.
        /// </summary>
        /// <param name="key">The key that caused the exception.</param>
        /// <param name="type">The type of the key that caused the exception.</param>
        public InvalidKeyException(object key, Type type)
        {
            Key = key;
            Type = type;
        }

        ///<inheritdoc cref="InvalidKeyException"/>
        public InvalidKeyException() {}

        ///<inheritdoc/>
        public InvalidKeyException(string message) : base(message) {}

        ///<inheritdoc/>
        public InvalidKeyException(string message, Exception innerException) : base(message, innerException) {}

        ///<inheritdoc/>
        protected InvalidKeyException(SerializationInfo message, StreamingContext context) : base(message, context) {}

        /// <summary>
        /// Stores information about the exception.
        /// </summary>
        public override string Message
        {
            get
            {
                return base.Message + $", Key={Key}, Type={Type}";
            }
        }
    }

    /// <summary>
    /// Entry point for Addressable API, this provides a simpler interface than using ResourceManager directly as it assumes string address type.
    /// </summary>
    public static class Addressables
    {
        internal static bool reinitializeAddressables = true;
        internal static AddressablesImpl _impl = new AddressablesImpl(new LRUCacheAllocationStrategy(1000, 1000, 100, 10));
        
        // Addressables是一个静态入口，其实底层调用的都是impl的具体函数，impl是一个非static的类，可以new
        // 这样设计的目的就是为了将来替换impl的时候方便，其实是一种策略模式，类似于数据库操作对外提供一个公用接口，
        // 但是内部可以用oracle,mysql等不同的impl去实现, 这里impl其实就是实现的一种具体的方式
        static AddressablesImpl impl
        {
            get
            {
#if UNITY_EDITOR && UNITY_2019_3_OR_NEWER
                if (EditorSettings.enterPlayModeOptionsEnabled && reinitializeAddressables)
                {
                    reinitializeAddressables = false;
                    _impl.ReleaseSceneManagerOperation();
                    _impl = new AddressablesImpl(new LRUCacheAllocationStrategy(1000, 1000, 100, 10));
                }
#endif
                return _impl;
            }
        }
        /// <summary>
        /// Stores the ResourceManager associated with this Addressables instance.
        /// </summary>
        public static ResourceManager ResourceManager { get { return impl.ResourceManager; } }
        internal static AddressablesImpl Instance { get { return impl; } }

#if UNITY_EDITOR && UNITY_2019_3_OR_NEWER
        [InitializeOnLoadMethod]
        static void RegisterPlayModeStateChange()
        {
            EditorApplication.playModeStateChanged += SetAddressablesReInitFlagOnExitPlayMode;
        }

        static void SetAddressablesReInitFlagOnExitPlayMode(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.ExitingPlayMode)
                reinitializeAddressables = true;
        }

#endif

        /// <summary>
        /// The Instance Provider used by the Addressables System.
        /// </summary>
        public static IInstanceProvider InstanceProvider { get { return impl.InstanceProvider; } }

        /// <summary>
        /// Used to resolve a string using addressables config values
        /// </summary>
        /// <param name="id">The internal id to resolve.</param>
        /// <returns>Returns the string that the internal id represents.</returns>
        public static string ResolveInternalId(string id)
        {
            return impl.ResolveInternalId(id);
        }

        /// <summary>
        /// Functor to transform internal ids before being used by the providers.
        /// See the [TransformInternalId](xref:addressables-api-transform-internal-id) documentation for more details.
        /// </summary>
        static public Func<IResourceLocation, string> InternalIdTransformFunc
        {
            get { return impl.InternalIdTransformFunc; }
            set { impl.InternalIdTransformFunc = value; }
        }

        /// <summary>
        /// Options for merging the results of requests.
        /// If keys (A, B) mapped to results ([1,2,4],[3,4,5])...
        ///  - UseFirst (or None) takes the results from the first key
        ///  -- [1,2,4]
        ///  - Union takes results of each key and collects items that matched any key.
        ///  -- [1,2,3,4,5]
        ///  - Intersection takes results of each key, and collects items that matched every key.
        ///  -- [4]
        /// </summary>
        public enum MergeMode
        {
            /// <summary>
            /// Use to indicate that no merge should occur. The first set of results will be used.
            /// </summary>
            None = 0,
            /// <summary>
            /// Use to indicate that the merge should take the first set of results.
            /// </summary>
            UseFirst = 0,
            /// <summary>
            /// Use to indicate that the merge should take the union of the results.
            /// </summary>
            Union,
            /// <summary>
            /// Use to indicate that the merge should take the intersection of the results.
            /// </summary>
            Intersection
        }

        /// <summary>
        /// The name of the PlayerPrefs value used to set the path to load the addressables runtime data file.
        /// </summary>
        public const string kAddressablesRuntimeDataPath = "AddressablesRuntimeDataPath";
        const string k_AddressablesLogConditional = "ADDRESSABLES_LOG_ALL";

        /// <summary>
        /// The name of the PlayerPrefs value used to set the path to check for build logs that need to be shown in the runtime.
        /// </summary>
        public const string kAddressablesRuntimeBuildLogPath = "AddressablesRuntimeBuildLog";

        /// <summary>
        /// The subfolder used by the Addressables system for its initialization data.
        /// </summary>
        public static string StreamingAssetsSubFolder { get { return impl.StreamingAssetsSubFolder; } }

        /// <summary>
        /// The path used by the Addressables system for its initialization data.
        /// </summary>
        public static string BuildPath { get { return impl.BuildPath; } }

        /// <summary>
        /// The path that addressables player data gets copied to during a player build.
        /// </summary>
        public static string PlayerBuildDataPath { get { return impl.PlayerBuildDataPath; } }

        /// <summary>
        /// The path used by the Addressables system to load initialization data.
        /// </summary>
        public static string RuntimePath { get { return impl.RuntimePath; } }


        /// <summary>
        /// Gets the collection of configured <see cref="IResourceLocator"/> objects. Resource Locators are used to find <see cref="IResourceLocation"/> objects from user-defined typed keys.
        /// </summary>
        /// <value>The resource locators collection.</value>
        public static IEnumerable<IResourceLocator> ResourceLocators { get { return impl.ResourceLocators; } }

        [Conditional(k_AddressablesLogConditional)]
        internal static void InternalSafeSerializationLog(string msg, LogType logType = LogType.Log)
        {
            if (_impl == null)
                return;
            switch (logType)
            {
                case LogType.Warning:
                    _impl.LogWarning(msg);
                    break;
                case LogType.Error:
                    _impl.LogError(msg);
                    break;
                case LogType.Log:
                    _impl.Log(msg);
                    break;
            }
        }

        [Conditional(k_AddressablesLogConditional)]
        internal static void InternalSafeSerializationLogFormat(string format, LogType logType = LogType.Log, params object[] args)
        {
            if (_impl == null)
                return;
            switch (logType)
            {
                case LogType.Warning:
                    _impl.LogWarningFormat(format, args);
                    break;
                case LogType.Error:
                    _impl.LogErrorFormat(format, args);
                    break;
                case LogType.Log:
                    _impl.LogFormat(format, args);
                    break;
            }
        }

        /// <summary>
        /// Debug.Log wrapper method that is contional on the ADDRESSABLES_LOG_ALL symbol definition.  This can be set in the Player preferences in the 'Scripting Define Symbols'.
        /// </summary>
        /// <param name="msg">The msg to log</param>
        [Conditional(k_AddressablesLogConditional)]
        public static void Log(string msg)
        {
            impl.Log(msg);
        }

        /// <summary>
        /// Debug.LogFormat wrapper method that is contional on the ADDRESSABLES_LOG_ALL symbol definition.  This can be set in the Player preferences in the 'Scripting Define Symbols'.
        /// </summary>
        /// <param name="format">The string with format tags.</param>
        /// <param name="args">The args used to fill in the format tags.</param>
        [Conditional(k_AddressablesLogConditional)]
        public static void LogFormat(string format, params object[] args)
        {
            impl.LogFormat(format, args);
        }

        /// <summary>
        /// Debug.LogWarning wrapper method.
        /// </summary>
        /// <param name="msg">The msg to log</param>
        public static void LogWarning(string msg)
        {
            impl.LogWarning(msg);
        }

        /// <summary>
        /// Debug.LogWarningFormat wrapper method.
        /// </summary>
        /// <param name="format">The string with format tags.</param>
        /// <param name="args">The args used to fill in the format tags.</param>
        public static void LogWarningFormat(string format, params object[] args)
        {
            impl.LogWarningFormat(format, args);
        }

        /// <summary>
        /// Debug.LogError wrapper method.
        /// </summary>
        /// <param name="msg">The msg to log</param>
        public static void LogError(string msg)
        {
            impl.LogError(msg);
        }

        /// <summary>
        /// Debug.LogException wrapper method.
        /// </summary>
        /// <param name="op">The operation handle.</param>
        /// <param name="ex">The exception.</param>
        public static void LogException(AsyncOperationHandle op, Exception ex)
        {
            impl.LogException(op, ex);
        }

        /// <summary>
        /// Debug.LogErrorFormat wrapper method.
        /// </summary>
        /// <param name="format">The string with format tags.</param>
        /// <param name="args">The args used to fill in the format tags.</param>
        public static void LogErrorFormat(string format, params object[] args)
        {
            impl.LogErrorFormat(format, args);
        }

        /// <summary>
        /// Initialize Addressables system.  Addressables will be initialized on the first API call if this is not called explicitly.
        /// </summary>
        /// <returns>The operation handle for the request.</returns>
        //[Obsolete("We have added Async to the name of all asycn methods (UnityUpgradable) -> InitializeAsync(*)", true)]
        [Obsolete]
        public static AsyncOperationHandle<IResourceLocator> Initialize()
        {
            return InitializeAsync();
        }

        /// <summary>
        /// Initialize Addressables system.  Addressables will be initialized on the first API call if this is not called explicitly.
        /// See the [InitializeAsync](xref:addressables-api-initialize-async) documentation for more details.
        /// </summary>
        /// <returns>The operation handle for the request.</returns>
        public static AsyncOperationHandle<IResourceLocator> InitializeAsync()
        {
            return impl.InitializeAsync();
        }
        
        /// <summary>
        /// Additively load catalogs from runtime data.  In order for content catalog caching to work properly the catalog json file
        /// should have a .hash file associated with the catalog.  This hash file will be used to determine if the catalog
        /// needs to be updated or not.  If no .hash file is provided, the catalog will be loaded from the specified path every time.
        /// See the [LoadContentCatalogAsync](xref:addressables-api-load-content-catalog-async) documentation for more details.
        /// </summary>
        /// <param name="catalogPath">The path to the runtime data.</param>
        /// <param name="providerSuffix">This value, if not null or empty, will be appended to all provider ids loaded from this data.</param>
        /// <returns>The operation handle for the request.</returns>
        public static AsyncOperationHandle<IResourceLocator> LoadContentCatalogAsync(string catalogPath, string providerSuffix = null)
        {
            return impl.LoadContentCatalogAsync(catalogPath, false, providerSuffix);
        }

        /// <summary>
        /// Additively load catalogs from runtime data.  In order for content catalog caching to work properly the catalog json file
        /// should have a .hash file associated with the catalog.  This hash file will be used to determine if the catalog
        /// needs to be updated or not.  If no .hash file is provided, the catalog will be loaded from the specified path every time.
        /// See the [LoadContentCatalogAsync](xref:addressables-api-load-content-catalog-async) documentation for more details.
        /// </summary>
        /// <param name="catalogPath">The path to the runtime data.</param>
        /// <param name="autoReleaseHandle">If true, the async operation handle will be automatically released on completion.</param>
        /// <param name="providerSuffix">This value, if not null or empty, will be appended to all provider ids loaded from this data.</param>
        /// <returns>The operation handle for the request.</returns>
        public static AsyncOperationHandle<IResourceLocator> LoadContentCatalogAsync(string catalogPath, bool autoReleaseHandle, string providerSuffix = null)
        {
            return impl.LoadContentCatalogAsync(catalogPath, autoReleaseHandle, providerSuffix);
        }

        /// <summary>
        /// Load a single asset
        /// See the [Loading Addressable Assets](xref:addressables-api-load-asset-async) documentation for more details.
        /// </summary>
        /// <typeparam name="TObject">The type of the asset.</typeparam>
        /// <param name="location">The location of the asset.</param>
        /// <returns>Returns the load operation.</returns>
        public static AsyncOperationHandle<TObject> LoadAssetAsync<TObject>(IResourceLocation location)
        {
            return impl.LoadAssetAsync<TObject>(location);
        }

        /// <summary>
        /// Load a single asset
        /// See the [Loading Addressable Assets](xref:addressables-api-load-asset-async) documentation for more details.
        /// </summary>
        /// <typeparam name="TObject">The type of the asset.</typeparam>
        /// <param name="key">The key of the location of the asset.</param>
        /// <returns>Returns the load operation.</returns>
        public static AsyncOperationHandle<TObject> LoadAssetAsync<TObject>(object key)
        {
            return impl.LoadAssetAsync<TObject>(key);
        }
        
        /// <summary>
        /// Loads the resource locations specified by the keys.
        /// The method will always return success, with a valid IList of results. If nothing matches keys, IList will be empty
        /// See the [LoadResourceLocations](xref:addressables-api-load-resource-locations-async) documentation for more details.
        /// </summary>
        /// <param name="keys">The set of keys to use.</param>
        /// <param name="mode">The mode for merging the results of the found locations.</param>
        /// <param name="type">A type restriction for the lookup.  Only locations of the provided type (or derived type) will be returned.</param>
        /// <returns>The operation handle for the request.</returns>
        public static AsyncOperationHandle<IList<IResourceLocation>> LoadResourceLocationsAsync(IEnumerable keys, MergeMode mode, Type type = null)
        {
            return impl.LoadResourceLocationsAsync(keys, mode, type);
        }
        
        /// <summary>
        /// Request the locations for a given key.
        /// The method will always return success, with a valid IList of results. If nothing matches key, IList will be empty
        /// See the [LoadResourceLocations](xref:addressables-api-load-resource-locations-async) documentation for more details.
        /// </summary>
        /// <param name="key">The key for the locations.</param>
        /// <param name="type">A type restriction for the lookup.  Only locations of the provided type (or derived type) will be returned.</param>
        /// <returns>The operation handle for the request.</returns>
        public static AsyncOperationHandle<IList<IResourceLocation>> LoadResourceLocationsAsync(object key, Type type = null)
        {
            return impl.LoadResourceLocationsAsync(key, type);
        }
        
        /// <summary>
        /// Load multiple assets, based on list of locations provided.
        /// If any fail, all successful loads and dependencies will be released.  The returned .Result will be null, and .Status will be Failed.
        /// See the [Loading Addressable Assets](xref:addressables-api-load-asset-async) documentation for more details.
        /// </summary>
        /// <typeparam name="TObject">The type of the assets.</typeparam>
        /// <param name="locations">The locations of the assets.</param>
        /// <param name="callback">Callback Action that is called per load operation.</param>
        /// <returns>The operation handle for the request.</returns>
        public static AsyncOperationHandle<IList<TObject>> LoadAssetsAsync<TObject>(IList<IResourceLocation> locations, Action<TObject> callback)
        {
            return impl.LoadAssetsAsync(locations, callback, true);
        }

        /// <summary>
        /// Load multiple assets, based on list of locations provided.
        /// See the [Loading Addressable Assets](xref:addressables-api-load-asset-async) documentation for more details.
        /// </summary>
        /// <typeparam name="TObject">The type of the assets.</typeparam>
        /// <param name="locations">The locations of the assets.</param>
        /// <param name="callback">Callback Action that is called per load operation.</param>
        /// <param name="releaseDependenciesOnFailure">
        /// If all matching locations succeed, this parameter is ignored.
        /// When true, if any matching location fails, all loads and dependencies will be released.  The returned .Result will be null, and .Status will be Failed.
        /// When false, if any matching location fails, the returned .Result will be an IList of size equal to the number of locations attempted.  Any failed location will
        /// correlate to a null in the IList, while successful loads will correlate to a TObject in the list. The .Status will still be Failed.
        /// When true, op does not need to be released if anything fails, when false, it must always be released.
        /// </param>
        /// <returns>The operation handle for the request.</returns>
        public static AsyncOperationHandle<IList<TObject>> LoadAssetsAsync<TObject>(IList<IResourceLocation> locations, Action<TObject> callback, bool releaseDependenciesOnFailure)
        {
            return impl.LoadAssetsAsync(locations, callback, releaseDependenciesOnFailure);
        }

        /// <summary>
        /// Load multiple assets.
        /// Each key in the provided list will be translated into a list of locations.  Those many lists will be combined
        /// down to one based on the provided MergeMode.
        /// If any locations from the final list fail, all successful loads and dependencies will be released.  The returned
        /// .Result will be null, and .Status will be Failed.
        /// See the [Loading Addressable Assets](xref:addressables-api-load-asset-async) documentation for more details.
        /// </summary>
        /// <typeparam name="TObject">The type of the assets.</typeparam>
        /// <param name="keys">List of keys for the locations.</param>
        /// <param name="callback">Callback Action that is called per load operation.</param>
        /// <param name="mode">Method for merging the results of key matches.  See <see cref="MergeMode"/> for specifics</param>
        /// <returns>The operation handle for the request.</returns>
        public static AsyncOperationHandle<IList<TObject>> LoadAssetsAsync<TObject>(IEnumerable keys, Action<TObject> callback, MergeMode mode)
        {
            return impl.LoadAssetsAsync(keys, callback, mode, true);
        }
        
        /// <summary>
        /// Load multiple assets.
        /// Each key in the provided list will be translated into a list of locations.  Those many lists will be combined
        /// down to one based on the provided MergeMode.
        /// See the [Loading Addressable Assets](xref:addressables-api-load-asset-async) documentation for more details.
        /// </summary>
        /// <param name="keys">IEnumerable set of keys for the locations.</param>
        /// <param name="callback">Callback Action that is called per load operation.</param>
        /// <param name="mode">Method for merging the results of key matches.  See <see cref="MergeMode"/> for specifics</param>
        /// <param name="releaseDependenciesOnFailure">
        /// If all matching locations succeed, this parameter is ignored.
        /// When true, if any matching location fails, all loads and dependencies will be released.  The returned .Result will be null, and .Status will be Failed.
        /// When false, if any matching location fails, the returned .Result will be an IList of size equal to the number of locations attempted.  Any failed location will
        /// correlate to a null in the IList, while successful loads will correlate to a TObject in the list. The .Status will still be Failed.
        /// When true, op does not need to be released if anything fails, when false, it must always be released.
        /// </param>
        /// <returns>The operation handle for the request.</returns>
        public static AsyncOperationHandle<IList<TObject>> LoadAssetsAsync<TObject>(IEnumerable keys, Action<TObject> callback, MergeMode mode, bool releaseDependenciesOnFailure)
        {
            return impl.LoadAssetsAsync(keys, callback, mode, releaseDependenciesOnFailure);
        }
        
        /// <summary>
        /// Load all assets that match the provided key.
        /// If any fail, all successful loads and dependencies will be released.  The returned .Result will be null, and .Status will be Failed.
        /// See the [Loading Addressable Assets](xref:addressables-api-load-asset-async) documentation for more details.
        /// </summary>
        /// <typeparam name="TObject">The type of the assets.</typeparam>
        /// <param name="key">Key for the locations.</param>
        /// <param name="callback">Callback Action that is called per load operation (per loaded asset).</param>
        /// <returns>The operation handle for the request.</returns>
        public static AsyncOperationHandle<IList<TObject>> LoadAssetsAsync<TObject>(object key, Action<TObject> callback)
        {
            return impl.LoadAssetsAsync(key, callback, true);
        }

        /// <summary>
        /// Load all assets that match the provided key.
        /// See the [Loading Addressable Assets](xref:addressables-api-load-asset-async) documentation for more details.
        /// </summary>
        /// <typeparam name="TObject">The type of the assets.</typeparam>
        /// <param name="key">Key for the locations.</param>
        /// <param name="callback">Callback Action that is called per load operation (per loaded asset).</param>
        /// <param name="releaseDependenciesOnFailure">
        /// If all matching locations succeed, this parameter is ignored.
        /// When true, if any matching location fails, all loads and dependencies will be released.  The returned .Result will be null, and .Status will be Failed.
        /// When false, if any matching location fails, the returned .Result will be an IList of size equal to the number of locations attempted.  Any failed location will
        /// correlate to a null in the IList, while successful loads will correlate to a TObject in the list. The .Status will still be Failed.
        /// When true, op does not need to be released if anything fails, when false, it must always be released.
        /// </param>
        /// <returns>The operation handle for the request.</returns>
        public static AsyncOperationHandle<IList<TObject>> LoadAssetsAsync<TObject>(object key, Action<TObject> callback, bool releaseDependenciesOnFailure)
        {
            return impl.LoadAssetsAsync(key, callback, releaseDependenciesOnFailure);
        }

        /// <summary>
        /// Release asset.
        /// </summary>
        /// <typeparam name="TObject">The type of the object being released</typeparam>
        /// <param name="obj">The asset to release.</param>
        public static void Release<TObject>(TObject obj)
        {
            impl.Release(obj);
        }

        /// <summary>
        /// Release the operation and its associated resources.
        /// </summary>
        /// <typeparam name="TObject">The type of the AsyncOperationHandle being released</typeparam>
        /// <param name="handle">The operation handle to release.</param>
        public static void Release<TObject>(AsyncOperationHandle<TObject> handle)
        {
            impl.Release(handle);
        }

        /// <summary>
        /// Release the operation and its associated resources.
        /// </summary>
        /// <param name="handle">The operation handle to release.</param>
        public static void Release(AsyncOperationHandle handle)
        {
            impl.Release(handle);
        }

        /// <summary>
        /// Releases and destroys an object that was created via Addressables.InstantiateAsync.
        /// </summary>
        /// <param name="instance">The GameObject instance to be released and destroyed.</param>
        /// <returns>Returns true if the instance was successfully released.</returns>
        public static bool ReleaseInstance(GameObject instance)
        {
            return impl.ReleaseInstance(instance);
        }

        /// <summary>
        /// Releases and destroys an object that was created via Addressables.InstantiateAsync.
        /// </summary>
        /// <param name="handle">The handle to the game object to destroy, that was returned by InstantiateAsync.</param>
        /// <returns>Returns true if the instance was successfully released.</returns>
        public static bool ReleaseInstance(AsyncOperationHandle handle)
        {
            impl.Release(handle);
            return true;
        }

        /// <summary>
        /// Releases and destroys an object that was created via Addressables.InstantiateAsync.
        /// </summary>
        /// <param name="handle">The handle to the game object to destroy, that was returned by InstantiateAsync.</param>
        /// <returns>Returns true if the instance was successfully released.</returns>
        public static bool ReleaseInstance(AsyncOperationHandle<GameObject> handle)
        {
            impl.Release(handle);
            return true;
        }

        /// <summary>
        /// Determines the required download size, dependencies included, for the specified <paramref name="key"/>.
        /// Cached assets require no download and thus their download size will be 0.  The Result of the operation
        /// is the download size in bytes.
        /// </summary>
        /// <returns>The operation handle for the request.</returns>
        /// <param name="key">The key of the asset(s) to get the download size of.</param>
        public static AsyncOperationHandle<long> GetDownloadSizeAsync(object key)
        {
            return impl.GetDownloadSizeAsync(key);
        }
        /// <summary>
        /// Determines the required download size, dependencies included, for the specified <paramref name="key"/>.
        /// Cached assets require no download and thus their download size will be 0.  The Result of the operation
        /// is the download size in bytes.
        /// </summary>
        /// <returns>The operation handle for the request.</returns>
        /// <param name="key">The key of the asset(s) to get the download size of.</param>
        public static AsyncOperationHandle<long> GetDownloadSizeAsync(string key)
        {
            return impl.GetDownloadSizeAsync((object)key);
        }
        
        /// <summary>
        /// Determines the required download size, dependencies included, for the specified <paramref name="keys"/>.
        /// Cached assets require no download and thus their download size will be 0.  The Result of the operation
        /// is the download size in bytes.
        /// </summary>
        /// <returns>The operation handle for the request.</returns>
        /// <param name="keys">The keys of the asset(s) to get the download size of.</param>
        public static AsyncOperationHandle<long> GetDownloadSizeAsync(IEnumerable keys)
        {
            return impl.GetDownloadSizeAsync(keys);
        }

        /// <summary>
        /// Downloads dependencies of assets marked with the specified label or address.
        /// See the [DownloadDependenciesAsync](xref:addressables-api-download-dependencies-async) documentation for more details.
        /// </summary>
        /// <param name="key">The key of the asset(s) to load dependencies for.</param>
        /// <param name="autoReleaseHandle">Automatically releases the handle on completion</param>
        /// <returns>The AsyncOperationHandle for the dependency load.</returns>
        public static AsyncOperationHandle DownloadDependenciesAsync(object key, bool autoReleaseHandle = false)
        {
            return impl.DownloadDependenciesAsync(key, autoReleaseHandle);
        }

        /// <summary>
        /// Downloads dependencies of assets at given locations.
        /// See the [DownloadDependenciesAsync](xref:addressables-api-download-dependencies-async) documentation for more details.
        /// </summary>
        /// <param name="locations">The locations of the assets.</param>
        /// <param name="autoReleaseHandle">Automatically releases the handle on completion</param>
        /// <returns>The AsyncOperationHandle for the dependency load.</returns>
        public static AsyncOperationHandle DownloadDependenciesAsync(IList<IResourceLocation> locations, bool autoReleaseHandle = false)
        {
            return impl.DownloadDependenciesAsync(locations, autoReleaseHandle);
        }
        
        /// <summary>
        /// Downloads dependencies of assets marked with the specified labels or addresses.
        /// See the [DownloadDependenciesAsync](xref:addressables-api-download-dependencies-async) documentation for more details.
        /// </summary>
        /// <param name="keys">List of keys for the locations.</param>
        /// <param name="mode">Method for merging the results of key matches.  See <see cref="MergeMode"/> for specifics</param>
        /// <param name="autoReleaseHandle">Automatically releases the handle on completion</param>
        /// <returns>The AsyncOperationHandle for the dependency load.</returns>
        public static AsyncOperationHandle DownloadDependenciesAsync(IEnumerable keys, MergeMode mode, bool autoReleaseHandle = false)
        {
            return impl.DownloadDependenciesAsync(keys, mode, autoReleaseHandle);
        }

        /// <summary>
        /// Clear the cached AssetBundles for a given key.  Operation may be performed async if Addressables
        /// is initializing or updating.
        /// </summary>
        /// <param name="key">The key to clear the cache for.</param>
        public static void ClearDependencyCacheAsync(object key)
        {
            impl.ClearDependencyCacheAsync(key, true);
        }

        /// <summary>
        /// Clear the cached AssetBundles for a list of Addressable locations.  Operation may be performed async if Addressables
        /// is initializing or updating.
        /// </summary>
        /// <param name="locations">The locations to clear the cache for.</param>
        public static void ClearDependencyCacheAsync(IList<IResourceLocation> locations)
        {
            impl.ClearDependencyCacheAsync(locations, true);
        }
        
        /// <summary>
        /// Clear the cached AssetBundles for a list of Addressable keys.  Operation may be performed async if Addressables
        /// is initializing or updating.
        /// </summary>
        /// <param name="keys">The keys to clear the cache for.</param>
        public static void ClearDependencyCacheAsync(IEnumerable keys)
        {
            impl.ClearDependencyCacheAsync(keys, true);
        }
        
        /// <summary>
        /// Clear the cached AssetBundles for a list of Addressable keys.  Operation may be performed async if Addressables
        /// is initializing or updating.
        /// </summary>
        /// <param name="keys">The key to clear the cache for.</param>
        public static void ClearDependencyCacheAsync(string key)
        {
            impl.ClearDependencyCacheAsync((object)key, true);
        }

        /// <summary>
        /// Clear the cached AssetBundles for a given key.  Operation may be performed async if Addressables
        /// is initializing or updating.
        /// </summary>
        /// <param name="key">The key to clear the cache for.</param>
        /// <param name="autoReleaseHandle">If true, the returned AsyncOperationHandle will be released on completion.</param>
        /// <returns>The operation handle for the request.</returns>
        public static AsyncOperationHandle<bool> ClearDependencyCacheAsync(object key, bool autoReleaseHandle)
        {
            return impl.ClearDependencyCacheAsync(key, autoReleaseHandle);
        }

        /// <summary>
        /// Clear the cached AssetBundles for a list of Addressable locations.  Operation may be performed async if Addressables
        /// is initializing or updating.
        /// </summary>
        /// <param name="locations">The locations to clear the cache for.</param>
        /// <param name="autoReleaseHandle">If true, the returned AsyncOperationHandle will be released on completion.</param>
        /// <returns>The operation handle for the request.</returns>
        public static AsyncOperationHandle<bool> ClearDependencyCacheAsync(IList<IResourceLocation> locations, bool autoReleaseHandle)
        {
            return impl.ClearDependencyCacheAsync(locations, autoReleaseHandle);
        }
        
        /// <summary>
        /// Clear the cached AssetBundles for a list of Addressable keys.  Operation may be performed async if Addressables
        /// is initializing or updating.
        /// </summary>
        /// <param name="keys">The keys to clear the cache for.</param>
        /// <param name="autoReleaseHandle">If true, the returned AsyncOperationHandle will be released on completion.</param>
        /// <returns>The operation handle for the request.</returns>
        public static AsyncOperationHandle<bool> ClearDependencyCacheAsync(IEnumerable keys, bool autoReleaseHandle)
        {
            return impl.ClearDependencyCacheAsync(keys, autoReleaseHandle);
        }

        /// <summary>
        /// Clear the cached AssetBundles for a list of Addressable keys.  Operation may be performed async if Addressables
        /// is initializing or updating.
        /// </summary>
        /// <param name="keys">The keys to clear the cache for.</param>
        /// <param name="autoReleaseHandle">If true, the returned AsyncOperationHandle will be released on completion.</param>
        /// <returns>The operation handle for the request.</returns>
        public static AsyncOperationHandle<bool> ClearDependencyCacheAsync(string key, bool autoReleaseHandle)
        {
            return impl.ClearDependencyCacheAsync((object)key, autoReleaseHandle);
        }
        
        /// <summary>
        /// Instantiate a single object. Note that the dependency loading is done asynchronously, but generally the actual instantiate is synchronous.
        /// See the [InstantiateAsync](xref:addressables-api-instantiate-async) documentation for more details.
        /// </summary>
        /// <param name="location">The location of the Object to instantiate.</param>
        /// <param name="parent">Parent transform for instantiated object.</param>
        /// <param name="instantiateInWorldSpace">Option to retain world space when instantiated with a parent.</param>
        /// <param name="trackHandle">If true, Addressables will track this request to allow it to be released via the result object.</param>
        /// <returns>The operation handle for the request.</returns>
        public static AsyncOperationHandle<GameObject> InstantiateAsync(IResourceLocation location, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true)
        {
            return impl.InstantiateAsync(location, new InstantiationParameters(parent, instantiateInWorldSpace), trackHandle);
        }

        /// <summary>
        /// Instantiate a single object. Note that the dependency loading is done asynchronously, but generally the actual instantiate is synchronous.
        /// See the [InstantiateAsync](xref:addressables-api-instantiate-async) documentation for more details.
        /// </summary>
        /// <param name="location">The location of the Object to instantiate.</param>
        /// <param name="position">The position of the instantiated object.</param>
        /// <param name="rotation">The rotation of the instantiated object.</param>
        /// <param name="parent">Parent transform for instantiated object.</param>
        /// <param name="trackHandle">If true, Addressables will track this request to allow it to be released via the result object.</param>
        /// <returns>The operation handle for the request.</returns>
        public static AsyncOperationHandle<GameObject> InstantiateAsync(IResourceLocation location, Vector3 position, Quaternion rotation, Transform parent = null, bool trackHandle = true)
        {
            return impl.InstantiateAsync(location, position, rotation, parent, trackHandle);
        }

        /// <summary>
        /// Instantiate a single object. Note that the dependency loading is done asynchronously, but generally the actual instantiate is synchronous.
        /// See the [InstantiateAsync](xref:addressables-api-instantiate-async) documentation for more details.
        /// </summary>
        /// <param name="key">The key of the location of the Object to instantiate.</param>
        /// <param name="parent">Parent transform for instantiated object.</param>
        /// <param name="instantiateInWorldSpace">Option to retain world space when instantiated with a parent.</param>
        /// <param name="trackHandle">If true, Addressables will track this request to allow it to be released via the result object.</param>
        /// <returns>The operation handle for the request.</returns>
        public static AsyncOperationHandle<GameObject> InstantiateAsync(object key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true)
        {
            return impl.InstantiateAsync(key, parent, instantiateInWorldSpace, trackHandle);
        }

        /// <summary>
        /// Instantiate a single object. Note that the dependency loading is done asynchronously, but generally the actual instantiate is synchronous.
        /// See the [InstantiateAsync](xref:addressables-api-instantiate-async) documentation for more details.
        /// </summary>
        /// <param name="key">The key of the location of the Object to instantiate.</param>
        /// <param name="position">The position of the instantiated object.</param>
        /// <param name="rotation">The rotation of the instantiated object.</param>
        /// <param name="parent">Parent transform for instantiated object.</param>
        /// <param name="trackHandle">If true, Addressables will track this request to allow it to be released via the result object.</param>
        /// <returns>The operation handle for the request.</returns>
        public static AsyncOperationHandle<GameObject> InstantiateAsync(object key, Vector3 position, Quaternion rotation, Transform parent = null, bool trackHandle = true)
        {
            return impl.InstantiateAsync(key, position, rotation, parent, trackHandle);
        }

        /// <summary>
        /// Instantiate a single object. Note that the dependency loading is done asynchronously, but generally the actual instantiate is synchronous.
        /// See the [InstantiateAsync](xref:addressables-api-instantiate-async) documentation for more details.
        /// </summary>
        /// <param name="key">The key of the location of the Object to instantiate.</param>
        /// <param name="instantiateParameters">Parameters for instantiation.</param>
        /// <param name="trackHandle">If true, Addressables will track this request to allow it to be released via the result object.</param>
        /// <returns>The operation handle for the request.</returns>
        public static AsyncOperationHandle<GameObject> InstantiateAsync(object key, InstantiationParameters instantiateParameters, bool trackHandle = true)
        {
            return impl.InstantiateAsync(key, instantiateParameters, trackHandle);
        }

        /// <summary>
        /// Instantiate a single object. Note that the dependency loading is done asynchronously, but generally the actual instantiate is synchronous.
        /// See the [InstantiateAsync](xref:addressables-api-instantiate-async) documentation for more details.
        /// </summary>
        /// <param name="location">The location of the Object to instantiate.</param>
        /// <param name="instantiateParameters">Parameters for instantiation.</param>
        /// <param name="trackHandle">If true, Addressables will track this request to allow it to be released via the result object.</param>
        /// <returns>The operation handle for the request.</returns>
        public static AsyncOperationHandle<GameObject> InstantiateAsync(IResourceLocation location, InstantiationParameters instantiateParameters, bool trackHandle = true)
        {
            return impl.InstantiateAsync(location, instantiateParameters, trackHandle);
        }

        /// <summary>
        /// Load scene.
        /// See the [LoadSceneAsync](xref:addressables-api-load-scene-async) documentation for more details.
        /// </summary>
        /// <param name="key">The key of the location of the scene to load.</param>
        /// <param name="loadMode">Scene load mode.</param>
        /// <param name="activateOnLoad">If false, the scene will load but not activate (for background loading).  The SceneInstance returned has an Activate() method that can be called to do this at a later point.</param>
        /// <param name="priority">Async operation priority for scene loading.</param>
        /// <returns>The operation handle for the request.</returns>
        public static AsyncOperationHandle<SceneInstance> LoadSceneAsync(object key, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100)
        {
            return impl.LoadSceneAsync(key, loadMode, activateOnLoad, priority);
        }

        /// <summary>
        /// Load scene.
        /// See the [LoadSceneAsync](xref:addressables-api-load-scene-async) documentation for more details.
        /// </summary>
        /// <param name="location">The location of the scene to load.</param>
        /// <param name="loadMode">Scene load mode.</param>
        /// <param name="activateOnLoad">If false, the scene will load but not activate (for background loading).  The SceneInstance returned has an Activate() method that can be called to do this at a later point.</param>
        /// <param name="priority">Async operation priority for scene loading.</param>
        /// <returns>The operation handle for the request.</returns>
        public static AsyncOperationHandle<SceneInstance> LoadSceneAsync(IResourceLocation location, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100)
        {
            return impl.LoadSceneAsync(location, loadMode, activateOnLoad, priority);
        }
        
        /// <summary>
        /// Release scene
        /// </summary>
        /// <param name="scene">The SceneInstance to release.</param>
        /// <param name="autoReleaseHandle">If true, the handle will be released automatically when complete.</param>
        /// <returns>The operation handle for the request.</returns>
        public static AsyncOperationHandle<SceneInstance> UnloadSceneAsync(SceneInstance scene, bool autoReleaseHandle = true)
        {
            return impl.UnloadSceneAsync(scene, autoReleaseHandle);
        }

        /// <summary>
        /// Release scene
        /// </summary>
        /// <param name="handle">The handle returned by LoadSceneAsync for the scene to release.</param>
        /// <param name="autoReleaseHandle">If true, the handle will be released automatically when complete.</param>
        /// <returns>The operation handle for the request.</returns>
        public static AsyncOperationHandle<SceneInstance> UnloadSceneAsync(AsyncOperationHandle handle, bool autoReleaseHandle = true)
        {
            return impl.UnloadSceneAsync(handle, autoReleaseHandle);
        }

        /// <summary>
        /// Release scene
        /// </summary>
        /// <param name="handle">The handle returned by LoadSceneAsync for the scene to release.</param>
        /// <param name="autoReleaseHandle">If true, the handle will be released automatically when complete.</param>
        /// <returns>The operation handle for the request.</returns>
        public static AsyncOperationHandle<SceneInstance> UnloadSceneAsync(AsyncOperationHandle<SceneInstance> handle, bool autoReleaseHandle = true)
        {
            return impl.UnloadSceneAsync(handle, autoReleaseHandle);
        }

        /// <summary>
        /// Checks all updatable content catalogs for a new version.
        /// </summary>
        /// <param name="autoReleaseHandle">If true, the handle will automatically be released when the operation completes.</param>
        /// <returns>The operation containing the list of catalog ids that have an available update.  This can be used to filter which catalogs to update with the UpdateContent.</returns>
        public static AsyncOperationHandle<List<string>> CheckForCatalogUpdates(bool autoReleaseHandle = true)
        {
            return impl.CheckForCatalogUpdates(autoReleaseHandle);
        }

        /// <summary>
        /// Update the specified catalogs.
        /// See the [UpdateCatalogs](xref:addressables-api-update-catalogs) documentation for more details.
        /// </summary>
        /// <param name="catalogs">The set of catalogs to update.  If null, all catalogs that have an available update will be updated.</param>
        /// <param name="autoReleaseHandle">If true, the handle will automatically be released when the operation completes.</param>
        /// <returns>The operation with the list of updated content catalog data.</returns>
        public static AsyncOperationHandle<List<IResourceLocator>> UpdateCatalogs(IEnumerable<string> catalogs = null, bool autoReleaseHandle = true)
        {
            return impl.UpdateCatalogs(catalogs, autoReleaseHandle);
        }

        /// <summary>
        /// Add a resource locator.
        /// </summary>
        /// <param name="locator">The locator object.</param>
        /// <param name="localCatalogHash">The hash of the local catalog. This can be null if the catalog cannot be updated.</param>
        /// <param name="remoteCatalogLocation">The location of the remote catalog. This can be null if the catalog cannot be updated.</param>
        public static void AddResourceLocator(IResourceLocator locator, string localCatalogHash = null, IResourceLocation remoteCatalogLocation = null)
        {
            impl.AddResourceLocator(locator, localCatalogHash, remoteCatalogLocation);
        }

        /// <summary>
        /// Remove a locator;
        /// </summary>
        /// <param name="locator">The locator to remove.</param>
        public static void RemoveResourceLocator(IResourceLocator locator)
        {
            impl.RemoveResourceLocator(locator);
        }

        /// <summary>
        /// Remove all locators.
        /// </summary>
        public static void ClearResourceLocators()
        {
            impl.ClearResourceLocators();
        }
    }
}
