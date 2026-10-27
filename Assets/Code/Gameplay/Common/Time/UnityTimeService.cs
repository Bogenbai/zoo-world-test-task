namespace Code.Gameplay.Common.Time
{
	public class UnityTimeService : ITimeService
	{
		public float DeltaTime => UnityEngine.Time.deltaTime;
		public float FixedDeltaTime => UnityEngine.Time.fixedDeltaTime;
	}
}
