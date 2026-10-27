using System;
using Code.Gameplay.Features.Animals.Configs;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Gameplay.Features.Animals.Data
{
	[Serializable]
	public class AnimalSpawnEntry
	{
		[SerializeField] private AssetReferenceT<AnimalConfig> animal;

		[SerializeField, Min(0f)]
		[Tooltip("Relative chance to be picked on spawn: weight 2 appears twice as often as weight 1.")]
		private float weight = 1f;

		public AssetReferenceT<AnimalConfig> Animal => animal;
		public float Weight => weight;
	}
}
