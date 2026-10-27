using Code.Gameplay.Features.Animals.Behaviours;
using Code.Gameplay.Features.FoodChain.Configs;
using Code.Gameplay.Features.Movement.Configs;
using Code.Infrastructure;
using Code.Infrastructure.AssetManagement;
using UnityEngine;

namespace Code.Gameplay.Features.Animals.Configs
{
	[CreateAssetMenu(menuName = Constants.GameName + "/Configs/Animal Config", fileName = "AnimalConfig")]
	public class AnimalConfig : ScriptableObject
	{
		[SerializeField] private AssetReferencePrefab<Animal> prefab;
		[SerializeField] private FoodChainRoleConfig role;

		[SerializeReference, SubclassSelector]
		private MovementConfig movement;

		public AssetReferencePrefab<Animal> Prefab => prefab;
		public FoodChainRoleConfig Role => role;
		public MovementConfig Movement => movement;
	}
}
