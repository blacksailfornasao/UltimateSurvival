using UnityEngine;

namespace UltimateSurvival.AI.Actions
{
    public class Flee : Action
    {
		[SerializeField]
		[Clamp(0f, 100f)]
		private float m_MinFleeDistance = 3f;

		[SerializeField]
		private SoundPlayer m_ScreamAudio;

		[SerializeField]
		[Clamp(0f, 15f)]
		private float m_ScreamInterval = 3f;

        private Vector3 m_FleePosition;
		private float m_LastTimeScreamed;


        public override void OnStart(AIBrain brain)
        {
            m_Priority = 12;
            m_IsInterruptable = true;
            m_RepeatType = ET.ActionRepeatType.Single;

			Preconditions.Add(HelpStrings.AI.IS_PLAYER_IN_SIGHT, true);

			Effects.Add(HelpStrings.AI.IS_PLAYER_IN_SIGHT, false);
        }

        public override bool CanActivate(AIBrain brain) 
		{
			if(brain.Settings.Detection.HasTarget())
			{
				Vector3 aiPos = brain.transform.position;
				Vector3 tPos = brain.Settings.Detection.GetRandomTarget().transform.position;

				return (aiPos - tPos).magnitude < m_MinFleeDistance;
			}

			return false;
		}

        public override void Activate(AIBrain brain)
        {
           
        }

        public override void OnUpdate(AIBrain brain) 
		{
			Vector3 aiPos = brain.transform.position;
			Vector3 tPos = brain.Settings.Detection.GetRandomTarget().transform.position;

			m_FleePosition = aiPos - tPos;
			m_FleePosition += brain.transform.position;

			brain.Settings.Movement.MoveTo(m_FleePosition, true);

			if(Time.time > m_LastTimeScreamed + m_ScreamInterval)
			{
				m_ScreamAudio.Play(ItemSelectionMethod.RandomlyButExcludeLast, brain.Settings.AudioSource, 1f);
				m_LastTimeScreamed = Time.time;
			}
		}

		public override void OnDeactivation(AIBrain brain)
		{
			brain.Settings.Animation.ToggleBool("Run", false);
		}

        public override bool StillValid(AIBrain brain) 
		{
			return brain.Settings.Detection.HasTarget();
		}

        public override bool IsDone(AIBrain brain)
		{
			return !brain.Settings.Detection.HasTarget();
		}
    }
}