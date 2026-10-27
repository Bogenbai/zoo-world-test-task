using Code.Infrastructure.AssetManagement;
using UnityEngine;

namespace Code.Ui.FloatingTexts.Services
{
	public interface IFloatingTextFactory
	{
		FloatingText CreateText(AssetReferencePrefab<FloatingText> prefab, Transform parent);
	}
}
