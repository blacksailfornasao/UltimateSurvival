using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class EntityVitals : GenericVitals
	{
		[Header("Fall Damage")]

		[SerializeField]
		[Range(1f, 15f)]
		[Tooltip("At which landing speed, the entity will start taking damage.")]
		private float m_MinFallSpeed = 4f;

		[SerializeField]
		[Range(10f, 50f)]
		[Tooltip("At which landing speed, the entity will die, if it has no defense.")]
		private float m_MaxFallSpeed = 15f;

		[Header("Audio")]

		[SerializeField]
		[Tooltip("The sounds that will be played when this entity receives damage.")]
		private SoundPlayer m_HurtAudio;

		[SerializeField]
		private float m_TimeBetweenScreams = 1f;

		[SerializeField]
		private SoundPlayer m_FallDamageAudio;

		[Header("Animation")]

		[SerializeField]
		private Animator m_Animator;

		[SerializeField]
		private float m_GetHitMax = 30f;

		private float m_NextTimeCanScream;
	

		private void Awake()
		{
			Entity.ChangeHealth.SetTryer(Try_ChangeHealth);
			Entity.Land.AddListener(On_Landed);
			Entity.Health.AddChangeListener(OnChanged_Health);
		}

		private void OnChanged_Health()
		{
			float delta = Entity.Health.Get() - Entity.Health.GetLastValue();
			if(delta < 0f)
			{
				if (m_Animator != null) 
				{
					m_Animator.SetFloat ("Get Hit Amount", Mathf.Abs (delta / m_GetHitMax));
					m_Animator.SetTrigger ("Get Hit");
				}

				if(delta < 0f && Time.time > m_NextTimeCanScream)
				{
					m_HurtAudio.Play(ItemSelectionMethod.RandomlyButExcludeLast, m_AudioSource);
					m_NextTimeCanScream = Time.time + m_TimeBetweenScreams;
				}
			}
		}

		private void On_Landed(float landSpeed)
		{
			if(landSpeed >= m_MinFallSpeed)
			{
				Entity.ChangeHealth.Try(new HealthEventData(-100f * (landSpeed / m_MaxFallSpeed)));
				m_FallDamageAudio.Play(ItemSelectionMethod.Randomly, m_AudioSource);
			}
		}
	}
}
