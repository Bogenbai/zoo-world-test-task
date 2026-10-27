using Code.Gameplay.Features.Animals.Behaviours;
using Code.Gameplay.Features.Animals.Configs;
using Code.Gameplay.Features.Movement.Services;
using Code.Gameplay.Features.Movement.Strategies;
using Code.Infrastructure.Pooling;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Features.Animals.Services
{
	public class AnimalFactory : IAnimalFactory
	{
		private readonly Transform _root = new GameObject("Animals").transform;

		private readonly IInstantiator _instantiator;
		private readonly IPrefabPool _prefabPool;
		private readonly IAnimalRegistry _animalRegistry;
		private readonly IMovementStrategyFactory _movementStrategyFactory;

		public AnimalFactory(
			IInstantiator instantiator,
			IPrefabPool prefabPool,
			IAnimalRegistry animalRegistry,
			IMovementStrategyFactory movementStrategyFactory)
		{
			_instantiator = instantiator;
			_prefabPool = prefabPool;
			_animalRegistry = animalRegistry;
			_movementStrategyFactory = movementStrategyFactory;
		}

		public Animal CreateAnimal(AnimalConfig config, Vector3 at, Vector3 direction)
		{
			if (_prefabPool.TryGet(config.Prefab, out Animal animal) == false)
				animal = _instantiator.InstantiatePrefabForComponent<Animal>(config.Prefab, _root);

			animal.Setup(config);
			IMovementStrategy movementStrategy = _movementStrategyFactory.CreateStrategy(config.Movement);
			animal.Movement.Setup(movementStrategy, direction);
			animal.gameObject.SetActive(true);
			animal.Movement.Teleport(at);
			_animalRegistry.Add(animal);

			return animal;
		}
	}
}
