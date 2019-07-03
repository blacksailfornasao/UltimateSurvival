using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival.AI
{
    [System.Serializable]
    public class EntityVitals
    {
        public bool IsHungry { get { return m_IsHungry; } set { m_IsHungry = value; } }

        public float LastTimeFed { get { return m_LastTimeFed; } set { m_LastTimeFed = value; } }

        [SerializeField]
        private float m_HungerRegenerationTime;

        private float m_LastTimeFed;
        private bool m_IsHungry;


        public void Update(AIBrain brain)
        {
            if (!m_IsHungry)
            {
                if (Time.time - m_LastTimeFed > m_HungerRegenerationTime)
                    m_IsHungry = true;
            }

			StateData.OverrideValue(HelpStrings.AI.IS_HUNGRY, m_IsHungry, brain.WorldState);
        }
    }
}