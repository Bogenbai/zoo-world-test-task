using System.Collections.Generic;
using UnityEngine;

namespace Code.Infrastructure.Pooling
{
	public class PrefabPool : IPrefabPool
	{
		private readonly Dictionary<string, Stack<Component>> _instancesByPrefab = new();

		public bool TryGet<T>(string prefabKey, out T instance) where T : Component
		{
			if (_instancesByPrefab.TryGetValue(prefabKey, out Stack<Component> instances) && instances.Count > 0)
			{
				instance = (T)instances.Pop();

				return true;
			}

			instance = null;

			return false;
		}

		public void Put(string prefabKey, Component instance)
		{
			foreach (IPooledHandler handler in instance.GetComponentsInChildren<IPooledHandler>(true))
				handler.OnPutToPool();

			instance.gameObject.SetActive(false);
			GetInstances(prefabKey).Push(instance);
		}

		private Stack<Component> GetInstances(string prefabKey)
		{
			if (_instancesByPrefab.TryGetValue(prefabKey, out Stack<Component> instances) == false)
			{
				instances = new();
				_instancesByPrefab.Add(prefabKey, instances);
			}

			return instances;
		}
	}
}
