using Code.Gameplay.Features.Movement.Configs;
using Code.Gameplay.Features.Movement.Strategies;

namespace Code.Gameplay.Features.Movement.Services
{
	public interface IMovementStrategyFactory
	{
		IMovementStrategy CreateStrategy(MovementConfig config);
	}
}
