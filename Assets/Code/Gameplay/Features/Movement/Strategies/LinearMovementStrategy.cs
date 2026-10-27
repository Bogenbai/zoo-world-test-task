using Code.Gameplay.Features.Movement.Configs;
using UnityEngine;

namespace Code.Gameplay.Features.Movement.Strategies
{
	public class LinearMovementStrategy : IMovementStrategy
	{
		private readonly LinearMovementConfig _config;

		public LinearMovementStrategy(LinearMovementConfig config)
		{
			_config = config;
		}

		public Vector3 Tick(Vector3 direction, float deltaTime)
		{
			return direction * _config.Speed;
		}
	}
}
