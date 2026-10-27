using Code.Gameplay.Features.Animals.Configs;
using Code.Gameplay.Features.WorldBounds.Configs;
using UnityEngine.AddressableAssets;

namespace Code.Infrastructure.ConfigsManagement
{
	public interface IConfigsService
	{
		AnimalConfig GetAnimalConfig(AssetReferenceT<AnimalConfig> reference);
		WorldConfig GetWorldConfig();
	}
}
