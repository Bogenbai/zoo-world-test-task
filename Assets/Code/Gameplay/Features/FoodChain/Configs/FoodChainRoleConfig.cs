using System.Collections.Generic;
using Code.Infrastructure;
using UnityEngine;

namespace Code.Gameplay.Features.FoodChain.Configs
{
	[CreateAssetMenu(
		menuName = Constants.GameName + "/Configs/Food Chain Role Config",
		fileName = "FoodChainRoleConfig")]
	public class FoodChainRoleConfig : ScriptableObject
	{
		[SerializeField] private List<FoodChainRoleConfig> canEat = new();

		public IReadOnlyList<FoodChainRoleConfig> CanEat => canEat;
	}
}
