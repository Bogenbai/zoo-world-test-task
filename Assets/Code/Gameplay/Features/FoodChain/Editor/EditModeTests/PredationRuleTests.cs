using Code.Gameplay.Common.Random;
using Code.Gameplay.Features.Animals.Behaviours;
using Code.Gameplay.Features.Animals.Editor.EditModeTests;
using Code.Gameplay.Features.Animals.Services;
using Code.Gameplay.Features.FoodChain.Configs;
using Code.Gameplay.Features.FoodChain.Services;
using Code.Infrastructure;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Code.Gameplay.Features.FoodChain.Editor.EditModeTests
{
	[Category(TestCategories.Gameplay)]
	public class PredationRuleTests
	{
		private AnimalTestObjects _objects;
		private FoodChainRoleConfig _prey;
		private FoodChainRoleConfig _predator;

		private IRandomService _randomService;
		private IAnimalDeathService _animalDeathService;
		private PredationRule _rule;

		[SetUp]
		public void SetUp()
		{
			_objects = new AnimalTestObjects();
			_prey = _objects.CreateRole();
			_predator = _objects.CreateRole();
			_objects.SetDiet(_predator, _prey, _predator);

			_randomService = Substitute.For<IRandomService>();
			_animalDeathService = Substitute.For<IAnimalDeathService>();
			_rule = new PredationRule(_randomService, _animalDeathService);
		}

		[TearDown]
		public void TearDown()
		{
			_objects.Dispose();
		}

		[Test]
		public void TwoPrey_ShouldBeLeftToNextRule()
		{
			// Arrange
			Animal first = _objects.CreateAnimal(_prey);
			Animal second = _objects.CreateAnimal(_prey);

			// Act
			bool handled = _rule.TryHandle(first, second);

			// Assert
			handled.Should().BeFalse("prey eat nobody, so the bounce rule decides");
			_animalDeathService.DidNotReceive().Kill(Arg.Any<Animal>());
		}

		[Test]
		public void PredatorAndPrey_ShouldKillPrey([Values] bool predatorFirst)
		{
			// Arrange
			Animal predator = _objects.CreateAnimal(_predator);
			Animal prey = _objects.CreateAnimal(_prey);

			// Act
			bool handled = predatorFirst
				? _rule.TryHandle(predator, prey)
				: _rule.TryHandle(prey, predator);

			// Assert
			handled.Should().BeTrue("eating ends the collision");
			_animalDeathService.Received(1).Kill(prey);
			_animalDeathService.DidNotReceive().Kill(predator);
		}

		[Test]
		public void TwoPredators_ShouldLetCoinFlipPickVictim([Values] bool coinFlip)
		{
			// Arrange
			_randomService.Chance(Arg.Any<float>()).Returns(coinFlip);
			Animal first = _objects.CreateAnimal(_predator);
			Animal second = _objects.CreateAnimal(_predator);

			// Act
			_rule.TryHandle(first, second);

			// Assert
			Animal victim = coinFlip ? second : first;
			Animal survivor = coinFlip ? first : second;
			_animalDeathService.Received(1).Kill(victim);
			_animalDeathService.DidNotReceive().Kill(survivor);
		}

		[Test]
		public void TwoPredators_ShouldFlipFairCoin()
		{
			// Arrange
			Animal first = _objects.CreateAnimal(_predator);
			Animal second = _objects.CreateAnimal(_predator);

			// Act
			_rule.TryHandle(first, second);

			// Assert
			_randomService.Received(1).Chance(0.5f);
		}
	}
}
