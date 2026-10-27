using UnityEngine;

namespace Code.Infrastructure.Pooling
{
	public interface IPrefabPool
	{
		bool TryGet<T>(string prefabKey, out T instance) where T : Component;
		void Put(string prefabKey, Component instance);
	}
}
