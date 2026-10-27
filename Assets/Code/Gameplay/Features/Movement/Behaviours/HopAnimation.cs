using Code.Gameplay.Common.Time;
using Code.Infrastructure.Pooling;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Features.Movement.Behaviours
{
	public class HopAnimation : MonoBehaviour, IPooledHandler
	{
		[SerializeField] private Rigidbody body;

		[SerializeField, Min(0f)]
		private float height = 0.5f;

		[SerializeField, Min(0f)]
		[Tooltip("The top-down camera does not show height, so the model also grows at the peak to read as closer.")]
		private float peakScale = 0.3f;

		[SerializeField, Min(0.01f)]
		[Tooltip("Match the jump duration of the movement, so the model lands when the body stops.")]
		private float duration = 0.2f;

		private Vector3 _restPosition;
		private Vector3 _restScale;
		private float _hopTime;
		private bool _isHopping;
		private bool _wasMoving;

		private ITimeService _timeService;

		private const float MinSpeed = 0.1f;

		[Inject]
		private void Construct(ITimeService timeService)
		{
			_timeService = timeService;
		}

		private void Awake()
		{
			_restPosition = transform.localPosition;
			_restScale = transform.localScale;
		}

		private void Update()
		{
			bool isMoving = body.linearVelocity.sqrMagnitude > MinSpeed * MinSpeed;

			if (isMoving && _wasMoving == false)
				StartHop();

			_wasMoving = isMoving;

			if (_isHopping)
				UpdateHop();
		}

		public void OnPutToPool()
		{
			_isHopping = false;
			_wasMoving = false;
			Land();
		}

		private void StartHop()
		{
			_hopTime = 0f;
			_isHopping = true;
		}

		private void UpdateHop()
		{
			_hopTime += _timeService.DeltaTime;
			float progress = Mathf.Clamp01(_hopTime / duration);
			float lift = Mathf.Sin(progress * Mathf.PI);

			transform.localPosition = _restPosition + Vector3.up * (height * lift);
			transform.localScale = _restScale * (1f + peakScale * lift);

			if (progress >= 1f)
			{
				_isHopping = false;
				Land();
			}
		}

		private void Land()
		{
			transform.localPosition = _restPosition;
			transform.localScale = _restScale;
		}
	}
}
