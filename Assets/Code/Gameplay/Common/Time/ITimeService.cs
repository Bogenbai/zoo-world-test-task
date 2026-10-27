namespace Code.Gameplay.Common.Time
{
	public interface ITimeService
	{
		float DeltaTime { get; }
		float FixedDeltaTime { get; }
	}
}
