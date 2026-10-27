using Code.Gameplay.Features.Animals.Editor.EditModeTests;
using Code.Gameplay.Features.FoodChain.Configs;
using Code.Gameplay.Features.Statistics.Services;
using Code.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace Code.Gameplay.Features.Statistics.Editor.EditModeTests
{
	[Category(TestCategories.Gameplay)]
	public class DeathStatisticsServiceTests
	{
		private AnimalTestObjects _objects;
		private FoodChainRoleConfig _prey;
		private FoodChainRoleConfig _predator;

		private DeathStatisticsService _service;

		private int _notificationCount;

		[SetUp]
		public void SetUp()
		{
			_objects = new AnimalTestObjects();
			_prey = _objects.CreateRole();
			_predator = _objects.CreateRole();

			_service = new DeathStatisticsService();
			_notificationCount = 0;
			_service.OnDeathsChanged += HandleDeathsChanged;
		}

		[TearDown]
		public void TearDown()
		{
			_service.OnDeathsChanged -= HandleDeathsChanged;
			_objects.Dispose();
		}

		[Test]
		public void NoDeaths_ShouldCountZero()
		{
			// Act
			int count = _service.GetDeathCount(_prey);

			// Assert
			count.Should().Be(0, "nothing has died yet");
		}

		[Test]
		public void RegisteredDeaths_ShouldBeCountedPerRole()
		{
			// Arrange
			_service.RegisterDeath(_prey);
			_service.RegisterDeath(_prey);
			_service.RegisterDeath(_predator);

			// Act
			int preyDeaths = _service.GetDeathCount(_prey);
			int predatorDeaths = _service.GetDeathCount(_predator);

			// Assert
			preyDeaths.Should().Be(2, "both prey deaths count for the prey role");
			predatorDeaths.Should().Be(1, "deaths of other roles do not mix in");
		}

		[Test]
		public void RegisteredDeath_ShouldNotifyListeners()
		{
			// Act
			_service.RegisterDeath(_prey);

			// Assert
			_notificationCount.Should().Be(1, "the counters on screen refresh on every death");
		}

		private void HandleDeathsChanged() => _notificationCount++;
	}
}
