using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Gameplay.Common.Random
{
	public class UnityRandomService : IRandomService
	{
		public float Range(float minInclusive, float maxInclusive) =>
			UnityEngine.Random.Range(minInclusive, maxInclusive);

		public int Range(int minInclusive, int maxExclusive) =>
			UnityEngine.Random.Range(minInclusive, maxExclusive);

		public bool Chance(float chance) => UnityEngine.Random.value < chance;

		public Vector2 GetRandomDirection()
		{
			float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);

			return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
		}

		public T PickWeighted<T>(IReadOnlyList<T> items, Func<T, float> weightSelector)
		{
			float totalWeight = 0f;

			foreach (T item in items)
				totalWeight += Mathf.Max(0f, weightSelector(item));

			if (totalWeight <= 0f)
				throw new InvalidOperationException("There are no items with a positive weight to pick from.");

			float threshold = UnityEngine.Random.value * totalWeight;
			T lastPositiveItem = default;

			foreach (T item in items)
			{
				float weight = weightSelector(item);

				if (weight <= 0f)
					continue;

				lastPositiveItem = item;
				threshold -= weight;

				if (threshold < 0f)
					return item;
			}

			return lastPositiveItem;
		}
	}
}
