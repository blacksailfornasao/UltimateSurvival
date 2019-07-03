using System.Collections.Generic;
using System.Linq;
using UltimateSurvival.AI.Actions;
using UltimateSurvival.AI.Goals;

namespace UltimateSurvival.AI
{
    public class Planner
    {
        public bool Plan(List<Goal> availableGoals, List<Action> availableActions, AIBrain brain, out Queue<Action> plan, out Goal selected)
        {
            bool planSuccessfull = false;

            plan = new Queue<Action>();
            selected = null;

            List<Action> actions = new List<Action>();
            actions = actions.CopyOther(availableActions);

            List<Goal> goals = new List<Goal>();
            goals = goals.CopyOther(availableGoals);

            goals = goals.OrderByDescending(x => x.Priority).ToList();

            for (int i = 0; i < goals.Count; i++)
            {
                Goal currentGoal = goals[i];

                List<Action> actionList = new List<Action>();
                FindActionThatMatchesState(currentGoal.GoalState, actions, actionList);

                if (actionList.Count > 0)
                {
                    bool canStartPlan = false;

                    for (int x = actionList.Count - 1; x >= 0; x--)
                        plan.Enqueue(actionList[x]);

                    canStartPlan = plan.Peek().CanActivate(brain);

                    if (canStartPlan)
                    {
                        selected = currentGoal;
                        planSuccessfull = true;
                        break;
                    }
                }
            }

            return planSuccessfull;
        }

        private void FindActionThatMatchesState(StateData goalState, List<Action> actions, List<Action> related)
        {
            for (int i = 0; i < actions.Count; i++)
            {
                StateData actionEffectData = actions[i].Effects;
                StateData actionPreconditionData = actions[i].Preconditions;

                foreach (var goalPair in goalState.m_Dictionary)
                {
                    foreach (var actionPair in actionEffectData.m_Dictionary)
                    {
                        bool keysMatch = (actionPair.Key == goalPair.Key);
                        bool valuesMatch = (actionPair.Value.ToString() == goalPair.Value.ToString());

                        if (keysMatch && valuesMatch)
                        {
                            related.Add(actions[i]);
                            actions.Remove(actions[i]);

                            FindActionThatMatchesState(actionPreconditionData, actions, related);
                        }
                    }
                }
            }
        }
    }
}