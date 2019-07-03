using UnityEngine;

namespace UltimateSurvival
{
	public class CameraAnimator : PlayerBehaviour 
	{
		[SerializeField]
		private Animator m_Animator;


		private void Awake()
		{
			Player.Run.AddStartListener(OnStart_Run);
			Player.Run.AddStopListener(OnStop_Run);
		}

		private void OnStart_Run()
		{
			m_Animator.SetBool("Run", true);
		}

		private void OnStop_Run()
		{
			m_Animator.SetBool("Run", false);
		}
	}
}