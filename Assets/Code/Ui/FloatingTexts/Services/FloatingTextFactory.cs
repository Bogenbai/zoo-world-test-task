using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.Pooling;
using UnityEngine;
using Zenject;

namespace Code.Ui.FloatingTexts.Services
{
	public class FloatingTextFactory : IFloatingTextFactory
	{
		private readonly IInstantiator _instantiator;
		private readonly IAssetsService _assetsService;
		private readonly IPrefabPool _prefabPool;

		public FloatingTextFactory(IInstantiator instantiator, IAssetsService assetsService, IPrefabPool prefabPool)
		{
			_instantiator = instantiator;
			_assetsService = assetsService;
			_prefabPool = prefabPool;
		}

		public FloatingText CreateText(AssetReferencePrefab<FloatingText> prefab, Transform parent)
		{
			if (_prefabPool.TryGet(prefab.AssetGUID, out FloatingText text) == false)
				text = Instantiate(prefab, parent);

			text.transform.SetParent(parent, false);
			text.gameObject.SetActive(true);

			return text;
		}

		private FloatingText Instantiate(AssetReferencePrefab<FloatingText> prefab, Transform parent)
		{
			FloatingText loadedPrefab = _assetsService.LoadPrefab(prefab);

			return _instantiator.InstantiatePrefabForComponent<FloatingText>(loadedPrefab, parent);
		}
	}
}
