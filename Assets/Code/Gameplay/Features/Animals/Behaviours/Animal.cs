using Code.Gameplay.Features.Animals.Configs;
using Code.Gameplay.Features.Collisions.Services;
using Code.Gameplay.Features.Movement.Behaviours;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Features.Animals.Behaviours
{
	[RequireComponent(typeof(RigidbodyMovement))]
	public class Animal : MonoBehaviour
	{
		[SerializeField] private RigidbodyMovement movement;

		private IAnimalCollisionService _animalCollisionService;

		public AnimalConfig Config { get; private set; }
		public RigidbodyMovement Movement => movement;

		[Inject]
		private void Construct(IAnimalCollisionService animalCollisionService)
		{
			_animalCollisionService = animalCollisionService;
		}

		public void Setup(AnimalConfig config)
		{
			Config = config;
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (collision.rigidbody != null && collision.rigidbody.TryGetComponent(out Animal other))
				_animalCollisionService.ProcessCollision(this, other);
		}
	}
}
