using UnityEngine;

namespace Code.Gameplay.Features.Movement.Strategies
{
	public interface IMovementStrategy
	{
		// The caller owns the direction and takes the direction of a non-zero result as the new one
		Vector3 Tick(Vector3 direction, float deltaTime);
	}
}
