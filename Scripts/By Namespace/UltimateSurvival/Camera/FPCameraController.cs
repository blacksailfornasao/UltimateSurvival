using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class FPCameraController : MonoSingleton<FPCameraController>
    {
		[Tooltip("The default position influence of all shakes created on the fly.")]
		[SerializeField]
		private Vector3 m_DefaultPosInfluence = new Vector3(0.15f, 0.15f, 0.15f);

		[Tooltip("The default rotation influence of all shakes created on the fly.")]
		[SerializeField]
		private Vector3 m_DefaultRotInfluence = new Vector3(1, 1, 1);

		[Header("Headbobs")]

		[SerializeField]
		private PlayerEventHandler m_Player;

		[SerializeField]
		private TrigonometricBob m_WalkHeadbob;

		[SerializeField]
		private TrigonometricBob m_RunHeadbob;

		[Header("Shakes")]

		[SerializeField]
		private GenericShake m_DamageShake;

		[SerializeField]
		private WeaponShake m_LandShake;

		[SerializeField]
		private float m_LandThreeshold = 3f;

		private Vector3 m_PositionAddShake, m_RotationAddShake;
		private Vector3 m_PositionAddBob;
		private Vector3 m_RotationAddBob;

		private List<ShakeInstance> m_ShakeInstances = new List<ShakeInstance>();


        public ShakeInstance Shake(ShakeInstance shake)
        {
            m_ShakeInstances.Add(shake);
            return shake;
        }
			
        public ShakeInstance ShakeOnce(float magnitude, float roughness, float fadeInTime, float fadeOutTime)
        {
            ShakeInstance shake = new ShakeInstance(magnitude, roughness, fadeInTime, fadeOutTime);
            shake.PositionInfluence = m_DefaultPosInfluence;
            shake.RotationInfluence = m_DefaultRotInfluence;
            m_ShakeInstances.Add(shake);

            return shake;
        }
			
        public ShakeInstance ShakeOnce(float magnitude, float roughness, float fadeInTime, float fadeOutTime, Vector3 posInfluence, Vector3 rotInfluence)
        {
            ShakeInstance shake = new ShakeInstance(magnitude, roughness, fadeInTime, fadeOutTime);
            shake.PositionInfluence = posInfluence;
            shake.RotationInfluence = rotInfluence;
            m_ShakeInstances.Add(shake);

            return shake;
        }
			
        public ShakeInstance StartShake(float magnitude, float roughness, float fadeInTime)
        {
            ShakeInstance shake = new ShakeInstance(magnitude, roughness);
            shake.PositionInfluence = m_DefaultPosInfluence;
            shake.RotationInfluence = m_DefaultRotInfluence;
            shake.StartFadeIn(fadeInTime);
            m_ShakeInstances.Add(shake);
            return shake;
        }
			
        public ShakeInstance StartShake(float magnitude, float roughness, float fadeInTime, Vector3 posInfluence, Vector3 rotInfluence)
        {
            ShakeInstance shake = new ShakeInstance(magnitude, roughness);

            shake.PositionInfluence = posInfluence;
            shake.RotationInfluence = rotInfluence;
            shake.StartFadeIn(fadeInTime);

            m_ShakeInstances.Add(shake);

            return shake;
        }

		private void Awake()
		{
			m_Player.ChangeHealth.AddListener(OnSuccess_PlayerHealthChanged);
			m_Player.Land.AddListener(On_PlayerLanded);
		}

		private void OnSuccess_PlayerHealthChanged(HealthEventData healthEventData)
		{
			if(healthEventData.Delta < 0f && healthEventData.Delta < -8f)
				m_DamageShake.Shake(Mathf.Abs(healthEventData.Delta / 100f));
		}

		private void On_PlayerLanded(float landSpeed)
		{
			if(landSpeed > m_LandThreeshold)
				m_LandShake.Shake();
		}

		private void LateUpdate()
		{
			m_PositionAddShake = Vector3.zero;
			m_RotationAddShake = Vector3.zero;

			for (int i = 0; i < m_ShakeInstances.Count; i++)
			{
				if (i >= m_ShakeInstances.Count)
					break;

				ShakeInstance c = m_ShakeInstances[i];

				if (c.CurrentState == ShakeState.Inactive && c.DeleteOnInactive)
				{
					m_ShakeInstances.RemoveAt(i);
					i--;
				}
				else if (c.CurrentState != ShakeState.Inactive)
				{
					m_PositionAddShake += Vector3.Scale(c.UpdateShake(), c.PositionInfluence);
					m_RotationAddShake += Vector3.Scale(c.UpdateShake(), c.RotationInfluence);
				}
			}

			float movementSpeed = m_Player.Velocity.Get().magnitude;

			// Update the walk headbob.
			if(m_Player.Walk.Active)
				m_PositionAddBob = m_WalkHeadbob.CalculateBob(movementSpeed, Time.deltaTime);
			else
				m_PositionAddBob = m_WalkHeadbob.Cooldown(Time.deltaTime);

			// Update the run headbob.
			if(m_Player.Run.Active)
				m_PositionAddBob += m_RunHeadbob.CalculateBob(movementSpeed, Time.deltaTime);
			else
				m_PositionAddBob += m_RunHeadbob.Cooldown(Time.deltaTime);

			transform.localPosition = m_PositionAddShake + m_PositionAddBob;
			transform.localEulerAngles = m_RotationAddShake;
		}
    }

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class WeaponShake
	{
		[Header("Shake")]
		[SerializeField]
		private float m_Magnitude = 15f;

		[SerializeField]
		private float m_Roughness = 3f;

		[SerializeField]
		private Vector3 m_PositionInfluence = new Vector3(0.01f, 0.01f, 0.01f);

		[SerializeField]
		private Vector3 m_RotationInfluence = new Vector3(0.8f, 0.5f, 0.5f);

		[SerializeField]
		private float m_FadeInTime = 0.2f;

		[SerializeField]
		private float m_FadeOutTime = 0.3f;

	
		/// <summary>
		/// 
		/// </summary>
		public void Shake()
		{
			FPCameraController.Instance.Shake(GetShakeInstance());
		}

		/// <summary>
		/// 
		/// </summary>
		private ShakeInstance GetShakeInstance()
		{
			var shake = new ShakeInstance(m_Magnitude, m_Roughness, m_FadeInTime, m_FadeOutTime);
			shake.PositionInfluence = m_PositionInfluence;
			shake.RotationInfluence = m_RotationInfluence;
			shake.IsWeapon = true;

			return shake;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class GenericShake
	{
		[Header("Shake")]

		[SerializeField]
		private float m_Magnitude = 15f;

		[SerializeField]
		private float m_MinMagnitudeScale = 0.5f;

		[SerializeField]
		private float m_Roughness = 3f;

		[SerializeField]
		private Vector3 m_PositionInfluence = new Vector3(0.01f, 0.01f, 0.01f);

		[SerializeField]
		private Vector3 m_RotationInfluence = new Vector3(0.8f, 0.5f, 0.5f);

		[SerializeField]
		private float m_FadeInTime = 0.2f;

		[SerializeField]
		private float m_FadeOutTime = 0.3f;


		/// <summary>
		/// 
		/// </summary>
		public void Shake(float scale)
		{
			FPCameraController.Instance.Shake(GetShakeInstance(scale));
		}

		/// <summary>
		/// 
		/// </summary>
		private ShakeInstance GetShakeInstance(float scale)
		{
			var shake = new ShakeInstance(m_Magnitude, m_Roughness, m_FadeInTime, m_FadeOutTime);
			shake.PositionInfluence = m_PositionInfluence;
			shake.RotationInfluence = m_RotationInfluence;
			shake.ScaleMagnitude *= Mathf.Max(scale, m_MinMagnitudeScale);

			return shake;
		}
	}
}