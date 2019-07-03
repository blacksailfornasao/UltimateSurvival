namespace UltimateSurvival.AI.Goals
{
    public class AvoidPlayer : Goal
    {
		public override void OnStart() 
		{ 
			GoalState.Add(HelpStrings.AI.IS_PLAYER_IN_SIGHT, false); 
		}

        public override void RecalculatePriority(AIBrain brain)
        {
			if(brain.Settings.Detection.HasTarget())
                m_Priority = m_PriorityRange.y;
            else
                m_Priority = m_PriorityRange.x;
        }
    }
}