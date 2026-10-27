using Code.Gameplay.Features.WorldBounds.Data;

namespace Code.Gameplay.Features.WorldBounds.Services
{
	public interface IWorldBoundsService
	{
		WorldRect GetBounds();
	}
}
