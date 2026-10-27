using System.Collections.Generic;
using Code.Gameplay.Features.Animals.Configs;
using Code.Gameplay.Features.WorldBounds.Configs;
using Code.Infrastructure.AssetManagement;
using Zenject;

namespace Code.Infrastructure.ConfigsManagement
{
	public class ConfigsService : IConfigsService, IInitializable
	{
		private IReadOnlyList<AnimalConfig> _animalConfigs;
		private WorldConfig _worldConfig;

		private readonly IAssetsService _assetsService;

		public ConfigsService(IAssetsService assetsService)
		{
			_assetsService = assetsService;
		}

		public void Initialize()
		{
			_animalConfigs = _assetsService.LoadAllByLabel<AnimalConfig>(Constants.Addressables.Labels.AnimalConfigs);
			_worldConfig = _assetsService.Load<WorldConfig>(Constants.Addressables.Keys.WorldConfig);
		}

		public IReadOnlyList<AnimalConfig> GetAnimalConfigs() => _animalConfigs;
		public WorldConfig GetWorldConfig() => _worldConfig;
	}
}
