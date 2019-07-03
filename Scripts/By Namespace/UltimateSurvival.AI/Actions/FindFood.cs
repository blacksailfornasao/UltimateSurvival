namespace UltimateSurvival.AI.Actions
{
    public class FindFood : PointBased
    {
        public override void OnStart(AIBrain brain)
        {
            m_Priority = 8;
            m_IsInterruptable = true;
            m_RepeatType = ET.ActionRepeatType.Single;

			Preconditions.Add(HelpStrings.AI.IS_HUNGRY, true);

			Effects.Add(HelpStrings.AI.NEXT_TO_FOOD, true);

            base.OnStart(brain);

            ChangePatrolPoint();
        }

        public override bool CanActivate(AIBrain brain) 
		{
			return (base.CanActivate(brain) && brain.Settings.Vitals.IsHungry);
		}
    }
}