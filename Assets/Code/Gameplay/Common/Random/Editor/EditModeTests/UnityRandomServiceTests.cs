using System;
using System.Collections.Generic;
using Code.Infrastructure;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

namespace Code.Gameplay.Common.Random.Editor.EditModeTests
{
	[Category(TestCategories.Gameplay)]
	public class UnityRandomServiceTests
	{
		private readonly UnityRandomService _randomService = new();

		private const int PickCount = 1000;

		[Test]
		public void ZeroWeightItem_ShouldNeverBePicked()
		{
			// Arrange
			string[] items = { "never", "always" };

			// Act
			HashSet<string> picked = PickMany(items, item => item == "never" ? 0f : 1f);

			// Assert
			picked.Should().NotContain("never", "an item with zero weight has no share of the total weight");
		}

		[Test]
		public void NegativeWeightItem_ShouldNeverBePicked()
		{
			// Arrange
			string[] items = { "never", "always" };

			// Act
			HashSet<string> picked = PickMany(items, item => item == "never" ? -5f : 1f);

			// Assert
			picked.Should().NotContain("never", "a negative weight is treated as zero");
		}

		[Test]
		public void SinglePositiveWeight_ShouldAlwaysBePicked()
		{
			// Arrange
			string[] items = { "zero", "only", "alsoZero" };

			// Act
			HashSet<string> picked = PickMany(items, item => item == "only" ? 2f : 0f);

			// Assert
			picked.Should().BeEquivalentTo(new[] { "only" }, "it is the only item that has any weight");
		}

		[Test]
		public void EveryPositiveWeight_ShouldEventuallyBePicked()
		{
			// Arrange
			string[] items = { "first", "second", "third" };

			// Act
			HashSet<string> picked = PickMany(items, item => 1f);

			// Assert
			picked.Should().BeEquivalentTo(items, "equal weights give every item the same chance");
		}

		[Test]
		public void NoPositiveWeights_ShouldThrow()
		{
			// Arrange
			string[] items = { "first", "second" };

			// Act
			Action pick = () => _randomService.PickWeighted(items, item => 0f);

			// Assert
			pick.Should().Throw<InvalidOperationException>("there is nothing to pick from");
		}

		[Test]
		public void RandomDirection_ShouldHaveUnitLength()
		{
			// Act
			List<Vector2> directions = new();

			for (int i = 0; i < PickCount; i++)
				directions.Add(_randomService.GetRandomDirection());

			// Assert
			directions.Should().OnlyContain(
				direction => Mathf.Abs(direction.magnitude - 1f) < 0.0001f,
				"a direction carries no speed");
		}

		private HashSet<string> PickMany(IReadOnlyList<string> items, Func<string, float> weightSelector)
		{
			HashSet<string> picked = new();

			for (int i = 0; i < PickCount; i++)
				picked.Add(_randomService.PickWeighted(items, weightSelector));

			return picked;
		}
	}
}
