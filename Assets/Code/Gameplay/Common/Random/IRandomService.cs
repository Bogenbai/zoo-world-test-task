using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Gameplay.Common.Random
{
	public interface IRandomService
	{
		float Range(float minInclusive, float maxInclusive);
		int Range(int minInclusive, int maxExclusive);
		bool Chance(float chance);
		Vector2 GetRandomDirection();
		T PickWeighted<T>(IReadOnlyList<T> items, Func<T, float> weightSelector);
	}
}
