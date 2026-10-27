using Code.Gameplay.Common.Time;
using Code.Gameplay.Features.Movement.Strategies;
using Code.Gameplay.Features.WorldBounds.Extensions;
using Code.Gameplay.Features.WorldBounds.Services;
using Code.Infrastructure.Pooling;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Features.Movement.Behaviours
{
	[RequireComponent(typeof(Rigidbody))]
	public class RigidbodyMovement : MonoBehaviour, IPooledHandler
	{
		[SerializeField] private Rigidbody body;

		private IMovementStrategy _movementStrategy;
		private Vector3 _direction;

		private ITimeService _timeService;
		private IWorldBoundsService _worldBoundsService;

		[Inject]
		private void Construct(ITimeService timeService, IWorldBoundsService worldBoundsService)
		{
			_timeService = timeService;
			_worldBoundsService = worldBoundsService;
		}

		public void Setup(IMovementStrategy movementStrategy, Vector3 direction)
		{
			_movementStrategy = movementStrategy;
			_direction = direction;
		}

		private void FixedUpdate()
		{
			Vector3 velocity = _movementStrategy.Tick(_direction, _timeService.FixedDeltaTime);

			if (velocity != Vector3.zero)
				_direction = velocity.normalized;

			_direction = _worldBoundsService.GetBounds().TurnInside(body.position, _direction);
			body.linearVelocity = _direction * velocity.magnitude;
		}

		public void Teleport(Vector3 at)
		{
			transform.SetPositionAndRotation(at, Quaternion.identity);
			body.position = at;
		}

		public void OnPutToPool() => body.linearVelocity = Vector3.zero;
	}
}
