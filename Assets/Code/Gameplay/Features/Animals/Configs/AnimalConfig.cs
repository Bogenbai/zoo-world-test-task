using Code.Gameplay.Features.Animals.Behaviours;
using Code.Gameplay.Features.FoodChain.Configs;
using Code.Gameplay.Features.Movement.Configs;
using Code.Infrastructure;
using UnityEngine;

namespace Code.Gameplay.Features.Animals.Configs
{
	[CreateAssetMenu(menuName = Constants.GameName + "/Configs/Animal Config", fileName = "AnimalConfig")]
	public class AnimalConfig : ScriptableObject
	{
		[SerializeField] private Animal prefab;
		[SerializeField] private FoodChainRoleConfig role;

		[SerializeField, Min(0f)]
		[Tooltip("Relative chance to be picked on spawn: weight 2 appears twice as often as weight 1.")]
		private float spawnWeight = 1f;

		[SerializeReference, SubclassSelector]
		private MovementConfig movement;

		public Animal Prefab => prefab;
		public FoodChainRoleConfig Role => role;
		public float SpawnWeight => spawnWeight;
		public MovementConfig Movement => movement;
	}
}
