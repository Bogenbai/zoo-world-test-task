using System.Collections.Generic;
using Code.Gameplay.Common.Random;
using Code.Gameplay.Features.Movement.Configs;
using Code.Gameplay.Features.Movement.Strategies;
using Code.Infrastructure;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

namespace Code.Gameplay.Features.Movement.Editor.EditModeTests
{
	[Category(TestCategories.Gameplay)]
	public class JumpMovementStrategyTests
	{
		private IRandomService _randomService;

		private const float JumpDistance = 2f;
		private const float JumpDuration = 0.25f;
		private const float JumpInterval = 1f;
		private const float MaxTurnAngle = 60f;
		private const float Tolerance = 0.0001f;
		private const float AngleTolerance = 0.01f;

		// A power-of-two step is exact in floating point, so tick counts below do not drift
		private const float Step = 1f / 32f;
		private const int TicksPerJump = 8;
		private const int TicksPerInterval = 32;

		[SetUp]
		public void SetUp()
		{
			_randomService = Substitute.For<IRandomService>();
		}

		[Test]
		public void FirstTick_ShouldStartJump()
		{
			// Arrange
			JumpMovementStrategy strategy = CreateStrategy();

			// Act
			Vector3 velocity = strategy.Tick(Vector3.forward, Step);

			// Assert
			velocity.magnitude.Should().BeGreaterThan(0f, "an animal starts moving as soon as it appears");
		}

		[TestCase(Step)]
		[TestCase(0.03f)]
		public void OneJump_ShouldCoverExactJumpDistance(float deltaTime)
		{
			// Arrange
			int tickCount = Mathf.FloorToInt(JumpInterval / deltaTime);

			// Act
			List<Vector3> velocities = Simulate(CreateStrategy(), deltaTime, tickCount);

			// Assert
			float distance = 0f;

			foreach (Vector3 velocity in velocities)
				distance += velocity.magnitude * deltaTime;

			distance.Should().BeApproximately(JumpDistance, Tolerance, "a jump covers a fixed distance");
		}

		[Test]
		public void BetweenJumps_ShouldNotMove()
		{
			// Act
			List<Vector3> velocities = Simulate(CreateStrategy(), Step, TicksPerInterval);

			// Assert
			List<Vector3> pause = velocities.GetRange(TicksPerJump, TicksPerInterval - TicksPerJump);
			pause.Should().OnlyContain(velocity => velocity == Vector3.zero, "the animal rests until the next jump");
		}

		[Test]
		public void NextJump_ShouldStartAfterInterval()
		{
			// Act
			List<Vector3> velocities = Simulate(CreateStrategy(), Step, TicksPerInterval + 1);

			// Assert
			velocities[TicksPerInterval - 1].Should().Be(Vector3.zero, "the interval has not passed yet");
			velocities[TicksPerInterval].magnitude.Should().BeGreaterThan(0f, "a new jump starts every interval");
		}

		[Test]
		public void Jump_ShouldTurnByRandomAngle()
		{
			// Arrange
			_randomService.Range(Arg.Any<float>(), Arg.Any<float>()).Returns(90f);
			JumpMovementStrategy strategy = CreateStrategy();

			// Act
			Vector3 velocity = strategy.Tick(Vector3.forward, Step);

			// Assert
			float turn = Vector3.Angle(Vector3.forward, velocity);
			turn.Should().BeApproximately(90f, AngleTolerance, "the random angle is applied at the start of the jump");
		}

		[Test]
		public void TurnAngle_ShouldBeDrawnWithinMaxTurnAngle()
		{
			// Arrange
			JumpMovementStrategy strategy = CreateStrategy();

			// Act
			strategy.Tick(Vector3.forward, Step);

			// Assert
			_randomService.Received(1).Range(-MaxTurnAngle, MaxTurnAngle);
		}

		[Test]
		public void ZeroDeltaTime_ShouldNotMove()
		{
			// Arrange
			JumpMovementStrategy strategy = CreateStrategy();

			// Act
			Vector3 velocity = strategy.Tick(Vector3.forward, 0f);

			// Assert
			velocity.Should().Be(Vector3.zero, "no time has passed");
		}

		private JumpMovementStrategy CreateStrategy()
		{
			JumpMovementConfig config = new(JumpDistance, JumpDuration, JumpInterval, MaxTurnAngle);

			return new JumpMovementStrategy(config, _randomService);
		}

		private static List<Vector3> Simulate(IMovementStrategy strategy, float deltaTime, int tickCount)
		{
			List<Vector3> velocities = new();
			Vector3 direction = Vector3.forward;

			for (int i = 0; i < tickCount; i++)
			{
				Vector3 velocity = strategy.Tick(direction, deltaTime);

				if (velocity != Vector3.zero)
					direction = velocity.normalized;

				velocities.Add(velocity);
			}

			return velocities;
		}
	}
}
