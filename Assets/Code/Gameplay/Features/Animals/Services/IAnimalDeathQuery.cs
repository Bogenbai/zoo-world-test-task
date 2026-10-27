using System;
using Code.Gameplay.Features.Animals.Behaviours;

namespace Code.Gameplay.Features.Animals.Services
{
	public interface IAnimalDeathQuery
	{
		event Action<Animal> OnAnimalDied;
	}
}
