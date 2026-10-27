using System;
using Code.Gameplay.Common.Random;
using Code.Gameplay.Features.Animals.Behaviours;
using Code.Gameplay.Features.Animals.Services;
using Code.Gameplay.Features.Collisions.Services;
using Code.Gameplay.Features.FoodChain.Extensions;

namespace Code.Gameplay.Features.FoodChain.Services
{
	public class PredationRule : ICollisionRule, IPredationQuery
	{
		private readonly IRandomService _randomService;
		private readonly IAnimalDeathService _animalDeathService;

		private const float CoinFlipChance = 0.5f;

		public event Action<Animal> OnPredatorAte;

		public PredationRule(IRandomService randomService, IAnimalDeathService animalDeathService)
		{
			_randomService = randomService;
			_animalDeathService = animalDeathService;
		}

		public bool TryHandle(Animal first, Animal second)
		{
			bool firstEatsSecond = first.Config.Role.Eats(second.Config.Role);
			bool secondEatsFirst = second.Config.Role.Eats(first.Config.Role);

			if (!firstEatsSecond && !secondEatsFirst)
				return false;

			Animal predator = firstEatsSecond ? first : second;

			if (firstEatsSecond && secondEatsFirst)
				predator = _randomService.Chance(CoinFlipChance) ? first : second;

			_animalDeathService.Kill(predator == first ? second : first);
			OnPredatorAte?.Invoke(predator);

			return true;
		}
	}
}
