using System;
using Code.Gameplay.Features.Movement.Configs;
using Code.Gameplay.Features.Movement.Strategies;
using Zenject;

namespace Code.Gameplay.Features.Movement.Services
{
	public class MovementStrategyFactory : IMovementStrategyFactory
	{
		private readonly IInstantiator _instantiator;

		public MovementStrategyFactory(IInstantiator instantiator)
		{
			_instantiator = instantiator;
		}

		public IMovementStrategy CreateStrategy(MovementConfig config)
		{
			switch (config)
			{
				case null:
					throw new ArgumentNullException(nameof(config));
				case LinearMovementConfig linearConfig:
					return _instantiator.Instantiate<LinearMovementStrategy>(new object[] { linearConfig });
				case JumpMovementConfig jumpConfig:
					return _instantiator.Instantiate<JumpMovementStrategy>(new object[] { jumpConfig });
				default:
					throw new ArgumentException($"No movement strategy for {config.GetType().Name}", nameof(config));
			}
		}
	}
}
