using UnityEngine;

namespace UltimateSurvival
{
	[System.Serializable]
	public class StatDepleter
	{
		[SerializeField]
		[Clamp(0f, 100f)]
		private float m_DepletionRate = 1f;

		[SerializeField]
		[Clamp(0f, 100f)]
		[Tooltip("Damage applied when this stat reaches 0.")]
		private float m_Damage = 3f;

		[SerializeField]
		[Clamp(0f, 100f)]
		[Tooltip("How frequent damage will be applied.")]
		private float m_DamagePeriod = 3f;

		private float m_LastDamageTime;


		public void Update(Value<float> statValue, Attempt<HealthEventData> healthChanger)
		{
			float thirst = statValue.Get() - m_DepletionRate * Time.deltaTime;
			thirst = Mathf.Clamp(thirst, 0f, 100f);
			statValue.Set(thirst);

			if(statValue.Is(0f) && Time.time - m_LastDamageTime > m_DamagePeriod)
			{
				m_LastDamageTime = Time.time;
				healthChanger.Try(new HealthEventData(-m_Damage));
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class PlayerVitals : EntityVitals 
	{
		public PlayerEventHandler Player
		{
			get 
			{
				if(!m_Player)
					m_Player = GetComponent<PlayerEventHandler>();
				if(!m_Player)
					m_Player = GetComponentInParent<PlayerEventHandler>();
				
				return m_Player;
			}
		}

		[Header("Stamina")]

		[SerializeField]
		[Clamp(0f, 300f)]
		private float m_StaminaDepletionRate = 30f;

		[SerializeField]
		private StatRegenData m_StaminaRegeneration;

		[SerializeField]
		private SoundPlayer m_BreathingHeavyAudio;

		[SerializeField]
		private float m_BreathingHeavyDuration = 11f;

		[SerializeField]
		[Clamp(0f, 100f)]
		private float m_JumpStaminaTake = 15f;

		[Header("Thirst")]

		[SerializeField]
		private StatDepleter m_ThirstDepletion;

		[Header("Hunger")]

		[SerializeField]
		private StatDepleter m_HungerDepletion;

		[Header("Sleeping")]

		[SerializeField]
		private bool m_SleepRestoresHealth = true;

		private PlayerEventHandler m_Player;
		private float m_LastHeavyBreathTime;


		protected override void Update()
		{
			base.Update();

			// Stamina.
			if(Player.Run.Active)
			{
				m_StaminaRegeneration.Pause();
				ModifyStamina(-m_StaminaDepletionRate * Time.deltaTime);
			}
			else if(m_StaminaRegeneration.CanRegenerate && Player.Stamina.Get() < 100f)
				ModifyStamina(m_StaminaRegeneration.RegenDelta);

			if(!m_StaminaRegeneration.CanRegenerate && Player.Stamina.Is(0f) && Time.time - m_LastHeavyBreathTime > m_BreathingHeavyDuration)
			{
				m_LastHeavyBreathTime = Time.time;
				m_BreathingHeavyAudio.Play2D();
			}

			// Thirst.
			m_ThirstDepletion.Update(Player.Thirst, Player.ChangeHealth);

			// Hunger.
			m_HungerDepletion.Update(Player.Hunger, Player.ChangeHealth);
		}

		protected override bool Try_ChangeHealth(HealthEventData healthEventData)
		{
			healthEventData.Delta *= (1f - Player.Defense.Get() / 100f);
			return base.Try_ChangeHealth(healthEventData);
		}

		private void Start()
		{
			// HACK:
			Player.Run.AddStartTryer(()=> { m_StaminaRegeneration.Pause(); return Player.Stamina.Get() > 0f; });

			Player.Jump.AddStartListener(()=> ModifyStamina(-m_JumpStaminaTake));

			if(m_SleepRestoresHealth)
				Player.Sleep.AddStopListener(()=> Player.ChangeHealth.Try(new HealthEventData(100f)));
		}

		private void ModifyStamina(float delta)
		{
			float stamina = Player.Stamina.Get() + delta;
			stamina = Mathf.Clamp(stamina, 0f, 100f);
			Player.Stamina.Set(stamina);
		}
	}
}
