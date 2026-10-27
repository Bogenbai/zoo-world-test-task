using System;
using Code.Gameplay.Features.Animals.Behaviours;
using Code.Gameplay.Features.Statistics.Services;
using Code.Infrastructure.Pooling;

namespace Code.Gameplay.Features.Animals.Services
{
	public class AnimalDeathService : IAnimalDeathService, IAnimalDeathQuery
	{
		private readonly IAnimalRegistry _animalRegistry;
		private readonly IDeathStatisticsService _deathStatisticsService;
		private readonly IPrefabPool _prefabPool;

		public event Action<Animal> OnAnimalDied;

		public AnimalDeathService(
			IAnimalRegistry animalRegistry,
			IDeathStatisticsService deathStatisticsService,
			IPrefabPool prefabPool)
		{
			_animalRegistry = animalRegistry;
			_deathStatisticsService = deathStatisticsService;
			_prefabPool = prefabPool;
		}

		public void Kill(Animal animal)
		{
			if (_animalRegistry.Remove(animal) == false)
				return;

			_deathStatisticsService.RegisterDeath(animal.Config.Role);
			OnAnimalDied?.Invoke(animal);
			_prefabPool.Put(animal.Config.Prefab.AssetGUID, animal);
		}
	}
}
