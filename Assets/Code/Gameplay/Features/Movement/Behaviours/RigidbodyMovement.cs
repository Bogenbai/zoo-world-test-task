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

		[SerializeField, Min(0f)]
		[Tooltip("How long physics alone moves the body after a bounce, so the animals visibly fly apart.")]
		private float knockbackDuration = 0.5f;

		[SerializeField, Min(0f)]
		[Tooltip("Slows the body while it flies apart, so it has almost stopped by the end of the knockback.")]
		private float knockbackDamping = 6f;

		[SerializeField, Min(0f)]
		[Tooltip("How long the animal stands still after the knockback, so the hit reads before it moves on.")]
		private float recoveryDelay = 0.5f;

		private IMovementStrategy _movementStrategy;
		private Vector3 _direction;

		private float _knockbackTimeLeft;
		private float _recoveryTimeLeft;
		private float _baseDamping;

		private ITimeService _timeService;
		private IWorldBoundsService _worldBoundsService;

		[Inject]
		private void Construct(ITimeService timeService, IWorldBoundsService worldBoundsService)
		{
			_timeService = timeService;
			_worldBoundsService = worldBoundsService;
		}

		private void Awake()
		{
			_baseDamping = body.linearDamping;
		}

		public void Setup(IMovementStrategy movementStrategy, Vector3 direction)
		{
			_movementStrategy = movementStrategy;
			_direction = direction;
		}

		private void FixedUpdate()
		{
			float deltaTime = _timeService.FixedDeltaTime;

			if (_knockbackTimeLeft > 0f)
				FlyAfterKnockback(deltaTime);
			else if (_recoveryTimeLeft > 0f)
				Recover(deltaTime);
			else
				MoveByStrategy(deltaTime);
		}

		public void Teleport(Vector3 at)
		{
			transform.SetPositionAndRotation(at, Quaternion.identity);
			body.position = at;
		}

		public void Knockback()
		{
			_knockbackTimeLeft = knockbackDuration;
			_recoveryTimeLeft = recoveryDelay;
		}

		public void OnPutToPool()
		{
			_knockbackTimeLeft = 0f;
			_recoveryTimeLeft = 0f;
			Stop();
		}

		private void MoveByStrategy(float deltaTime)
		{
			Vector3 velocity = _movementStrategy.Tick(_direction, deltaTime);

			if (velocity != Vector3.zero)
				_direction = velocity.normalized;

			_direction = TurnInside(_direction);
			body.linearVelocity = _direction * velocity.magnitude;
		}

		private void FlyAfterKnockback(float deltaTime)
		{
			_knockbackTimeLeft -= deltaTime;
			body.linearDamping = knockbackDamping;
			Vector3 velocity = body.linearVelocity;

			if (velocity != Vector3.zero)
			{
				_direction = TurnInside(velocity.normalized);
				body.linearVelocity = _direction * velocity.magnitude;
			}

			if (_knockbackTimeLeft <= 0f)
				Stop();
		}

		private void Recover(float deltaTime)
		{
			_recoveryTimeLeft -= deltaTime;
			Stop();
		}

		private void Stop()
		{
			body.linearDamping = _baseDamping;
			body.linearVelocity = Vector3.zero;
		}

		private Vector3 TurnInside(Vector3 direction) =>
			_worldBoundsService.GetBounds().TurnInside(body.position, direction);
	}
}
