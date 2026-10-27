using UnityEngine;

namespace Code.Infrastructure.Pooling
{
	public interface IPrefabPool
	{
		bool TryGet<T>(T prefab, out T instance) where T : Component;
		void Put<T>(T prefab, T instance) where T : Component;
	}
}
