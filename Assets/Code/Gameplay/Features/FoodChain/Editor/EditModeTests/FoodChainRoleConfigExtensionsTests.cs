using Code.Gameplay.Features.Animals.Editor.EditModeTests;
using Code.Gameplay.Features.FoodChain.Configs;
using Code.Gameplay.Features.FoodChain.Extensions;
using Code.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace Code.Gameplay.Features.FoodChain.Editor.EditModeTests
{
	[Category(TestCategories.Gameplay)]
	public class FoodChainRoleConfigExtensionsTests
	{
		private AnimalTestObjects _objects;

		[SetUp]
		public void SetUp()
		{
			_objects = new AnimalTestObjects();
		}

		[TearDown]
		public void TearDown()
		{
			_objects.Dispose();
		}

		[Test]
		public void RoleInDiet_ShouldBeEaten()
		{
			// Arrange
			FoodChainRoleConfig eater = _objects.CreateRole();
			FoodChainRoleConfig food = _objects.CreateRole();
			_objects.SetDiet(eater, food);

			// Act
			bool eats = eater.Eats(food);

			// Assert
			eats.Should().BeTrue("the role is listed in the eater's diet");
		}

		[Test]
		public void RoleNotInDiet_ShouldNotBeEaten()
		{
			// Arrange
			FoodChainRoleConfig eater = _objects.CreateRole();
			_objects.SetDiet(eater, _objects.CreateRole());

			// Act
			bool eats = eater.Eats(_objects.CreateRole());

			// Assert
			eats.Should().BeFalse("only roles listed in the diet are food");
		}

		[Test]
		public void RoleInOwnDiet_ShouldEatItsKind()
		{
			// Arrange
			FoodChainRoleConfig predator = _objects.CreateRole();
			_objects.SetDiet(predator, predator);

			// Act
			bool eats = predator.Eats(predator);

			// Assert
			eats.Should().BeTrue("a role may list itself, like predators eating predators");
		}
	}
}
