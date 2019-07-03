using UnityEngine;
using System.Collections.Generic;

namespace UltimateSurvival
{
    [System.Serializable]
    public class Pool
    {
        public string PoolName { get { return m_PoolName; } }

        public GameObject Prefab { get { return m_Prefab; } }

        public int Amount { get { return m_Amount; } }

        public List<GameObject> PooledObjects { get { return m_PooledObjects; } }

        [SerializeField]
        private string m_PoolName;

        [SerializeField]
        private GameObject m_Prefab;

        [SerializeField]
        private int m_Amount;

        private List<GameObject> m_PooledObjects = new List<GameObject>();

        private GameObject FindFirstActiveObject()
        {
            GameObject toGet = null;

            for (int i = 0; i < m_PooledObjects.Count; i++)
            {
                if (m_PooledObjects[i].activeInHierarchy)
                    toGet = m_PooledObjects[i];
            }

            if (toGet == null)
                Debug.LogError("There are no available active objects to get from the " + m_PoolName + " pool");

            return toGet;
        }

        private GameObject FindFirstInactiveObject()
        {
            GameObject toGet = null;

            for (int i = 0; i < m_PooledObjects.Count; i++)
            {
                if (!m_PooledObjects[i].activeInHierarchy)
                    toGet = m_PooledObjects[i];
            }

            if (toGet == null)
                Debug.LogError("There are no available inactive objects to get from the " + m_PoolName + " pool");

            return toGet;
        }

        public void Spawn(Vector3 position, Quaternion rotation)
        {
            GameObject toSpawn = FindFirstInactiveObject();
            Transform toSpawnTransform = toSpawn.transform;

            toSpawnTransform.position = position;
            toSpawnTransform.rotation = rotation;

            toSpawn.SetActive(true);

            toSpawn.GetComponent<PoolableObject>().OnSpawn();
        }

        public void SpawnAll(Vector3[] positions, Quaternion[] rotations)
        {
            for (int i = 0; i < m_PooledObjects.Count; i++)
                Spawn(positions[i], rotations[i]);
        }

        public void DespawnSpecificObject(GameObject toDespawn)
        {
            bool isPartOfPool = m_PooledObjects.Contains(toDespawn);

            if (isPartOfPool)
                toDespawn.SetActive(false);
            else
            {
                Debug.LogError("Object that entered the trigger is not part of the " + m_PoolName + " pool");
                return;
            }
        }

        public void Despawn()
        {
            GameObject toDespawn = FindFirstActiveObject();

            toDespawn.GetComponent<PoolableObject>().OnDespawn();

            toDespawn.SetActive(false);           
        }

        public void DestroyObjects(bool activeOnes)
        {
            for (int i = 0; i < m_PooledObjects.Count; i++)
            {
                if (m_PooledObjects[i].activeInHierarchy == activeOnes)
                {
                    m_PooledObjects[i].GetComponent<PoolableObject>().OnPoolableDestroy();

                    GameObject.Destroy(m_PooledObjects[i]);
                }
            }
        }

        public void DestroyAll()
        {
            for (int i = 0; i < m_PooledObjects.Count; i++)
            {
                m_PooledObjects[i].GetComponent<PoolableObject>().OnPoolableDestroy();

                GameObject.Destroy(m_PooledObjects[i]);
            }
        }

        public void DespawnAll()
        {
            for (int i = 0; i < m_PooledObjects.Count; i++)
            {
                m_PooledObjects[i].GetComponent<PoolableObject>().OnDespawn();

                m_PooledObjects[i].SetActive(false);
            }
        }
    }
}