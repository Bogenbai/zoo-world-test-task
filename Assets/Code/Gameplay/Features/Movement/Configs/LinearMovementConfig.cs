using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Code.Gameplay.Features.Movement.Configs
{
	[Serializable]
	public class LinearMovementConfig : MovementConfig
	{
		[SerializeField, Min(0f)]
		private float speed = 2.5f;

		public float Speed => speed;

		[UsedImplicitly]
		public LinearMovementConfig()
		{
		}

		public LinearMovementConfig(float speed)
		{
			this.speed = speed;
		}
	}
}
