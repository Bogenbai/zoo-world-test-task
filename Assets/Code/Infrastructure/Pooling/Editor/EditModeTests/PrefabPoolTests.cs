using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

namespace Code.Infrastructure.Pooling.Editor.EditModeTests
{
	[Category(TestCategories.Infrastructure)]
	public class PrefabPoolTests
	{
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
			bool found = _pool.TryGet(Create("Prefab"), out Transform _);

			found.Should().BeFalse("nothing was put into the pool");
		}

		[Test]
		public void PutInstance_ShouldBeReusedForItsPrefab()
		{
			Transform prefab = Create("Prefab");
			Transform instance = Create("Instance");

			_pool.Put(prefab, instance);
			_pool.TryGet(prefab, out Transform reused);

			reused.Should().BeSameAs(instance, "a returned instance is reused for the same prefab");
		}

		[Test]
		public void PutInstance_ShouldNotBeReusedForAnotherPrefab()
		{
			_pool.Put(Create("Prefab"), Create("Instance"));

			bool found = _pool.TryGet(Create("OtherPrefab"), out Transform _);

			found.Should().BeFalse("instances of different prefabs never mix");
		}

		[Test]
		public void ReusedInstance_ShouldLeavePool()
		{
			Transform prefab = Create("Prefab");
			_pool.Put(prefab, Create("Instance"));

			_pool.TryGet(prefab, out Transform _);
			bool foundAgain = _pool.TryGet(prefab, out Transform _);

			foundAgain.Should().BeFalse("one instance is handed out only once");
		}

		[Test]
		public void Put_ShouldDeactivateInstance()
		{
			Transform instance = Create("Instance");

			_pool.Put(Create("Prefab"), instance);

			instance.gameObject.activeSelf.Should().BeFalse("a pooled instance must not take part in the game");
		}

		private Transform Create(string name)
		{
			GameObject gameObject = new(name);
			_createdObjects.Add(gameObject);

			return gameObject.transform;
		}
	}
}
