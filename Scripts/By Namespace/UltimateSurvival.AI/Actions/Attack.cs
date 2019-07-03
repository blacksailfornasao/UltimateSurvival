using UnityEngine;

namespace UltimateSurvival.AI.Actions
{
    public class Attack : Action
    {
		[SerializeField]
		private float m_MinAttackDistance = 1.5f;

		[SerializeField]
		private float m_AttackInterval = 0.8f;

		private bool m_HasKilledPlayer = false;
		private GameObject m_Target;
		private float m_NextTimeCanAttack;


        public override void OnStart(AIBrain brain)
        {
            m_Priority = 10;
            m_RepeatType = ET.ActionRepeatType.Single;
            m_IsInterruptable = false;

			Preconditions.Add(HelpStrings.AI.CAN_ATTACK_PLAYER, true);

			Effects.Add(HelpStrings.AI.IS_PLAYER_DEAD, true);
        }

        public override bool CanActivate(AIBrain brain) 
		{ 
			return brain.Settings.Detection.HasTarget(); 
		}

        public override void Activate(AIBrain brain)
        {
			m_Target = brain.Settings.Detection.LastChasedTarget;
			//brain.AManager.Movement.MoveTo(m_Target.transform.position);
        }

		public override void OnUpdate (AIBrain brain)
		{
			if(Time.time > m_NextTimeCanAttack)
			{
				brain.Settings.Animation.SetTrigger(HelpStrings.AI.ANIMATOR_PARAM_ATTACK);
				m_NextTimeCanAttack = Time.time + m_AttackInterval;
			}

			RotateTowards(brain.transform, m_Target.transform, 5f);
		}

        public override bool StillValid(AIBrain brain) 
		{ 
			bool targetIsClose = Vector3.Distance(m_Target.transform.position, brain.transform.position) < m_MinAttackDistance;
			bool attackIsDone = Time.time > m_NextTimeCanAttack;

			return (brain.Settings.Detection.HasTarget() && targetIsClose) || !attackIsDone;
		}

        public override bool IsDone(AIBrain brain) 
		{ 
			return m_HasKilledPlayer && Time.time > m_NextTimeCanAttack;
		}

		private void RotateTowards(Transform transform, Transform target, float rotationSpeed)
		{
			Vector3 direction = (target.position - transform.position).normalized;
			Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
			transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
		}
    }
}