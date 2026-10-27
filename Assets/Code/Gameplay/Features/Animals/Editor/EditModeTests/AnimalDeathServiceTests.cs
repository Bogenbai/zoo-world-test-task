using Code.Gameplay.Features.Animals.Behaviours;
using Code.Gameplay.Features.Animals.Services;
using Code.Infrastructure;
using Code.Infrastructure.Pooling;
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
		private IPrefabPool _prefabPool;
		private AnimalDeathService _service;

		[SetUp]
		public void SetUp()
		{
			_objects = new AnimalTestObjects();
			_animal = _objects.CreateAnimal(_objects.CreateRole());

			_animalRegistry = Substitute.For<IAnimalRegistry>();
			_prefabPool = Substitute.For<IPrefabPool>();
			_service = new AnimalDeathService(_animalRegistry, _prefabPool);
		}

		[TearDown]
		public void TearDown()
		{
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
		public void AlreadyRemovedAnimal_ShouldNotGoToPoolAgain()
		{
			// Arrange
			_animalRegistry.Remove(_animal).Returns(false);

			// Act
			_service.Kill(_animal);

			// Assert
			_prefabPool.DidNotReceive().Put(Arg.Any<string>(), Arg.Any<Component>());
		}
	}
}
