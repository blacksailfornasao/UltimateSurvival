using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class FPMelee : FPWeaponBase
	{
		/// <summary>
		/// 
		/// </summary>
		public enum HitInvokeMethod
		{
			/// <summary> </summary>
			ByTimer,

			/// <summary> </summary>
			ByAnimationEvent
		}

		/// <summary>True when an object was in range (subscribe to this instead of the regular "Attack" message, when dealing with melee weapons).</summary>
		public Message<bool> MeleeAttack { get { return m_MeleeAttack; } }

		/// <summary></summary>
		public Message Miss { get { return m_Miss; } }

		/// <summary></summary>
		public Message Hit { get { return m_Hit; } }

		/// <summary></summary>
		public float MaxReach { get { return m_MaxReach; } }

		/// <summary></summary>
		public float DamagePerHit { get { return m_DamagePerHit; } }

		[Header("Melee Setup")]

		[SerializeField]
		[Tooltip("The animation event handler - that picks up animation events from Mecanim.")]
		private FPMeleeEventHandler m_EventHandler;

		[Header("Melee Settings")]

		[SerializeField]
		[Tooltip("From how far can this object hit stuff?")]
		private float m_MaxReach = 0.5f;

		[SerializeField]
		[Tooltip("Useful for limiting the number of hits you can do in a period of time.")]
		private float m_TimeBetweenAttacks = 0.85f;

		[SerializeField]
		[Range(0f, 1000f)]
		private float m_DamagePerHit = 15f;

		[Range(0f, 1000f)]
		[SerializeField]
		private float m_ImpactForce = 15f;

		[Header("Audio")]

		[SerializeField]
		private AudioSource m_AudioSource;

		[SerializeField]
		private SoundPlayer m_AttackAudio;

		[SerializeField]
		private SoundType m_SoundType;

		private Message<bool> m_MeleeAttack = new Message<bool>();
		private Message m_Miss = new Message();
		private Message m_Hit = new Message();
		private float m_NextUseTime;


		public override bool TryAttackOnce(Camera camera)
		{
			if(Time.time < m_NextUseTime)
				return false;
					
			var raycastData = Player.RaycastData.Get();

			// Send the melee specific attack message.
			if(raycastData)
			{
				bool objectIsClose = raycastData.HitInfo.distance < m_MaxReach;
				MeleeAttack.Send(objectIsClose);
			}
			else
				MeleeAttack.Send(false);

			// Send the regular attack message.
			Attack.Send();

			m_NextUseTime = Time.time + m_TimeBetweenAttacks;

			return true;
		}

		public override bool TryAttackContinuously(Camera camera)
		{
			return TryAttackOnce(camera);
		}

		protected virtual void Start()
		{
			m_EventHandler.Hit.AddListener(On_Hit);
			m_EventHandler.Woosh.AddListener(On_Woosh);
		}

		protected virtual void On_Hit()
		{
			var raycastData = Player.RaycastData.Get();

			if(!raycastData)
				return;

			if(GameController.SurfaceDatabase)
			{
				var data = GameController.SurfaceDatabase.GetSurfaceData(raycastData.HitInfo);
				data.PlaySound(ItemSelectionMethod.Randomly, m_SoundType, 1f, raycastData.HitInfo.point);

				if(m_SoundType == SoundType.Hit)
					data.CreateHitFX(raycastData.HitInfo.point, Quaternion.LookRotation(raycastData.HitInfo.normal));
				else if(m_SoundType == SoundType.Chop)
					data.CreateChopFX(raycastData.HitInfo.point, Quaternion.LookRotation(raycastData.HitInfo.normal));
			}

			var damageable = raycastData.GameObject.GetComponent<IDamageable>();
			if(damageable != null)
			{
				var damageData = new HealthEventData(-m_DamagePerHit, Player, transform.position, GameController.WorldCamera.transform.forward, m_ImpactForce);
				damageable.ReceiveDamage(damageData);
			}

			// Lower the durability...
			if(m_Durability != null)
			{
				var value = m_Durability.Float;
				value.Current --;
				m_Durability.SetValue(ItemProperty.Type.Float, value);

				if(value.Current == 0)
					Player.DestroyEquippedItem.Try();
			}

			Hit.Send();
		}

		private void On_Woosh()
		{
			m_AttackAudio.Play(ItemSelectionMethod.Randomly, m_AudioSource, 0.6f);
			Miss.Send();
		}
	}
}
