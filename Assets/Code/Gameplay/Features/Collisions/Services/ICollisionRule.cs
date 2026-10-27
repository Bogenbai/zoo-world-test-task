using Code.Gameplay.Features.Animals.Behaviours;

namespace Code.Gameplay.Features.Collisions.Services
{
	public interface ICollisionRule
	{
		// True means the collision is fully handled and the rules after this one must not run
		bool TryHandle(Animal first, Animal second);
	}
}
