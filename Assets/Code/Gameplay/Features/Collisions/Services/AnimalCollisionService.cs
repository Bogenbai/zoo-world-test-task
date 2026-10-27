using System.Collections.Generic;
using Code.Gameplay.Features.Animals.Behaviours;
using Code.Gameplay.Features.Animals.Services;

namespace Code.Gameplay.Features.Collisions.Services
{
	public class AnimalCollisionService : IAnimalCollisionService
	{
		private readonly IAnimalRegistry _animalRegistry;
		private readonly List<ICollisionRule> _rules;

		public AnimalCollisionService(IAnimalRegistry animalRegistry, List<ICollisionRule> rules)
		{
			_animalRegistry = animalRegistry;
			_rules = rules;
		}

		public void ProcessCollision(Animal animal, Animal other)
		{
			// Both bodies report the same contact, so only the one with the smaller id handles it
			if (animal.GetEntityId().CompareTo(other.GetEntityId()) > 0)
				return;

			if (_animalRegistry.Contains(animal) == false || _animalRegistry.Contains(other) == false)
				return;

			foreach (ICollisionRule rule in _rules)
			{
				if (rule.TryHandle(animal, other))
					return;
			}
		}
	}
}
