using System.Linq;
using Code.Gameplay.Features.FoodChain.Configs;

namespace Code.Gameplay.Features.FoodChain.Extensions
{
	public static class FoodChainRoleConfigExtensions
	{
		public static bool Eats(this FoodChainRoleConfig eater, FoodChainRoleConfig food) =>
			eater.CanEat.Contains(food);
	}
}
