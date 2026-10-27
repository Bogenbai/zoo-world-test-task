using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Code.Gameplay.Features.Movement.Configs
{
	[Serializable]
	public class JumpMovementConfig : MovementConfig
	{
		[SerializeField, Min(0f)]
		private float jumpDistance = 2f;

		[SerializeField, Min(0.01f)]
		private float jumpDuration = 0.2f;

		[SerializeField, Min(0.01f)]
		[Tooltip("Time between the starts of two jumps, so the rhythm does not depend on the jump duration.")]
		private float jumpInterval = 1f;

		[SerializeField, Range(0f, 180f)]
		[Tooltip("Every jump turns by a random angle up to this value, so the path never becomes a straight line.")]
		private float maxTurnAngle = 60f;

		public float JumpDistance => jumpDistance;
		public float JumpDuration => jumpDuration;
		public float JumpInterval => jumpInterval;
		public float MaxTurnAngle => maxTurnAngle;

		[UsedImplicitly]
		public JumpMovementConfig()
		{
		}

		public JumpMovementConfig(
			float jumpDistance,
			float jumpDuration,
			float jumpInterval,
			float maxTurnAngle)
		{
			this.jumpDistance = jumpDistance;
			this.jumpDuration = jumpDuration;
			this.jumpInterval = jumpInterval;
			this.maxTurnAngle = maxTurnAngle;
		}
	}
}
