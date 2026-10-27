using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Code.Infrastructure.AssetManagement
{
	[Serializable]
	public class AssetReferencePrefab<TComponent> : AssetReferenceGameObject where TComponent : Component
	{
		public AssetReferencePrefab(string guid) : base(guid)
		{
		}

		public override bool ValidateAsset(Object obj) =>
			obj is GameObject prefab && prefab.GetComponent<TComponent>() != null;

		public override bool ValidateAsset(string path)
		{
#if UNITY_EDITOR
			GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

			return prefab != null && prefab.GetComponent<TComponent>() != null;
#else
			return true;
#endif
		}
	}
}
