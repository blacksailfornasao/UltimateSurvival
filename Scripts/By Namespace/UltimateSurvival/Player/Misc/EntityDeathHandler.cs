using System.Collections;
using UnityEngine;

namespace UltimateSurvival
{
	public class EntityDeathHandler : EntityBehaviour
	{
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
		[Tooltip("On death, you can either have a ragdoll, or an animation to play.")]
		private bool m_EnableRagdoll;

		[SerializeField]
		[Tooltip("A Ragdoll component, usually attached to the armature of the character.")]
		private Ragdoll m_Ragdoll;

		[Header("Death Animation")]

		[SerializeField]
		[Tooltip("On death, you can either have a ragdoll, or an animation to play.")]
		private bool m_EnableDeathAnim;

		[SerializeField]
		private Animator m_Animator;

		[Header("Destroy Timer")]

		[SerializeField]
		[Clamp(0f, 1000f)]
		[Tooltip("")]
		private float m_DestroyTimer = 0f;

		private Vector3 m_CamStartPos;
		private Quaternion m_CamStartRot;


		private void Awake()
		{
			if(m_EnableRagdoll && !m_Ragdoll)
				Debug.LogError("The ragdoll option has been enabled but no ragdoll object is assigned!", this);

			Entity.Health.AddChangeListener(OnChanged_Health);
		}

		private void OnChanged_Health()
		{
			if(Entity.Health.Is(0f))
				On_Death();
		}

		private void On_Death()
		{
			m_DeathAudio.Play(ItemSelectionMethod.Randomly, m_AudioSource);

			if(m_EnableRagdoll && m_Ragdoll)
			{
				m_Ragdoll.Enable();
				StartCoroutine(C_DisableRagdoll());
			}

			if(m_EnableDeathAnim && m_Animator)
				m_Animator.SetTrigger("Die");

			foreach(var obj in m_ObjectsToDisable)
				obj.SetActive(false);

			foreach(var behaviour in m_BehavioursToDisable)
			{
				var animator = behaviour as Animator;
				if(animator != null)
					Destroy(animator);
				else
					behaviour.enabled = false;
			}

			foreach(var collider in m_CollidersToDisable)
				collider.enabled = false;

			Destroy(gameObject, m_DestroyTimer);

			Entity.Death.Send();
		}

		private IEnumerator C_DisableRagdoll()
		{
			yield return new WaitForSeconds(3f);

			m_Ragdoll.Disable();
		}
	}
}
