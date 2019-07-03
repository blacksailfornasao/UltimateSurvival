using UnityEngine;
using System.Collections.Generic;

namespace UltimateSurvival
{
    public class PoolManager : MonoBehaviour
    {
        [SerializeField]
        private List<Pool> _pools = new List<Pool>();

        private void Awake()
        {
            InitializePools();
        }

        private void InitializePools()
        {
            for (int x = 0; x < _pools.Count; x++)
            {
                Pool pool = _pools[x];

                if (pool.PoolName == string.Empty)
                {
                    Debug.LogError("Pool with index of " + x + " does not contain a pool name.");
                    return;
                }

                Transform poolHolder = new GameObject(pool.PoolName + " Pool").transform;
                poolHolder.SetParent(transform);

                for (int i = 0; i < pool.Amount; i++)
                {
                    if (!pool.Prefab)
                    {
                        Debug.LogError("There is no prefab assigned for the " + pool.PoolName + " pool");
                        return;
                    }

                    GameObject poolObject = Instantiate(pool.Prefab);

                    poolObject.transform.SetParent(poolHolder);
                    poolObject.SetActive(false);

                    pool.PooledObjects.Add(poolObject);
                }
            }
        }

        public Pool GetPool(string name)
        {
            Pool toGet = null;

            for (int i = 0; i < _pools.Count; i++)
            {
                if (_pools[i].PoolName == name)
                    toGet = _pools[i];
            }

            if (toGet == null)
                Debug.LogError("Couldn't find pool with name: " + name);

            return toGet;
        }
    }
}