using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Infrastructure.AssetManagement
{
	public interface IAssetsService
	{
		void CleanUp();
		TAsset Load<TAsset>(string key) where TAsset : Object;
		TAsset Load<TAsset>(AssetReference reference) where TAsset : Object;
		TComponent LoadPrefab<TComponent>(AssetReferencePrefab<TComponent> reference) where TComponent : Component;
		IReadOnlyList<TAsset> LoadAllByLabel<TAsset>(string label) where TAsset : Object;
	}
}
