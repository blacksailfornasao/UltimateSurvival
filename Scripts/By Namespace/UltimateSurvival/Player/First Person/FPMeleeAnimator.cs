using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class FPMeleeAnimator : FPAnimator 
	{
		[Header("Melee")]

		[SerializeField]
		private float m_MissSpeed = 2f;

		[SerializeField]
		private float m_HitSpeed = 1.5f;

		[SerializeField]
		private WeaponShake m_MeleeWooshShake;

		[SerializeField]
		private WeaponShake m_MeleeHitShake;

		private FPMelee m_Melee;


		protected override void Awake()
		{
			base.Awake();

			if(FPObject as FPMelee)
			{
				m_Melee = FPObject as FPMelee;

				m_Melee.MeleeAttack.AddListener(On_MeleeAttack);
				m_Melee.Miss.AddListener(On_MeleeWoosh);
				m_Melee.Hit.AddListener(On_MeleeHit);

				Animator.SetFloat("Miss Speed", m_MissSpeed);
				Animator.SetFloat("Hit Speed", m_HitSpeed);
			}
			else
				Debug.LogError("The animator is of type Melee, but no Melee script found on this game object!", this);
		}

		protected override void OnValidate()
		{
			base.OnValidate();

			if(FPObject && FPObject.IsEnabled && Animator)
			{
				Animator.SetFloat("Hit Speed", m_HitSpeed);
				Animator.SetFloat("Miss Speed", m_MissSpeed);
			}
		}

		private void On_MeleeAttack(bool hitObject)
		{
			if(!hitObject)
				Animator.SetTrigger("Miss");
			else
				Animator.SetTrigger("Hit");
		}

		private void On_MeleeWoosh()
		{
			var raycastData = Player.RaycastData.Get();
			if(!raycastData || raycastData.HitInfo.distance > m_Melee.MaxReach)
				m_MeleeWooshShake.Shake();
		}

		private void On_MeleeHit()
		{
			m_MeleeHitShake.Shake();	
		}
	}
}
