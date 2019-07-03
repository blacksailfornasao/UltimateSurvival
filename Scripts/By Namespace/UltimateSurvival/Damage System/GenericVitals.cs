using UnityEngine;

namespace UltimateSurvival
{
	[System.Serializable]
	public class StatRegenData
	{
		public bool CanRegenerate { get { return m_Enabled && !IsPaused; } }

		public bool Enabled { get { return m_Enabled; } }

		public bool IsPaused { get { return Time.time < m_NextRegenTime; } }

		public float RegenDelta { get { return m_Speed * Time.deltaTime; } }

		[SerializeField]
		private bool m_Enabled = true;

		[SerializeField]
		private float m_Pause = 2f;

		[SerializeField]
		[Clamp(0f, 1000f)]
		private float m_Speed = 10f;

		private float m_NextRegenTime;


		public void Pause()
		{
			m_NextRegenTime = Time.time + m_Pause;
		}
	}

    public class GenericVitals : EntityBehaviour
    {
        [Header("Health & Damage")]

        [SerializeField]
        [Tooltip("The health to start with.")]
        private float m_MaxHealth = 100f;

		[SerializeField]
		private StatRegenData m_HealthRegeneration;

		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("0 -> the damage received will not be decreased, \n1 -> the damage will be reduced to 0 (GOD mode).")]
		private float m_Resistance = 0.1f;

        [Header("Audio")]

        [SerializeField]
		protected AudioSource m_AudioSource;

		protected float m_HealthDelta;
        private float m_NextRegenTime;
        

        private void Start() 
		{
			Entity.ChangeHealth.SetTryer(Try_ChangeHealth); 
			m_MaxHealth *= 5f;
		}

        protected virtual void Update()
		{
			if(m_HealthRegeneration.CanRegenerate && Entity.Health.Get() < 100f && Entity.Health.Get() > 0f)
			{
				var data = new HealthEventData(m_HealthRegeneration.RegenDelta);
				Entity.ChangeHealth.Try(data);
			}
		}

		protected virtual bool Try_ChangeHealth(HealthEventData healthEventData)
		{
			if(Entity.Health.Get() == 0f)
				return false;
			if(healthEventData.Delta > 0f && Entity.Health.Get() == 100f)
				return false;

			float healthDelta = healthEventData.Delta;
			if(healthDelta < 0f)
				healthDelta *= (1f - m_Resistance);

			float newHealth = Mathf.Clamp(Entity.Health.Get() + healthDelta, 0f, 100f);
			Entity.Health.Set(newHealth);

			if(healthDelta < 0f)
				m_HealthRegeneration.Pause();

			return true;
		}
    }
}