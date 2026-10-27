using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

namespace Code.Infrastructure.Pooling.Editor.EditModeTests
{
	[Category(TestCategories.Infrastructure)]
	public class PrefabPoolTests
	{
		private const string PrefabKey = "prefab";
		private const string OtherPrefabKey = "other_prefab";

		private readonly List<GameObject> _createdObjects = new();

		private PrefabPool _pool;

		[SetUp]
		public void SetUp()
		{
			_pool = new PrefabPool();
		}

		[TearDown]
		public void TearDown()
		{
			foreach (GameObject createdObject in _createdObjects)
				Object.DestroyImmediate(createdObject);

			_createdObjects.Clear();
		}

		[Test]
		public void EmptyPool_ShouldNotGiveInstance()
		{
			// Act
			bool found = _pool.TryGet(PrefabKey, out Transform _);

			// Assert
			found.Should().BeFalse("nothing was put into the pool");
		}

		[Test]
		public void PutInstance_ShouldBeReusedForItsPrefab()
		{
			// Arrange
			Transform instance = CreateInstance();
			_pool.Put(PrefabKey, instance);

			// Act
			_pool.TryGet(PrefabKey, out Transform reused);

			// Assert
			reused.Should().BeSameAs(instance, "a returned instance is reused for the same prefab");
		}

		[Test]
		public void PutInstance_ShouldNotBeReusedForAnotherPrefab()
		{
			// Arrange
			_pool.Put(PrefabKey, CreateInstance());

			// Act
			bool found = _pool.TryGet(OtherPrefabKey, out Transform _);

			// Assert
			found.Should().BeFalse("instances of different prefabs never mix");
		}

		[Test]
		public void ReusedInstance_ShouldLeavePool()
		{
			// Arrange
			_pool.Put(PrefabKey, CreateInstance());

			// Act
			_pool.TryGet(PrefabKey, out Transform _);
			bool foundAgain = _pool.TryGet(PrefabKey, out Transform _);

			// Assert
			foundAgain.Should().BeFalse("one instance is handed out only once");
		}

		[Test]
		public void Put_ShouldDeactivateInstance()
		{
			// Arrange
			Transform instance = CreateInstance();

			// Act
			_pool.Put(PrefabKey, instance);

			// Assert
			instance.gameObject.activeSelf.Should().BeFalse("a pooled instance must not take part in the game");
		}

		private Transform CreateInstance()
		{
			GameObject gameObject = new("Instance");
			_createdObjects.Add(gameObject);

			return gameObject.transform;
		}
	}
}
