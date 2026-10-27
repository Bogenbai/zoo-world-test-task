using Code.Gameplay.Features.Animals.Behaviours;
using Code.Gameplay.Features.Animals.Services;
using Code.Gameplay.Features.FoodChain.Configs;
using Code.Gameplay.Features.Statistics.Services;
using Code.Infrastructure;
using Code.Infrastructure.Pooling;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

namespace Code.Gameplay.Features.Animals.Editor.EditModeTests
{
	[Category(TestCategories.Gameplay)]
	public class AnimalDeathServiceTests
	{
		private AnimalTestObjects _objects;
		private Animal _animal;

		private IAnimalRegistry _animalRegistry;
		private IDeathStatisticsService _deathStatisticsService;
		private IPrefabPool _prefabPool;
		private AnimalDeathService _service;

		private Animal _reportedAnimal;

		[SetUp]
		public void SetUp()
		{
			_objects = new AnimalTestObjects();
			_animal = _objects.CreateAnimal(_objects.CreateRole());

			_animalRegistry = Substitute.For<IAnimalRegistry>();
			_deathStatisticsService = Substitute.For<IDeathStatisticsService>();
			_prefabPool = Substitute.For<IPrefabPool>();
			_service = new AnimalDeathService(_animalRegistry, _deathStatisticsService, _prefabPool);

			_reportedAnimal = null;
			_service.OnAnimalDied += HandleAnimalDied;
		}

		[TearDown]
		public void TearDown()
		{
			_service.OnAnimalDied -= HandleAnimalDied;
			_objects.Dispose();
		}

		[Test]
		public void KilledAnimal_ShouldLeaveRegistryAndGoToPool()
		{
			// Arrange
			_animalRegistry.Remove(_animal).Returns(true);

			// Act
			_service.Kill(_animal);

			// Assert
			_animalRegistry.Received(1).Remove(_animal);
			_prefabPool.Received(1).Put(_animal.Config.Prefab.AssetGUID, _animal);
		}

		[Test]
		public void KilledAnimal_ShouldCountForItsRole()
		{
			// Arrange
			_animalRegistry.Remove(_animal).Returns(true);

			// Act
			_service.Kill(_animal);

			// Assert
			_deathStatisticsService.Received(1).RegisterDeath(_animal.Config.Role);
		}

		[Test]
		public void KilledAnimal_ShouldBeReported()
		{
			// Arrange
			_animalRegistry.Remove(_animal).Returns(true);

			// Act
			_service.Kill(_animal);

			// Assert
			_reportedAnimal.Should().BeSameAs(_animal, "listeners have to drop whatever they attached to it");
		}

		[Test]
		public void AlreadyRemovedAnimal_ShouldBeIgnored()
		{
			// Arrange
			_animalRegistry.Remove(_animal).Returns(false);

			// Act
			_service.Kill(_animal);

			// Assert
			_prefabPool.DidNotReceive().Put(Arg.Any<string>(), Arg.Any<Component>());
			_deathStatisticsService.DidNotReceive().RegisterDeath(Arg.Any<FoodChainRoleConfig>());
			_reportedAnimal.Should().BeNull("an animal dies only once");
		}

		private void HandleAnimalDied(Animal animal) => _reportedAnimal = animal;
	}
}
