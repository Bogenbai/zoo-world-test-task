using System.Collections.Generic;
using Code.Gameplay.Features.Animals.Behaviours;
using Code.Gameplay.Features.Animals.Editor.EditModeTests;
using Code.Gameplay.Features.Animals.Services;
using Code.Gameplay.Features.Collisions.Services;
using Code.Infrastructure;
using NSubstitute;
using NUnit.Framework;

namespace Code.Gameplay.Features.Collisions.Editor.EditModeTests
{
	[Category(TestCategories.Gameplay)]
	public class AnimalCollisionServiceTests
	{
		private AnimalTestObjects _objects;
		private Animal _animal;
		private Animal _other;

		private IAnimalRegistry _animalRegistry;
		private ICollisionRule _firstRule;
		private ICollisionRule _secondRule;
		private AnimalCollisionService _service;

		[SetUp]
		public void SetUp()
		{
			_objects = new AnimalTestObjects();
			_animal = _objects.CreateAnimal(_objects.CreateRole());
			_other = _objects.CreateAnimal(_objects.CreateRole());

			_animalRegistry = Substitute.For<IAnimalRegistry>();
			_animalRegistry.Contains(Arg.Any<Animal>()).Returns(true);
			_firstRule = Substitute.For<ICollisionRule>();
			_secondRule = Substitute.For<ICollisionRule>();
			List<ICollisionRule> rules = new() { _firstRule, _secondRule };
			_service = new AnimalCollisionService(_animalRegistry, rules);
		}

		[TearDown]
		public void TearDown()
		{
			_objects.Dispose();
		}

		[Test]
		public void Contact_ShouldBeHandledOnce()
		{
			// Act
			ReportFromBothBodies();

			// Assert
			_firstRule.Received(1).TryHandle(Arg.Any<Animal>(), Arg.Any<Animal>());
		}

		[Test]
		public void RemovedAnimal_ShouldBeSkipped()
		{
			// Arrange
			_animalRegistry.Contains(_other).Returns(false);

			// Act
			ReportFromBothBodies();

			// Assert
			_firstRule.DidNotReceive().TryHandle(Arg.Any<Animal>(), Arg.Any<Animal>());
		}

		[Test]
		public void HandledContact_ShouldStopPipeline()
		{
			// Arrange
			_firstRule.TryHandle(Arg.Any<Animal>(), Arg.Any<Animal>()).Returns(true);

			// Act
			ReportFromBothBodies();

			// Assert
			_secondRule.DidNotReceive().TryHandle(Arg.Any<Animal>(), Arg.Any<Animal>());
		}

		[Test]
		public void UnhandledContact_ShouldReachNextRule()
		{
			// Arrange
			_firstRule.TryHandle(Arg.Any<Animal>(), Arg.Any<Animal>()).Returns(false);

			// Act
			ReportFromBothBodies();

			// Assert
			_secondRule.Received(1).TryHandle(Arg.Any<Animal>(), Arg.Any<Animal>());
		}

		private void ReportFromBothBodies()
		{
			_service.ProcessCollision(_animal, _other);
			_service.ProcessCollision(_other, _animal);
		}
	}
}
