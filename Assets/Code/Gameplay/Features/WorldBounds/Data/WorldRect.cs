namespace Code.Gameplay.Features.WorldBounds.Data
{
	public readonly struct WorldRect
	{
		public float MinX { get; }
		public float MaxX { get; }
		public float MinZ { get; }
		public float MaxZ { get; }

		public WorldRect(
			float minX,
			float maxX,
			float minZ,
			float maxZ)
		{
			MinX = minX;
			MaxX = maxX;
			MinZ = minZ;
			MaxZ = maxZ;
		}
	}
}
