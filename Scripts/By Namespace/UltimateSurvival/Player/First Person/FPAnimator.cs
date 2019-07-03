using System.Collections;
using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	[RequireComponent(typeof(FPObject))]
	public class FPAnimator : PlayerBehaviour 
	{
		/// <summary>
		/// 
		/// </summary>
		public enum ObjectType
		{
			Normal,
			Melee,
			Throwable,
			Hitscan,
			Bow
		}

		public Animator Animator { get { return m_Animator; } }
		public FPObject FPObject { get { return m_Object; } }

		[SerializeField]
		private Animator m_Animator;

		[Header("General")]

		[SerializeField]
		private float m_DrawSpeed = 1f;

		[SerializeField]
		private float m_HolsterSpeed = 1f;

		private FPObject m_Object;

		private bool m_Initialized;


		protected virtual void Awake()
		{
			m_Object = GetComponent<FPObject>();

			m_Object.Draw.AddListener(On_Draw);
			m_Object.Holster.AddListener(On_Holster);
			Player.Sleep.AddStopListener(OnStop_Sleep);
			Player.Respawn.AddListener(On_Respawn);

			m_Animator.SetFloat("Draw Speed", m_DrawSpeed);
			m_Animator.SetFloat("Holster Speed", m_HolsterSpeed);
		}

		protected virtual void OnValidate()
		{
			if(FPObject && FPObject.IsEnabled && Animator)
			{
				m_Animator.SetFloat("Draw Speed", m_DrawSpeed);
				m_Animator.SetFloat("Holster Speed", m_HolsterSpeed);
			}
		}
	
		private void On_Draw()
		{
			OnValidate();

			if(m_Animator)
				m_Animator.SetTrigger("Draw");
		}

		private void On_Holster()
		{
			if(m_Animator)
				m_Animator.SetTrigger("Holster");
		}

		private void OnStop_Sleep()
		{
			if(FPObject.IsEnabled)
				OnValidate();
		}

		private void On_Respawn()
		{
			if(FPObject.IsEnabled)
				OnValidate();
		}
	}
}
