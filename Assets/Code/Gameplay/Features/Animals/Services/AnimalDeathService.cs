using Code.Gameplay.Features.Animals.Behaviours;
using Code.Infrastructure.Pooling;

namespace Code.Gameplay.Features.Animals.Services
{
	public class AnimalDeathService : IAnimalDeathService
	{
		private readonly IAnimalRegistry _animalRegistry;
		private readonly IPrefabPool _prefabPool;

		public AnimalDeathService(IAnimalRegistry animalRegistry, IPrefabPool prefabPool)
		{
			_animalRegistry = animalRegistry;
			_prefabPool = prefabPool;
		}

		public void Kill(Animal animal)
		{
			if (_animalRegistry.Remove(animal))
				_prefabPool.Put(animal.Config.Prefab.AssetGUID, animal);
		}
	}
}
