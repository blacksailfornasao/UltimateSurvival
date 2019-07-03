using UnityEngine;

namespace UltimateSurvival.AI.Actions
{
    public class Eat : Action
    {
		[SerializeField]
		[Tooltip("Determines the time it will take for the AI to eat.")]
		private float m_EatTime;

        private bool m_IsDoneEating;
        private float m_EatStartTime;


        public override void OnStart(AIBrain brain)
        {
            m_Priority = 8;
            m_IsInterruptable = true;
            m_RepeatType = ET.ActionRepeatType.Single;

            Preconditions.Add(HelpStrings.AI.NEXT_TO_FOOD, true);

			Effects.Add(HelpStrings.AI.IS_HUNGRY, false);
        }

        public override void Activate(AIBrain brain)
        {
            ResetValues();

			brain.Settings.Animation.ToggleBool(HelpStrings.AI.ANIMATOR_PARAM_EAT, true);

            m_EatStartTime = Time.time;
        }

        public override void OnUpdate(AIBrain brain)
        {
            if(Time.time - m_EatStartTime > m_EatTime)
            {
				// The information Vitals has is never getting passed to the WorldState.
                brain.Settings.Vitals.LastTimeFed = Time.time;

				// So the AI will not know when it's hungry again.
                brain.Settings.Vitals.IsHungry = false;

                m_IsDoneEating = true;

				brain.Settings.Animation.ToggleBool(HelpStrings.AI.ANIMATOR_PARAM_EAT, false);
            }
        }

		public override void OnDeactivation (AIBrain brain)
		{
			brain.Settings.Animation.ToggleBool(HelpStrings.AI.ANIMATOR_PARAM_EAT, false);
		}

        public override bool IsDone(AIBrain brain) 
		{ 
			return m_IsDoneEating; 
		}

        public override void ResetValues()
        {
            m_IsDoneEating = false;
            m_EatStartTime = 0;
        }
    }
}