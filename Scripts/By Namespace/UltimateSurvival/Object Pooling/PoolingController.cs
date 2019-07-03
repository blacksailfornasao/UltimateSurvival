using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	/*public class PoolingController : MonoSingleton<PoolingController> 
	{
		[SerializeField]
		private int m_DefaultPoolSize = 50;

		private Dictionary<int, ObjectPool> m_Pools = new Dictionary<int, ObjectPool>();
		private Transform m_Container;

		/// <summary>
		/// 
		/// </summary>
		public void CreatePool(PooledObject prefab, int size)
		{
			if(!prefab)
				return;

			if(!m_Pools.ContainsKey(prefab.GetInstanceID()))
			{
				var pool = new ObjectPool(prefab, size, m_Container);
				m_Pools.Add(prefab.GetInstanceID(), pool);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public PooledObject GetInstance(PooledObject prefab, Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion), Transform parent = null)
		{
			if(!prefab)
				return null;

			if(!m_Pools.ContainsKey(prefab.GetInstanceID()))
				CreatePool(prefab, m_DefaultPoolSize);

			ObjectPool parentPool = m_Pools[prefab.GetInstanceID()];

			var instance = parentPool.GetInstance();
			instance.OnUse(position, rotation, parent);

			return instance;
		}

		/// <summary>
		/// Will destroy all the objects in the pool, and the pool itself.
		/// </summary>
		public void DestroyPool(PooledObject prefab)
		{
			ObjectPool pool;
			if(m_Pools.TryGetValue(prefab.GetInstanceID(), out pool))
			{
				pool.DestroyObjects();
				m_Pools.Remove(prefab.GetInstanceID());
			}
		}

		private void Awake()
		{
			// Create the container object for all the future pooled objects.
			m_Container = new GameObject("Pooled Objects").transform;
			m_Container.SetParent(transform);
			m_Container.transform.localPosition = Vector3.zero;
			m_Container.transform.localRotation = Quaternion.identity;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class ObjectPool
	{
		private Transform m_LocalContainer;
		private Queue<PooledObject> m_Queue;

		
		public ObjectPool(PooledObject prefab, int size, Transform parentContainer)
		{
			m_LocalContainer = new GameObject(prefab.name + " (Pool)").transform;
			m_LocalContainer.SetParent(parentContainer);
			m_Queue = new Queue<PooledObject>();

			for(int i = 0;i < size;i ++)
			{
				var obj = GameObject.Instantiate<PooledObject>(prefab);
				obj.Released.AddListener(On_Released);

				obj.transform.SetParent(m_LocalContainer);
				obj.OnRelease();
				m_Queue.Enqueue(obj);
			}
		}

		public PooledObject GetInstance()
		{
			var instance = m_Queue.Dequeue();
			m_Queue.Enqueue(instance);

			return instance;
		}

		public void DestroyObjects()
		{
			for(int i = 0;i < m_Queue.Count;i ++)
				GameObject.Destroy(m_Queue.Dequeue());
		}

		private void On_Released(PooledObject releasedObject)
		{
			releasedObject.transform.SetParent(m_LocalContainer);
		}
	}*/
}
