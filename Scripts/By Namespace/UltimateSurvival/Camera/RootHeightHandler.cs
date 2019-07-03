using System.Collections;
using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class RootHeightHandler : PlayerBehaviour  
	{
		[SerializeField]
		[Clamp(-2f, 0f)]
		private float m_CrouchOffset = -1f;

		[SerializeField]
		private float m_CrouchSpeed = 5f;

		private float m_CurrentOffsetOnY;
		private float m_InitialHeight;


		private void Start()
		{
			Player.Crouch.AddStartListener(OnStart_Crouch);
			Player.Crouch.AddStopListener(OnStop_Crouch);
			m_InitialHeight = transform.localPosition.y;
		}

		private void OnStart_Crouch()
		{
			StopAllCoroutines();
			StartCoroutine(SetOffsetOnY(m_CrouchOffset));
		}

		private void OnStop_Crouch()
		{
			StopAllCoroutines();
			StartCoroutine(SetOffsetOnY(0f));
		}

		private IEnumerator SetOffsetOnY(float targetY)
		{
			WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

			while(Mathf.Abs(targetY - m_CurrentOffsetOnY) > Mathf.Epsilon)
			{
				m_CurrentOffsetOnY = Mathf.MoveTowards(m_CurrentOffsetOnY, targetY, Time.deltaTime * m_CrouchSpeed);
				transform.localPosition = Vector3.up * (m_CurrentOffsetOnY + m_InitialHeight);

				yield return waitForFixedUpdate;	
			}
		}
	}
}
