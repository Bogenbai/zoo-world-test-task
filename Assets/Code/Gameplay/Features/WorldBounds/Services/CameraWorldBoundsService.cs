using Code.Gameplay.Features.WorldBounds.Data;
using Code.Gameplay.Features.WorldBounds.Extensions;
using Code.Infrastructure.ConfigsManagement;
using UnityEngine;

namespace Code.Gameplay.Features.WorldBounds.Services
{
	public class CameraWorldBoundsService : IWorldBoundsService
	{
		private WorldRect _bounds;
		private Vector2Int _boundsScreenSize;

		private readonly Camera _camera;
		private readonly IConfigsService _configsService;

		private static readonly Plane Ground = new(Vector3.up, Vector3.zero);

		public CameraWorldBoundsService(Camera camera, IConfigsService configsService)
		{
			_camera = camera;
			_configsService = configsService;
		}

		public WorldRect GetBounds()
		{
			Vector2Int screenSize = new(Screen.width, Screen.height);

			if (screenSize != _boundsScreenSize)
			{
				_bounds = CalculateBounds();
				_boundsScreenSize = screenSize;
			}

			return _bounds;
		}

		private WorldRect CalculateBounds()
		{
			Vector3 bottomLeft = ProjectOnGround(new Vector3(0f, 0f));
			Vector3 topRight = ProjectOnGround(new Vector3(1f, 1f));
			float margin = _configsService.GetWorldConfig().ScreenMargin;

			WorldRect visibleRect = new(
				Mathf.Min(bottomLeft.x, topRight.x),
				Mathf.Max(bottomLeft.x, topRight.x),
				Mathf.Min(bottomLeft.z, topRight.z),
				Mathf.Max(bottomLeft.z, topRight.z));

			return visibleRect.Shrink(margin);
		}

		private Vector3 ProjectOnGround(Vector3 viewportPoint)
		{
			Ray ray = _camera.ViewportPointToRay(viewportPoint);
			Ground.Raycast(ray, out float distance);

			return ray.GetPoint(distance);
		}
	}
}
