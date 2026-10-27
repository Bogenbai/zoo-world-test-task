using Code.Gameplay.Features.Animals.Configs;
using Code.Gameplay.Features.Movement.Behaviours;
using UnityEngine;

namespace Code.Gameplay.Features.Animals.Behaviours
{
	[RequireComponent(typeof(RigidbodyMovement))]
	public class Animal : MonoBehaviour
	{
		[SerializeField] private RigidbodyMovement movement;

		public AnimalConfig Config { get; private set; }
		public RigidbodyMovement Movement => movement;

		public void Setup(AnimalConfig config)
		{
			Config = config;
		}
	}
}
