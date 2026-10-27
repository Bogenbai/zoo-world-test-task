using Code.Infrastructure;
using UnityEngine;

namespace Code.Gameplay.Features.WorldBounds.Configs
{
	[CreateAssetMenu(menuName = Constants.GameName + "/Configs/World Config", fileName = "WorldConfig")]
	public class WorldConfig : ScriptableObject
	{
		[SerializeField, Min(0f)]
		[Tooltip("Animals turn back this far inside the screen edge, so they never fully leave the view.")]
		private float screenMargin = 0.6f;

		public float ScreenMargin => screenMargin;
	}
}
