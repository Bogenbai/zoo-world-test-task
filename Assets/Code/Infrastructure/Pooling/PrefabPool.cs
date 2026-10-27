using System.Collections.Generic;
using UnityEngine;

namespace Code.Infrastructure.Pooling
{
	public class PrefabPool : IPrefabPool
	{
		private readonly Dictionary<Component, Stack<Component>> _instancesByPrefab = new();

		public bool TryGet<T>(T prefab, out T instance) where T : Component
		{
			if (_instancesByPrefab.TryGetValue(prefab, out Stack<Component> instances) && instances.Count > 0)
			{
				instance = (T)instances.Pop();

				return true;
			}

			instance = null;

			return false;
		}

		public void Put<T>(T prefab, T instance) where T : Component
		{
			foreach (IPooledHandler handler in instance.GetComponentsInChildren<IPooledHandler>(true))
				handler.OnPutToPool();

			instance.gameObject.SetActive(false);
			GetInstances(prefab).Push(instance);
		}

		private Stack<Component> GetInstances(Component prefab)
		{
			if (_instancesByPrefab.TryGetValue(prefab, out Stack<Component> instances) == false)
			{
				instances = new();
				_instancesByPrefab.Add(prefab, instances);
			}

			return instances;
		}
	}
}
