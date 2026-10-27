using Code.Gameplay.Features.FoodChain.Configs;

namespace Code.Gameplay.Features.Statistics.Services
{
	public interface IDeathStatisticsService
	{
		void RegisterDeath(FoodChainRoleConfig role);
	}
}
