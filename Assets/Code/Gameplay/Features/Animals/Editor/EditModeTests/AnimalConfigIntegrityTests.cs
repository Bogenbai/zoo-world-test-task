using System.Collections.Generic;
using System.Linq;
using Code.Gameplay.Features.Animals.Behaviours;
using Code.Gameplay.Features.Animals.Configs;
using Code.Infrastructure;
using FluentAssertions;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Code.Gameplay.Features.Animals.Editor.EditModeTests
{
	[Category(TestCategories.Gameplay)]
	public class AnimalConfigIntegrityTests
	{
		[Test]
		public void AnimalConfigs_ShouldExist()
		{
			// Act
			List<AnimalConfig> configs = LoadAnimalConfigs();

			// Assert
			configs.Should().NotBeEmpty("the spawner has nothing to spawn without them");
		}

		[Test]
		public void EveryAnimalConfig_ShouldReferencePrefabWithAnimal()
		{
			// Act
			List<string> broken = NamesOf(LoadAnimalConfigs().Where(config => HasAnimalPrefab(config) == false));

			// Assert
			broken.Should().BeEmpty("an animal is created from a prefab that carries the Animal component");
		}

		[Test]
		public void EveryAnimalConfig_ShouldHaveRole()
		{
			// Act
			List<string> broken = NamesOf(LoadAnimalConfigs().Where(config => config.Role == null));

			// Assert
			broken.Should().BeEmpty("collisions are decided by the food chain role");
		}

		[Test]
		public void EveryAnimalConfig_ShouldHaveMovement()
		{
			// Act
			List<string> broken = NamesOf(LoadAnimalConfigs().Where(config => config.Movement == null));

			// Assert
			broken.Should().BeEmpty("the movement strategy factory throws for a missing movement type");
		}

		private static List<AnimalConfig> LoadAnimalConfigs() =>
			AssetDatabase.FindAssets($"t:{nameof(AnimalConfig)}")
				.Select(guid => AssetDatabase.LoadAssetAtPath<AnimalConfig>(AssetDatabase.GUIDToAssetPath(guid)))
				.ToList();

		private static List<string> NamesOf(IEnumerable<AnimalConfig> configs) =>
			configs.Select(config => config.name).ToList();

		private static bool HasAnimalPrefab(AnimalConfig config)
		{
			string path = AssetDatabase.GUIDToAssetPath(config.Prefab.AssetGUID);
			GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

			return prefab != null && prefab.GetComponent<Animal>() != null;
		}
	}
}
