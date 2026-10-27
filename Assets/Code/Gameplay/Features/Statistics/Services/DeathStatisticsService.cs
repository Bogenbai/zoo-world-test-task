using System;
using System.Collections.Generic;
using Code.Gameplay.Features.FoodChain.Configs;

namespace Code.Gameplay.Features.Statistics.Services
{
	public class DeathStatisticsService : IDeathStatisticsService, IDeathStatisticsQuery
	{
		private readonly Dictionary<FoodChainRoleConfig, int> _deathsByRole = new();

		public event Action OnDeathsChanged;

		public void RegisterDeath(FoodChainRoleConfig role)
		{
			_deathsByRole[role] = GetDeathCount(role) + 1;
			OnDeathsChanged?.Invoke();
		}

		public int GetDeathCount(FoodChainRoleConfig role) => _deathsByRole.GetValueOrDefault(role);
	}
}
