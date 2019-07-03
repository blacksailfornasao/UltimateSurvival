using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UltimateSurvival.AI
{
	public class AIGroup : MonoBehaviour 
	{
		public enum SpawnMode { AtNight, AtDaytime, AllDay }

		[SerializeField]
		private Color m_GroupColor;

		[SerializeField]
		[Space()]
		private bool m_EnableSpawning = true;

		[SerializeField]
		private bool m_MakeAgentsChildren = true;

		[SerializeField]
		private GameObject[] m_Prefabs;

		[SerializeField]
		[Space()]
		[Clamp(0f, 30f)]
		private float m_GroupRadius = 10f;

		[SerializeField]
		[Clamp(0, 5)]
		private int m_MaxCount = 3;

		[SerializeField]
		[Space()]
		private SpawnMode m_SpawnMode;

		[SerializeField]
		[Clamp(3f, 120f)]
		private float m_SpawnInterval = 30f;

		private float m_LastUpdateTime;
		private List<Vector3> m_SpawnPoints = new List<Vector3>();
		private List<EntityEventHandler> m_AliveAgents = new List<EntityEventHandler>();
		private Transform m_Player;


		private void Start()
		{
			for(int sp = 0; sp < m_MaxCount; sp ++)
			{
				var randomPos = transform.position + new Vector3(Random.insideUnitCircle.x, 0f, Random.insideUnitCircle.y) * m_GroupRadius;

				NavMeshHit navMeshHit;
				if(NavMesh.SamplePosition(randomPos, out navMeshHit, 10f, NavMesh.AllAreas))
					randomPos = navMeshHit.position;

				m_SpawnPoints.Add(randomPos);

				TrySpawn();

				//Debug.DrawRay(randomPos, Vector3.up * 5f, Color.red, 5f);
			}

			m_Player = GameController.LocalPlayer.transform;
		}

		private void Update()
		{
			bool shouldSpawn =
				m_EnableSpawning && 
				Time.time > m_LastUpdateTime && 
				m_AliveAgents.Count < m_MaxCount && 
				Vector3.Dot(m_Player.forward, transform.position - m_Player.position) < 0f;

			if(shouldSpawn)
				TrySpawn();
		}

		private void TrySpawn()
		{
			// Spawn conditions.
			bool spawnAllDayCondition = m_SpawnMode == SpawnMode.AllDay;
			bool nightCondition = TimeOfDay.Instance.State.Get() == ET.TimeOfDay.Night && m_SpawnMode == SpawnMode.AtNight;
			bool daytimeCondition = TimeOfDay.Instance.State.Get() == ET.TimeOfDay.Day && m_SpawnMode == SpawnMode.AtDaytime;
			bool canSpawn = spawnAllDayCondition || nightCondition || daytimeCondition;

			if(!canSpawn)
				return;

			m_LastUpdateTime = Time.time + m_SpawnInterval;

			// Spawning logic.
			var randomPos = m_SpawnPoints[Random.Range(0, m_SpawnPoints.Count)];
			var randomPrefab = m_Prefabs[Random.Range(0, m_Prefabs.Length)];

			var cannibalObject = Instantiate(randomPrefab, randomPos, Quaternion.Euler(Vector3.up * Random.Range(-360f, 360f)));
			var agent = cannibalObject.GetComponent<EntityEventHandler>();

			if(m_MakeAgentsChildren)
				cannibalObject.transform.SetParent(transform, true);

			if(agent != null)
			{
				m_AliveAgents.Add(agent);
				agent.Death.AddListener(()=> On_AgentDeath(agent));
			}

            agent.GetComponent<NavMeshAgent>().Warp(randomPos);
        }

		private void On_AgentDeath(EntityEventHandler agent)
		{
			m_AliveAgents.Remove(agent);
		}

		private void OnDrawGizmosSelected()
		{
			Color prevCol = Gizmos.color;

			Gizmos.color = m_GroupColor;
			Gizmos.DrawSphere(transform.position, m_GroupRadius);

			Gizmos.color = prevCol;
		}
	}
}
