using Code.Gameplay.Common.Random;
using Code.Gameplay.Features.Animals.Behaviours;
using Code.Gameplay.Features.Animals.Services;
using Code.Gameplay.Features.Collisions.Services;
using Code.Gameplay.Features.FoodChain.Extensions;

namespace Code.Gameplay.Features.FoodChain.Services
{
	public class PredationRule : ICollisionRule
	{
		private readonly IRandomService _randomService;
		private readonly IAnimalDeathService _animalDeathService;

		private const float CoinFlipChance = 0.5f;

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

			if (firstEatsSecond && secondEatsFirst)
				_animalDeathService.Kill(_randomService.Chance(CoinFlipChance) ? second : first);
			else
				_animalDeathService.Kill(firstEatsSecond ? second : first);

			return true;
		}
	}
}
