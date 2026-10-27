using System.Collections.Generic;
using Code.Gameplay.Common.Random;
using Code.Gameplay.Common.Time;
using Code.Gameplay.Features.Animals.Configs;
using Code.Gameplay.Features.Animals.Data;
using Code.Gameplay.Features.Animals.Services;
using Code.Gameplay.Features.WorldBounds.Data;
using Code.Gameplay.Features.WorldBounds.Services;
using Code.Infrastructure.ConfigsManagement;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Features.Animals.Behaviours
{
	public class AnimalSpawner : MonoBehaviour
	{
		[SerializeField] private List<AnimalSpawnEntry> animalsToSpawn = new();

		[SerializeField, Min(0.1f)]
		private float minSpawnInterval = 1f;

		[SerializeField, Min(0.1f)]
		private float maxSpawnInterval = 2f;

		[SerializeField, Min(1)]
		[Tooltip("Caps the physics cost if predators stop keeping the population down.")]
		private int maxAliveAnimals = 60;

		private float _timeUntilNextSpawn;

		private IConfigsService _configsService;
		private IRandomService _randomService;
		private ITimeService _timeService;
		private IWorldBoundsService _worldBoundsService;
		private IAnimalFactory _animalFactory;
		private IAnimalRegistry _animalRegistry;

		[Inject]
		private void Construct(
			IConfigsService configsService,
			IRandomService randomService,
			ITimeService timeService,
			IWorldBoundsService worldBoundsService,
			IAnimalFactory animalFactory,
			IAnimalRegistry animalRegistry)
		{
			_configsService = configsService;
			_randomService = randomService;
			_timeService = timeService;
			_worldBoundsService = worldBoundsService;
			_animalFactory = animalFactory;
			_animalRegistry = animalRegistry;
		}

		private void Update()
		{
			_timeUntilNextSpawn -= _timeService.DeltaTime;

			if (_timeUntilNextSpawn > 0f)
				return;

			_timeUntilNextSpawn = _randomService.Range(minSpawnInterval, maxSpawnInterval);

			if (_animalRegistry.GetAliveCount() >= maxAliveAnimals)
				return;

			SpawnRandomAnimal();
		}

		private void SpawnRandomAnimal()
		{
			AnimalSpawnEntry entry = _randomService.PickWeighted(animalsToSpawn, spawnEntry => spawnEntry.Weight);
			AnimalConfig config = _configsService.GetAnimalConfig(entry.Animal);

			_animalFactory.CreateAnimal(config, PickSpawnPoint(), PickDirection());
		}

		private Vector3 PickSpawnPoint()
		{
			WorldRect bounds = _worldBoundsService.GetBounds();

			return new Vector3(
				_randomService.Range(bounds.MinX, bounds.MaxX),
				0f,
				_randomService.Range(bounds.MinZ, bounds.MaxZ));
		}

		private Vector3 PickDirection()
		{
			Vector2 direction = _randomService.GetRandomDirection();

			return new Vector3(direction.x, 0f, direction.y);
		}
	}
}
