using System;
using Code.Gameplay.Features.FoodChain.Configs;

namespace Code.Gameplay.Features.Statistics.Services
{
	public interface IDeathStatisticsQuery
	{
		event Action OnDeathsChanged;

		int GetDeathCount(FoodChainRoleConfig role);
	}
}
