using Code.Gameplay.Features.Animals.Configs;
using Code.Gameplay.Features.FoodChain.Configs;
using Code.Gameplay.Features.WorldBounds.Configs;
using Code.Infrastructure.AssetManagement;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Code.Infrastructure.ConfigsManagement
{
	public class ConfigsService : IConfigsService, IInitializable
	{
		private WorldConfig _worldConfig;

		private readonly IAssetsService _assetsService;

		public ConfigsService(IAssetsService assetsService)
		{
			_assetsService = assetsService;
		}

		public void Initialize()
		{
			_worldConfig = _assetsService.Load<WorldConfig>(Constants.Addressables.Keys.WorldConfig);
		}

		public AnimalConfig GetAnimalConfig(AssetReferenceT<AnimalConfig> reference) =>
			_assetsService.Load<AnimalConfig>(reference);

		public FoodChainRoleConfig GetFoodChainRole(AssetReferenceT<FoodChainRoleConfig> reference) =>
			_assetsService.Load<FoodChainRoleConfig>(reference);

		public WorldConfig GetWorldConfig() => _worldConfig;
	}
}
