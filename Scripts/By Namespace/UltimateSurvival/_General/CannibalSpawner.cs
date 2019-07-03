using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival
{
	public class CannibalSpawner : MonoBehaviour 
	{
		public enum SpawnMode { AtNight, AtDaytime, AllDay }

		[SerializeField]
		private bool m_ShowGUI = true;

		[SerializeField]
		private bool m_EnableSpawning = true;

		[SerializeField]
		[Space()]
		[Clamp(0, 99)]
		private int m_MaxCount = 20;

		[SerializeField]
		private SpawnMode m_SpawnMode;

		[SerializeField]
		[Clamp(3f, 120f)]
		private float m_SpawnInterval = 30f;

		[SerializeField]
		private GameObject[] m_Prefabs;

		private float m_LastUpdateTime;
		private GameObject[] m_SpawnPoints;
		private List<EntityEventHandler> m_AliveCannibals = new List<EntityEventHandler>();


		private void Start()
		{
			m_SpawnPoints = GameObject.FindGameObjectsWithTag(Tags.SPAWN_POINT);
		}

		private void OnGUI()
		{
			if(!m_ShowGUI)
				return;

			Rect rect = new Rect(8f, 64f, 256f, 20f);

			GUI.Label(rect, "# CANNIBAL SPAWNER");

			// Enable spawning toggle
			rect.width = 128f;
			rect.y = rect.yMax + 4f; 

			m_EnableSpawning = GUI.Toggle(rect, m_EnableSpawning, "Enable Spawning");

			// Spawn interval slider
			rect.y = rect.yMax + 4f;

			GUI.Label(rect, "Spawn Interval: " + m_SpawnInterval);

			rect.y = rect.yMax + 4f; 

			m_SpawnInterval = (int)GUI.HorizontalSlider(rect, m_SpawnInterval, 3f, 120f);

			// Mob cap slider
			rect.y = rect.yMax + 4f;

			GUI.Label(rect, "Max Count: " + m_MaxCount);

			rect.y = rect.yMax + 4f; 

			m_MaxCount = (int)GUI.HorizontalSlider(rect, m_MaxCount, 0, 99);

			/*// Add all button
			rect.y = rect.yMax + 4f;
			if(GUI.Button(rect, "Add All"))
			{
				int spawnCount = m_MobCap - m_AliveCannibals.Count;
				for(int i = 0; i < spawnCount; i ++)
					Spawn();
			}*/

			// Remove All button
			rect.y = rect.yMax + 4f; 

			if(GUI.Button(rect, "Remove All"))
			{
				foreach(var c in FindObjectsOfType<AIEventHandler>())
				{
					if(c.name.Contains("Cannibal"))
						Destroy(c.gameObject);
				}

				m_AliveCannibals.Clear();
			}
		}

		private void Update()
		{
			if(m_EnableSpawning && Time.time > m_LastUpdateTime && m_AliveCannibals.Count < m_MaxCount)
			{
				bool spawnAllDayCondition = m_SpawnMode == SpawnMode.AllDay;
				bool nightCondition = TimeOfDay.Instance.State.Get() == ET.TimeOfDay.Night && m_SpawnMode == SpawnMode.AtNight;
				bool daytimeCondition = TimeOfDay.Instance.State.Get() == ET.TimeOfDay.Day && m_SpawnMode == SpawnMode.AtDaytime;
				bool canSpawn = spawnAllDayCondition || nightCondition || daytimeCondition;

				if(!canSpawn)
					return;

				m_LastUpdateTime = Time.time + m_SpawnInterval;

				Spawn();
			}
		}

		private void Spawn()
		{
			var randomSP = m_SpawnPoints[Random.Range(0, m_SpawnPoints.Length)].transform;
			var randomPrefab = m_Prefabs[Random.Range(0, m_Prefabs.Length)];

			var cannibalObject = Instantiate(randomPrefab, randomSP.position, Quaternion.Euler(Vector3.up * Random.Range(-360f, 360f)));
			var cannibal = cannibalObject.GetComponent<EntityEventHandler>();

			if(cannibal != null)
			{
				m_AliveCannibals.Add(cannibal);
				cannibal.Death.AddListener(()=> On_CannibalDeath(cannibal));
			}
		}

		private void On_CannibalDeath(EntityEventHandler cannibal)
		{
			m_AliveCannibals.Remove(cannibal);
		}
	}
}
