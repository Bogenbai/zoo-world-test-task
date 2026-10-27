using System.Collections.Generic;
using Code.Gameplay.Features.Animals.Configs;
using Code.Gameplay.Features.WorldBounds.Configs;

namespace Code.Infrastructure.ConfigsManagement
{
	public interface IConfigsService
	{
		IReadOnlyList<AnimalConfig> GetAnimalConfigs();
		WorldConfig GetWorldConfig();
	}
}
