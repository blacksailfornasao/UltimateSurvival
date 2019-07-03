using UltimateSurvival.AI.Actions;

namespace UltimateSurvival.AI
{
    public class ActionTemplate : Action, Template
    {
        public override void OnStart(AIBrain brain)
        {
            
        }

        public override bool CanActivate(AIBrain brain)
        {
            return false;
        }

        public override void Activate(AIBrain brain)
        {
            
        }

        public override void OnUpdate(AIBrain brain)
        {
            
        }

        public override bool IsDone(AIBrain brain)
        {
            return false;
        }

        public override void ResetValues()
        {
            
        }
    }
}