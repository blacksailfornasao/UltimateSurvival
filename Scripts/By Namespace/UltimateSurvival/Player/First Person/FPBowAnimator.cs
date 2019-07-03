using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class FPBowAnimator : FPAnimator 
	{
		[Header("Bow")]

		[SerializeField]
		private float m_ReleaseSpeed = 1f;

		[SerializeField]
		private WeaponShake m_ReleaseShake;

		private FPBow m_Bow;


		protected override void Awake()
		{
			base.Awake();

			Player.Aim.AddStartListener(OnStart_Aim);
			Player.Aim.AddStopListener(OnStop_Aim);

			if(FPObject as FPBow)
			{
				m_Bow = FPObject as FPBow;

				m_Bow.Attack.AddListener(On_Release);
				Animator.SetFloat("Release Speed", m_ReleaseSpeed);
			}
			else
				Debug.LogError("The animator is of type Bow, but no Bow script found on this game object!", this);
		}

		protected override void OnValidate()
		{
			base.OnValidate();

			if(FPObject && FPObject.IsEnabled && Animator)
				Animator.SetFloat("Release Speed", m_ReleaseSpeed);
		}

		private void OnStart_Aim()
		{
			if(FPObject.IsEnabled)
				Animator.SetBool("Aim", true);
		}

		private void OnStop_Aim()
		{
			if(FPObject.IsEnabled)
				Animator.SetBool("Aim", false);
		}

		private void On_Release()
		{
			Animator.SetBool("Aim", false); 
			Animator.SetTrigger("Release");

			m_ReleaseShake.Shake();
		}
	}
}
