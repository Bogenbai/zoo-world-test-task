using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Animals.Behaviours;
using Code.Gameplay.Features.Animals.Configs;
using Code.Gameplay.Features.FoodChain.Configs;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Code.Gameplay.Features.Animals.Editor.EditModeTests
{
	public class AnimalTestObjects : IDisposable
	{
		private readonly List<Object> _createdObjects = new();

		public FoodChainRoleConfig CreateRole()
		{
			FoodChainRoleConfig role = ScriptableObject.CreateInstance<FoodChainRoleConfig>();
			_createdObjects.Add(role);

			return role;
		}

		public void SetDiet(FoodChainRoleConfig role, params FoodChainRoleConfig[] diet)
		{
			SerializedObject serializedRole = new(role);
			SerializedProperty canEat = serializedRole.FindProperty("canEat");
			canEat.arraySize = diet.Length;

			for (int i = 0; i < diet.Length; i++)
				canEat.GetArrayElementAtIndex(i).objectReferenceValue = diet[i];

			serializedRole.ApplyModifiedPropertiesWithoutUndo();
		}

		public Animal CreateAnimal(FoodChainRoleConfig role)
		{
			AnimalConfig config = ScriptableObject.CreateInstance<AnimalConfig>();
			_createdObjects.Add(config);

			SerializedObject serializedConfig = new(config);
			serializedConfig.FindProperty("prefab.m_AssetGUID").stringValue = Guid.NewGuid().ToString("N");
			serializedConfig.FindProperty("role").objectReferenceValue = role;
			serializedConfig.ApplyModifiedPropertiesWithoutUndo();

			GameObject gameObject = new("Animal");
			_createdObjects.Add(gameObject);

			Animal animal = gameObject.AddComponent<Animal>();
			animal.Setup(config);

			return animal;
		}

		public void Dispose()
		{
			foreach (Object createdObject in _createdObjects)
				Object.DestroyImmediate(createdObject);

			_createdObjects.Clear();
		}
	}
}
