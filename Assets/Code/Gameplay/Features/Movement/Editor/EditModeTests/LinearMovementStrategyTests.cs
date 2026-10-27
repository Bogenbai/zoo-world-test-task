using Code.Gameplay.Features.Movement.Configs;
using Code.Gameplay.Features.Movement.Strategies;
using Code.Infrastructure;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

namespace Code.Gameplay.Features.Movement.Editor.EditModeTests
{
	[Category(TestCategories.Gameplay)]
	public class LinearMovementStrategyTests
	{
		private const float Speed = 2.5f;
		private const float Step = 0.02f;

		[Test]
		public void Velocity_ShouldMatchSpeedAlongDirection()
		{
			// Arrange
			LinearMovementStrategy strategy = new(new LinearMovementConfig(Speed));

			// Act
			Vector3 velocity = strategy.Tick(Vector3.right, Step);

			// Assert
			velocity.Should().Be(Vector3.right * Speed, "a linear mover keeps a constant speed along its direction");
		}

		[Test]
		public void Velocity_ShouldNotChangeOverTime()
		{
			// Arrange
			LinearMovementStrategy strategy = new(new LinearMovementConfig(Speed));

			// Act
			Vector3 first = strategy.Tick(Vector3.right, Step);
			Vector3 later = strategy.Tick(Vector3.right, Step);

			// Assert
			later.Should().Be(first, "a linear mover has no pauses or bursts");
		}
	}
}
