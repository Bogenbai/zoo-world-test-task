using System.Collections.Generic;
using UnityEngine;

namespace Code.Infrastructure.AssetManagement
{
	public interface IAssetsService
	{
		void CleanUp();
		TAsset Load<TAsset>(string key) where TAsset : Object;
		IReadOnlyList<TAsset> LoadAllByLabel<TAsset>(string label) where TAsset : Object;
	}
}
