using System;
using Code.Gameplay.Features.Animals.Behaviours;

namespace Code.Gameplay.Features.FoodChain.Services
{
	public interface IPredationQuery
	{
		event Action<Animal> OnPredatorAte;
	}
}
