using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateSurvival;

[RequireComponent(typeof(Animator))]
public class AIAnimator : EntityBehaviour 
{
	private Animator m_Animator;


	private void Start()
	{
		Entity.ChangeHealth.AddListener(OnAttempt_HealthChange);
		m_Animator = GetComponent<Animator>();
	}

	private void OnAttempt_HealthChange(HealthEventData data)
	{
		if(data.Delta < 0f)
		{
			float weight = Mathf.Abs(data.Delta) / 100f;
			m_Animator.SetLayerWeight(1, weight);

			m_Animator.SetTrigger("Get Hit");
		}
	}
}
