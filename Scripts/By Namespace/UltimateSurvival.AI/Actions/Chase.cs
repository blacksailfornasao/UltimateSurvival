using UnityEngine;

namespace UltimateSurvival.AI.Actions
{
    public class Chase : Action
    {
		[SerializeField]
		private SoundPlayer m_InitialScreamAudio;

		[SerializeField]
		private SoundPlayer m_ScreamingAudio;

		[SerializeField]
		private Vector2 m_ScreamInterval = new Vector2(0.7f, 1.2f);

        private Transform m_Target;
		private float m_NextScreamTime;


        public override void OnStart(AIBrain brain)
        {
            m_Priority = 1;
            m_IsInterruptable = false;
            m_RepeatType = ET.ActionRepeatType.Single;

            Preconditions.Add(HelpStrings.AI.IS_PLAYER_IN_SIGHT, true);

            Effects.Add(HelpStrings.AI.CAN_ATTACK_PLAYER, true);

            ResetValues();
        }

        public override bool CanActivate(AIBrain brain)
        {
			if (!brain.Settings.Detection.HasTarget())
                return false;

            m_Target = brain.Settings.Detection.GetRandomTarget();
			brain.Settings.Detection.LastChasedTarget = m_Target.gameObject;

			return true;
        }

        public override void Activate(AIBrain brain)
		{ 
			brain.Settings.Movement.MoveTo(m_Target.position, true);

			if(Vector3.Distance(brain.transform.position, m_Target.position) > 3.5f)
				m_InitialScreamAudio.Play(ItemSelectionMethod.RandomlyButExcludeLast, brain.Settings.AudioSource);
		}

        public override void OnUpdate(AIBrain brain)
		{
			brain.Settings.Movement.MoveTo(m_Target.position, true);

			var agent = brain.Settings.Movement.Agent;

			if(agent.velocity.sqrMagnitude < 0.01f)
			{
				brain.Settings.Animation.ToggleBool(HelpStrings.AI.ANIMATOR_PARAM_RUN, false);
				//Debug.Log("run false");
				agent.updateRotation = false;
				RotateTowards(agent.transform, m_Target, 5f);
				//agent.transform.rotation = Quaternion.Lerp(agent.transform.rotation, Quaternion.LookRotation(m_Target.position - agent.transform.position), Time.deltaTime * 8f);
			}
			else
			{
				agent.updateRotation = true;
				brain.Settings.Animation.ToggleBool(HelpStrings.AI.ANIMATOR_PARAM_RUN, true);
				//Debug.Log("run true");

				if(Time.time > m_NextScreamTime)
				{
					m_ScreamingAudio.Play(ItemSelectionMethod.RandomlyButExcludeLast, brain.Settings.AudioSource, 1f);
					m_NextScreamTime = Time.time + Random.Range(m_ScreamInterval.x, m_ScreamInterval.y);
				}
			}
		}

        public override bool StillValid(AIBrain brain)
		{
			var stillValid = brain.Settings.Detection.HasTarget();
			if(!stillValid)
			{
				brain.Settings.Animation.ToggleBool(HelpStrings.AI.ANIMATOR_PARAM_RUN, false);
				brain.Settings.Movement.Agent.updateRotation = true;
			}
			
			return stillValid;
		}

        public override bool IsDone(AIBrain brain)
		{
			var done = brain.Settings.Movement.ReachedDestination(true) && (brain.transform.position - m_Target.position).sqrMagnitude < brain.Settings.Movement.Agent.stoppingDistance * brain.Settings.Movement.Agent.stoppingDistance;

			return done; 
		}

        public override void ResetValues()
		{ 
			m_Target = null;
		}

		private void RotateTowards(Transform transform, Transform target, float rotationSpeed)
		{
			Vector3 direction = (target.position - transform.position).normalized;
			Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
			transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
		}
    }
}