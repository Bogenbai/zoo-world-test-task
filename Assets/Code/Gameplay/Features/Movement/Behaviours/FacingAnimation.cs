using Code.Gameplay.Common.Time;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Features.Movement.Behaviours
{
	public class FacingAnimation : MonoBehaviour
	{
		[SerializeField] private Rigidbody body;

		[SerializeField, Min(0f)]
		[Tooltip("Degrees per second, so the model turns smoothly instead of snapping to every new direction.")]
		private float turnSpeed = 720f;

		private ITimeService _timeService;

		private const float MinSpeed = 0.1f;

		[Inject]
		private void Construct(ITimeService timeService)
		{
			_timeService = timeService;
		}

		private void Update()
		{
			Vector3 velocity = body.linearVelocity;
			velocity.y = 0f;

			if (velocity.sqrMagnitude < MinSpeed * MinSpeed)
				return;

			Quaternion target = Quaternion.LookRotation(velocity);
			float maxTurn = turnSpeed * _timeService.DeltaTime;
			transform.rotation = Quaternion.RotateTowards(transform.rotation, target, maxTurn);
		}
	}
}
