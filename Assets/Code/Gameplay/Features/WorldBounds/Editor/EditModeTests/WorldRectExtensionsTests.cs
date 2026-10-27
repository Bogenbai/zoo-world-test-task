using Code.Gameplay.Features.WorldBounds.Data;
using Code.Gameplay.Features.WorldBounds.Extensions;
using Code.Infrastructure;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

namespace Code.Gameplay.Features.WorldBounds.Editor.EditModeTests
{
	[Category(TestCategories.Gameplay)]
	public class WorldRectExtensionsTests
	{
		private readonly WorldRect _rect = new(-10f, 10f, -5f, 5f);

		private const float Tolerance = 0.0001f;

		[Test]
		public void InsideRect_ShouldKeepDirection()
		{
			// Arrange
			Vector3 direction = new Vector3(1f, 0f, 1f).normalized;

			// Act
			Vector3 result = _rect.TurnInside(Vector3.zero, direction);

			// Assert
			Vector3.Distance(result, direction).Should().BeLessThan(Tolerance, "there is no edge to turn away from");
		}

		[Test]
		public void PastRightEdge_ShouldTurnLeft()
		{
			// Act
			Vector3 result = _rect.TurnInside(new Vector3(12f, 0f, 0f), new Vector3(1f, 0f, 0.5f));

			// Assert
			result.x.Should().BeNegative("the animal left through the right edge and has to come back");
			result.z.Should().BePositive("only the outward component is turned");
		}

		[Test]
		public void PastTopEdge_ShouldTurnDown()
		{
			// Act
			Vector3 result = _rect.TurnInside(new Vector3(0f, 0f, 7f), new Vector3(0.5f, 0f, 1f));

			// Assert
			result.z.Should().BeNegative("the animal left through the top edge and has to come back");
			result.x.Should().BePositive("only the outward component is turned");
		}

		[Test]
		public void PastCorner_ShouldTurnOnBothAxes()
		{
			// Act
			Vector3 result = _rect.TurnInside(new Vector3(-12f, 0f, -7f), new Vector3(-1f, 0f, -1f));

			// Assert
			result.x.Should().BePositive("the animal left through the left edge");
			result.z.Should().BePositive("the animal also left through the bottom edge");
		}

		[Test]
		public void OutsideButMovingInward_ShouldKeepDirection()
		{
			// Arrange
			Vector3 direction = Vector3.left;

			// Act
			Vector3 result = _rect.TurnInside(new Vector3(12f, 0f, 0f), direction);

			// Assert
			Vector3.Distance(result, direction).Should().BeLessThan(Tolerance, "turning again would push it back out");
		}

		[Test]
		public void ParallelToEdgeOutside_ShouldGainInwardComponent()
		{
			// Act
			Vector3 result = _rect.TurnInside(new Vector3(12f, 0f, 0f), Vector3.forward);

			// Assert
			result.x.Should().BeNegative("sliding along the edge outside would never bring the animal back");
		}

		[Test]
		public void Shrink_ShouldMoveEveryEdgeInward()
		{
			// Act
			WorldRect shrunk = _rect.Shrink(1f);

			// Assert
			shrunk.MinX.Should().Be(-9f, "the left edge moves right by the margin");
			shrunk.MaxX.Should().Be(9f, "the right edge moves left by the margin");
			shrunk.MinZ.Should().Be(-4f, "the bottom edge moves up by the margin");
			shrunk.MaxZ.Should().Be(4f, "the top edge moves down by the margin");
		}

		[Test]
		public void TurnedDirection_ShouldBeNormalized()
		{
			// Act
			Vector3 result = _rect.TurnInside(new Vector3(12f, 0f, 0f), new Vector3(1f, 0f, 0.1f).normalized);

			// Assert
			result.magnitude.Should().BeApproximately(1f, Tolerance, "a direction carries no speed");
		}
	}
}
