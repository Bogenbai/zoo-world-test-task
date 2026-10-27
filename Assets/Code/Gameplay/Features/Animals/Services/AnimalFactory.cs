using Code.Gameplay.Features.Animals.Behaviours;
using Code.Gameplay.Features.Animals.Configs;
using Code.Gameplay.Features.Movement.Services;
using Code.Gameplay.Features.Movement.Strategies;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.Pooling;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Features.Animals.Services
{
	public class AnimalFactory : IAnimalFactory
	{
		private readonly Transform _root = new GameObject("Animals").transform;

		private readonly IInstantiator _instantiator;
		private readonly IAssetsService _assetsService;
		private readonly IPrefabPool _prefabPool;
		private readonly IAnimalRegistry _animalRegistry;
		private readonly IMovementStrategyFactory _movementStrategyFactory;

		public AnimalFactory(
			IInstantiator instantiator,
			IAssetsService assetsService,
			IPrefabPool prefabPool,
			IAnimalRegistry animalRegistry,
			IMovementStrategyFactory movementStrategyFactory)
		{
			_instantiator = instantiator;
			_assetsService = assetsService;
			_prefabPool = prefabPool;
			_animalRegistry = animalRegistry;
			_movementStrategyFactory = movementStrategyFactory;
		}

		public Animal CreateAnimal(AnimalConfig config, Vector3 at, Vector3 direction)
		{
			if (_prefabPool.TryGet(config.Prefab.AssetGUID, out Animal animal) == false)
				animal = Instantiate(config);

			animal.Setup(config);
			IMovementStrategy movementStrategy = _movementStrategyFactory.CreateStrategy(config.Movement);
			animal.Movement.Setup(movementStrategy, direction);
			animal.gameObject.SetActive(true);
			animal.Movement.Teleport(at);
			_animalRegistry.Add(animal);

			return animal;
		}

		private Animal Instantiate(AnimalConfig config)
		{
			Animal prefab = _assetsService.LoadPrefab(config.Prefab);

			return _instantiator.InstantiatePrefabForComponent<Animal>(prefab, _root);
		}
	}
}
