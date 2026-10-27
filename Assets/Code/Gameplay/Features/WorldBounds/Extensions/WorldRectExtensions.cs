using Code.Gameplay.Features.WorldBounds.Data;
using UnityEngine;

namespace Code.Gameplay.Features.WorldBounds.Extensions
{
	public static class WorldRectExtensions
	{
		private const float MinInwardComponent = 0.3f;

		public static Vector3 TurnInside(this WorldRect rect, Vector3 position, Vector3 direction)
		{
			// Abs() makes the turn stable: an animal still outside on the next step does not flip back out
			if (position.x > rect.MaxX)
				direction.x = -Inward(direction.x);
			else if (position.x < rect.MinX)
				direction.x = Inward(direction.x);

			if (position.z > rect.MaxZ)
				direction.z = -Inward(direction.z);
			else if (position.z < rect.MinZ)
				direction.z = Inward(direction.z);

			return direction.normalized;
		}

		public static WorldRect Shrink(this WorldRect rect, float margin) =>
			new(rect.MinX + margin, rect.MaxX - margin, rect.MinZ + margin, rect.MaxZ - margin);

		// Moving along an edge would never bring the animal back, so the inward part has a lower bound
		private static float Inward(float component) => Mathf.Max(Mathf.Abs(component), MinInwardComponent);
	}
}
