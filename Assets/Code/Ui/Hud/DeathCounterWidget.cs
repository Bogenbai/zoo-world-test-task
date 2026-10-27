using Code.Gameplay.Features.FoodChain.Configs;
using Code.Gameplay.Features.Statistics.Services;
using Code.Infrastructure.ConfigsManagement;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Code.Ui.Hud
{
	public class DeathCounterWidget : MonoBehaviour
	{
		[SerializeField] private AssetReferenceT<FoodChainRoleConfig> role;

		[SerializeField] private string label;

		[SerializeField] private TMP_Text text;

		private FoodChainRoleConfig _role;

		private IConfigsService _configsService;
		private IDeathStatisticsQuery _deathStatisticsQuery;

		[Inject]
		private void Construct(IConfigsService configsService, IDeathStatisticsQuery deathStatisticsQuery)
		{
			_configsService = configsService;
			_deathStatisticsQuery = deathStatisticsQuery;
		}

		private void Awake()
		{
			_role = _configsService.GetFoodChainRole(role);
		}

		private void OnEnable()
		{
			RefreshText();
			_deathStatisticsQuery.OnDeathsChanged += HandleDeathsChanged;
		}

		private void OnDisable()
		{
			_deathStatisticsQuery.OnDeathsChanged -= HandleDeathsChanged;
		}

		private void RefreshText()
		{
			text.text = $"{label}: {_deathStatisticsQuery.GetDeathCount(_role)}";
		}

		private void HandleDeathsChanged()
		{
			RefreshText();
		}
	}
}
