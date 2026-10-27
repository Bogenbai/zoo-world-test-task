using Code.Gameplay.Features.Animals.Behaviours;

namespace Code.Gameplay.Features.Animals.Services
{
	public interface IAnimalRegistry
	{
		void Add(Animal animal);
		bool Remove(Animal animal);
		bool Contains(Animal animal);
		int GetCount();
	}
}
