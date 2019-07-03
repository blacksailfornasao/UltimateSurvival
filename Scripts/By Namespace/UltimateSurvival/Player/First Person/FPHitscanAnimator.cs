using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class FPHitscanAnimator : FPAnimator 
	{
		[Header("Hitscan")]

		[SerializeField]
		private float m_FireSpeed = 1f;

		[SerializeField]
		[Clamp(0, 10)]
		private int m_FireTypesCount = 1;

		[SerializeField]
		private WeaponShake m_FireShake;

		private FPHitscan m_Hitscan;


		protected override void Awake()
		{
			base.Awake();

			Player.Aim.AddStartListener(OnStart_Aim);
			Player.Aim.AddStopListener(OnStop_Aim);

			if(FPObject as FPHitscan)
			{
				m_Hitscan = FPObject as FPHitscan;
				
				m_Hitscan.Attack.AddListener(On_GunFired);
				Animator.SetFloat("Fire Speed", m_FireSpeed);
			}
			else
				Debug.LogError("The animator is of type Hitscan, but no Hitscan script found on this game object!", this);
		}

		protected override void OnValidate()
		{
			base.OnValidate();

			if(FPObject && FPObject.IsEnabled && Animator)
				Animator.SetFloat("Fire Speed", m_FireSpeed);
		}

		private void OnStart_Aim()
		{
			if(FPObject.IsEnabled)
				Animator.Play("Hold Pose", 0, 0f);
		}

		private void OnStop_Aim()
		{
			if(FPObject.IsEnabled)
				Animator.Play("Idle", 0, 0f);
		}

		private void On_GunFired()
		{
			Animator.SetFloat("Fire Type", Random.Range(0, m_FireTypesCount));
			Animator.SetTrigger("Fire");
			m_FireShake.Shake();
		}
	}
}
