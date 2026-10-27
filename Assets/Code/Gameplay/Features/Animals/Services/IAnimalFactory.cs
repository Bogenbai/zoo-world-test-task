using Code.Gameplay.Features.Animals.Behaviours;
using Code.Gameplay.Features.Animals.Configs;
using UnityEngine;

namespace Code.Gameplay.Features.Animals.Services
{
	public interface IAnimalFactory
	{
		Animal CreateAnimal(AnimalConfig config, Vector3 at, Vector3 direction);
	}
}
