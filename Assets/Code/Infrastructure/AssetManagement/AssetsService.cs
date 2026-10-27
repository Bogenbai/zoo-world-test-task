using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Code.Infrastructure.AssetManagement
{
	public class AssetsService : IAssetsService, IDisposable
	{
		private readonly Dictionary<string, AsyncOperationHandle> _assetHandles = new();
		private readonly Dictionary<string, AsyncOperationHandle> _labelHandles = new();

		public void Dispose()
		{
			CleanUp();
		}

		public void CleanUp()
		{
			ReleaseAll(_assetHandles);
			ReleaseAll(_labelHandles);
		}

		public TAsset Load<TAsset>(string key) where TAsset : Object
		{
			if (_assetHandles.TryGetValue(key, out AsyncOperationHandle cachedHandle))
				return (TAsset)cachedHandle.Result;

			AsyncOperationHandle<TAsset> handle = Addressables.LoadAssetAsync<TAsset>(key);
			handle.WaitForCompletion();
			EnsureSucceeded(handle, key);
			_assetHandles.Add(key, handle);

			return handle.Result;
		}

		public IReadOnlyList<TAsset> LoadAllByLabel<TAsset>(string label) where TAsset : Object
		{
			if (_labelHandles.TryGetValue(label, out AsyncOperationHandle cachedHandle))
				return new List<TAsset>((IList<TAsset>)cachedHandle.Result);

			AsyncOperationHandle<IList<TAsset>> handle = Addressables.LoadAssetsAsync<TAsset>(label, null);
			handle.WaitForCompletion();
			EnsureSucceeded(handle, label);
			_labelHandles.Add(label, handle);

			return new List<TAsset>(handle.Result);
		}

		private static void ReleaseAll(Dictionary<string, AsyncOperationHandle> handles)
		{
			foreach (AsyncOperationHandle handle in handles.Values)
				Addressables.Release(handle);

			handles.Clear();
		}

		private static void EnsureSucceeded<TResult>(AsyncOperationHandle<TResult> handle, string key)
		{
			if (handle.Status == AsyncOperationStatus.Succeeded)
				return;

			Exception reason = handle.OperationException;
			Addressables.Release(handle);

			throw new InvalidOperationException($"Failed to load addressable '{key}' as {typeof(TResult).Name}", reason);
		}
	}
}
