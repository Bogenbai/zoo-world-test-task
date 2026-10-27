using System.Collections.Generic;
using Code.Gameplay.Features.Animals.Behaviours;

namespace Code.Gameplay.Features.Animals.Services
{
	public class AnimalRegistry : IAnimalRegistry
	{
		private readonly List<Animal> _aliveAnimals = new();

		public void Add(Animal animal) => _aliveAnimals.Add(animal);
		public int GetAliveCount() => _aliveAnimals.Count;
	}
}
