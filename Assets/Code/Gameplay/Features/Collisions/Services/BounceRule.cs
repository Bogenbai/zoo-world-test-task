using Code.Gameplay.Features.Animals.Behaviours;

namespace Code.Gameplay.Features.Collisions.Services
{
	public class BounceRule : ICollisionRule
	{
		public bool TryHandle(Animal first, Animal second)
		{
			first.Movement.Knockback();
			second.Movement.Knockback();

			return true;
		}
	}
}
