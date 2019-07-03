using System.Collections;
using UnityEngine;

namespace UltimateSurvival
{
	public class PlayerDeathHandler : PlayerBehaviour
	{
		[SerializeField]
		private GameObject m_Camera;

		[Header("Audio")]

		[SerializeField]
		private AudioSource m_AudioSource;

		[SerializeField]
		private SoundPlayer m_DeathAudio;

		[Header("Stuff To Disable On Death")]

		[SerializeField]
		private GameObject[] m_ObjectsToDisable;

		[SerializeField]
		private Behaviour[] m_BehavioursToDisable;

		[SerializeField]
		private Collider[] m_CollidersToDisable;

		[Header("Ragdoll")]

		[SerializeField]
		private bool m_EnableRagdoll;

		[SerializeField]
		[Tooltip("A Ragdoll component, usually attached to the armature of the character.")]
		private Ragdoll m_Ragdoll;

		[Header("Respawn")]

		[SerializeField]
		private bool m_AutoRespawn = true;

		[SerializeField]
		private float m_RespawnDuration = 10f;

		[SerializeField]
		private float m_RespawnBlockTime = 3f;

		private Vector3 m_CamStartPos;
		private Quaternion m_CamStartRot;


		private void Awake()
		{
			if(m_EnableRagdoll && !m_Ragdoll)
				Debug.LogError("The ragdoll option has been enabled but no ragdoll object is assigned!", this);

			Player.Health.AddChangeListener(OnChanged_Health);

			m_CamStartPos = m_Camera.transform.localPosition;
			m_CamStartRot = m_Camera.transform.localRotation;
		}

		private void OnChanged_Health()
		{
			if(Player.Health.Is(0f))
			{
				On_Death();

				RaycastHit hitInfo;
				Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
				if(Physics.Raycast(ray, out hitInfo, 1.5f, ~0))
				{
					m_Camera.transform.position = hitInfo.point + Vector3.up * 0.1f;
					m_Camera.transform.rotation = Quaternion.Euler(-30f, Random.Range(-180f, 180f), 0f);
				}
			}	
		}

		private void On_Death()
		{
			m_DeathAudio.Play(ItemSelectionMethod.Randomly, m_AudioSource);

			if(m_EnableRagdoll && m_Ragdoll)
				m_Ragdoll.Enable();

			foreach(var obj in m_ObjectsToDisable)
				obj.SetActive(false);

			foreach(var behaviour in m_BehavioursToDisable)
				behaviour.enabled = false;

			foreach(var collider in m_CollidersToDisable)
				collider.enabled = false;

			if(m_AutoRespawn)
				StartCoroutine(C_Respawn());

			Player.Death.Send();
		}

		private IEnumerator C_Respawn()
		{
			yield return new WaitForSeconds(m_RespawnDuration);

			if(m_EnableRagdoll && m_Ragdoll)
				m_Ragdoll.Disable();

			m_Camera.transform.localPosition = m_CamStartPos;
			m_Camera.transform.localRotation = m_CamStartRot;

			if(Player.LastSleepPosition.Get() != Vector3.zero)
			{
				transform.position = Player.LastSleepPosition.Get();
				transform.rotation = Quaternion.Euler(Vector3.up * Random.Range(-180f, 180f));
			}
			else
			{
				var spawnPoints = GameObject.FindGameObjectsWithTag(Tags.SPAWN_POINT);
				if(spawnPoints != null && spawnPoints.Length > 0)
				{
					var randomSP = spawnPoints[Random.Range(0, spawnPoints.Length)];
					transform.position = randomSP.transform.position;
					transform.rotation = Quaternion.Euler(Vector3.up * Random.Range(-180f, 180f));
				}
			}

			yield return new WaitForSeconds(m_RespawnBlockTime);

			foreach(var obj in m_ObjectsToDisable)
				obj.SetActive(true);

			foreach(var behaviour in m_BehavioursToDisable)
				behaviour.enabled = true;

			foreach(var collider in m_CollidersToDisable)
				collider.enabled = true;

			Player.Health.Set(100f);
			Player.Thirst.Set(100f);
			Player.Hunger.Set(100f);
			Player.Stamina.Set(100f);

			Player.Respawn.Send();
		}
	}
}
