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
			string[] items = { "never", "always" };

			HashSet<string> picked = PickMany(items, item => item == "never" ? 0f : 1f);

			picked.Should().NotContain("never", "an item with zero weight has no share of the total weight");
		}

		[Test]
		public void NegativeWeightItem_ShouldNeverBePicked()
		{
			string[] items = { "never", "always" };

			HashSet<string> picked = PickMany(items, item => item == "never" ? -5f : 1f);

			picked.Should().NotContain("never", "a negative weight is treated as zero");
		}

		[Test]
		public void SinglePositiveWeight_ShouldAlwaysBePicked()
		{
			string[] items = { "zero", "only", "alsoZero" };

			HashSet<string> picked = PickMany(items, item => item == "only" ? 2f : 0f);

			picked.Should().BeEquivalentTo(new[] { "only" }, "it is the only item that has any weight");
		}

		[Test]
		public void EveryPositiveWeight_ShouldEventuallyBePicked()
		{
			string[] items = { "first", "second", "third" };

			HashSet<string> picked = PickMany(items, item => 1f);

			picked.Should().BeEquivalentTo(items, "equal weights give every item the same chance");
		}

		[Test]
		public void NoPositiveWeights_ShouldThrow()
		{
			string[] items = { "first", "second" };

			Action pick = () => _randomService.PickWeighted(items, item => 0f);

			pick.Should().Throw<InvalidOperationException>("there is nothing to pick from");
		}

		[Test]
		public void RandomDirection_ShouldHaveUnitLength()
		{
			for (int i = 0; i < PickCount; i++)
			{
				Vector2 direction = _randomService.GetRandomDirection();

				direction.magnitude.Should().BeApproximately(1f, 0.0001f, "a direction carries no speed");
			}
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
