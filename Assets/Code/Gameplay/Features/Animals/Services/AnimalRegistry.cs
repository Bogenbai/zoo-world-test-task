using System.Collections.Generic;
using Code.Gameplay.Features.Animals.Behaviours;

namespace Code.Gameplay.Features.Animals.Services
{
	public class AnimalRegistry : IAnimalRegistry
	{
		private readonly HashSet<Animal> _animals = new();

		public void Add(Animal animal) => _animals.Add(animal);
		public bool Remove(Animal animal) => _animals.Remove(animal);
		public bool Contains(Animal animal) => _animals.Contains(animal);
		public int GetCount() => _animals.Count;
	}
}
