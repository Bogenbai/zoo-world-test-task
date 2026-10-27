using Code.Gameplay.Features.Animals.Behaviours;

namespace Code.Gameplay.Features.Collisions.Services
{
	public interface IAnimalCollisionService
	{
		void ProcessCollision(Animal animal, Animal other);
	}
}
