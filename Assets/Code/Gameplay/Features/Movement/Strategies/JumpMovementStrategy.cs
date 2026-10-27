using Code.Gameplay.Common.Random;
using Code.Gameplay.Features.Movement.Configs;
using UnityEngine;

namespace Code.Gameplay.Features.Movement.Strategies
{
	public class JumpMovementStrategy : IMovementStrategy
	{
		private float _timeUntilNextJump;
		private float _distanceLeft;

		private readonly JumpMovementConfig _config;
		private readonly IRandomService _randomService;

		public JumpMovementStrategy(JumpMovementConfig config, IRandomService randomService)
		{
			_config = config;
			_randomService = randomService;
		}

		public Vector3 Tick(Vector3 direction, float deltaTime)
		{
			if (deltaTime <= 0f)
				return Vector3.zero;

			Vector3 jumpDirection = direction;

			if (_timeUntilNextJump <= 0f)
			{
				_timeUntilNextJump += _config.JumpInterval;
				_distanceLeft = _config.JumpDistance;
				jumpDirection = Turn(direction);
			}

			_timeUntilNextJump -= deltaTime;

			if (_distanceLeft <= 0f)
				return Vector3.zero;

			float jumpSpeed = _config.JumpDistance / _config.JumpDuration;

			// Clamping the last step keeps the jump distance exact whatever the physics step is
			float step = Mathf.Min(jumpSpeed * deltaTime, _distanceLeft);
			_distanceLeft -= step;

			return jumpDirection * (step / deltaTime);
		}

		private Vector3 Turn(Vector3 direction)
		{
			float angle = _randomService.Range(-_config.MaxTurnAngle, _config.MaxTurnAngle);

			return Quaternion.AngleAxis(angle, Vector3.up) * direction;
		}
	}
}
