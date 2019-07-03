using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UltimateSurvival.AI.Actions;
using UltimateSurvival.AI.Goals;

namespace UltimateSurvival.AI
{
    [RequireComponent(typeof(AISettings))]
	public class AIBrain : AIBehaviour
    {
       	public List<Action> AvailableActions { get { return m_AvailableActions; } }

     	public Action Fallback { get { return m_Fallback; } }

        public Queue<Action> ActionQueue { get { return m_ActionQueue; } }

        public Goal CurrentGoal { get { return m_CurrentGoal; } }

		public StateData WorldState { get { return m_WorldState; } }

		public AISettings Settings { get { return m_Settings; } }

		[SerializeField]
		private List<Action> m_AvailableActions;

		[SerializeField]
		private List<Goal> m_AvailableGoals;

        [SerializeField]
		private Action m_Fallback;

        [SerializeField]
        private float m_MinPlanInterval;

        [SerializeField]
        private float m_MinGoalPriorityCheckInterval;

        private Queue<Action> m_ActionQueue;
        private Planner m_Planner;
        private Goal m_CurrentGoal;

        private StateData m_WorldState;
        private AISettings m_Settings;

        private float m_LastPlanTime;
        private float m_LastGoalPriorityCheckTime;


        private void Start()
        {
			// HACK:
			m_Fallback = Instantiate(m_Fallback) as Action;

			// HACK:
			for(int i = 0;i < m_AvailableActions.Count;i ++)
				m_AvailableActions[i] = Instantiate(m_AvailableActions[i]) as Action;

			// HACK:
			for(int i = 0;i < m_AvailableGoals.Count;i ++)
				m_AvailableGoals[i] = Instantiate(m_AvailableGoals[i]) as Goal;

            m_Planner = new Planner();
            m_Settings = GetComponent<AISettings>();

            CreateNewWorldState();
            InitializeData();
        }

        private void Update()
        {
            bool replanByGoals = IsReplanNeededBecauseOfGoals();
            bool queueNull = (m_ActionQueue == null || m_ActionQueue.Count == 0);

            if (queueNull || replanByGoals)
                Replan();

            if (!queueNull)
            {
                Action currentAction = m_ActionQueue.Peek();
                bool canPlan = (Time.time - m_LastPlanTime > m_MinPlanInterval && currentAction.IsInterruptable);

                if (m_ActionQueue.Count > 1 && canPlan)
                {
                    Action secondAction = m_ActionQueue.ToArray()[1];
                    bool isHigherPriority = (secondAction.Priority > currentAction.Priority);
                    bool canBeActivated = (secondAction.CanActivate(this));

                    if (isHigherPriority && canBeActivated)
                    {
                        m_LastPlanTime = Time.time;

                        m_ActionQueue.Dequeue();
                        currentAction = m_ActionQueue.Peek();
                    }
                }

                if (currentAction.IsActive)
                {
                    if (!currentAction.StillValid(this))
                    {
                        currentAction.IsActive = false;

                        m_ActionQueue.Clear();

                        return;
                    }

                    currentAction.OnUpdate(this);

                    if (currentAction.IsDone(this))
                    {
                        currentAction.OnCompletion(this);
                        currentAction.IsActive = false;

                        m_ActionQueue.Dequeue();
                    }
                }
                else if (currentAction.IsActive == false)
                {
                    if (!currentAction.CanActivate(this))
                    {
                        m_ActionQueue.Clear();

                        return;
                    }

                    currentAction.Activate(this);

                    currentAction.IsActive = true;
                }
            }
        }

        private bool IsReplanNeededBecauseOfGoals()
        {
            if (m_AvailableGoals == null || m_AvailableGoals.Count == 0)
                return false;

            if (m_ActionQueue == null || m_ActionQueue.Count == 0)
                return false;

            bool replan = false;

           if (Time.time - m_LastGoalPriorityCheckTime > m_MinGoalPriorityCheckInterval)
            {
                m_LastGoalPriorityCheckTime = Time.time;

                for (int i = 0; i < m_AvailableGoals.Count; i++)
                    m_AvailableGoals[i].RecalculatePriority(this);

                m_AvailableGoals = m_AvailableGoals.OrderByDescending(x => x.Priority).ToList();

                if (m_AvailableGoals[0] != CurrentGoal)
                {
                    m_ActionQueue.Peek().OnDeactivation(this);
			
                    replan = true;
                }
            }

            return replan;
        }

        private void Replan()
        {
            for (int i = 0; i < m_AvailableActions.Count; i++)
                m_AvailableActions[i].IsActive = false;

            bool plan = m_Planner.Plan(m_AvailableGoals, m_AvailableActions, this, out m_ActionQueue, out m_CurrentGoal);

            if (!plan)
            {
                FallBack();

                return;
            }
            else
            {
                m_Fallback.OnDeactivation(this);

                m_Fallback.IsActive = false;
            }

            m_LastPlanTime = Time.time;
        }

        private void FallBack()
        {
           if (m_ActionQueue == null || m_ActionQueue.Peek() != m_Fallback)
            {
                m_ActionQueue = new Queue<Action>();
                m_ActionQueue.Enqueue(m_Fallback);

                m_Fallback.Activate(this);
                m_Fallback.IsActive = true;
            }
        }

        /// <summary>
        /// We create the state with wich all AIs start with. This is all they know about the world.
        /// </summary>
        private void CreateNewWorldState()
        {
            m_WorldState = new StateData();
			m_WorldState.Add(HelpStrings.AI.IS_PLAYER_DEAD, false);
			m_WorldState.Add(HelpStrings.AI.CAN_ATTACK_PLAYER, false);
			m_WorldState.Add(HelpStrings.AI.IS_PLAYER_IN_SIGHT, false);
			m_WorldState.Add(HelpStrings.AI.NEXT_TO_FOOD, false);
			m_WorldState.Add(HelpStrings.AI.IS_HUNGRY, false);
        }

        private void InitializeData()
        {
           	if (m_AvailableActions.Count == 0)
            {
				Debug.LogError("No actions set for " + gameObject.name + " entity", this);
                return;
            }

            if (m_AvailableGoals.Count == 0)
            {
				Debug.LogError("No goals set for " + gameObject.name + " entity", this);
                return;
            }

            for (int i = 0; i < m_AvailableActions.Count; i++)
            {
                m_AvailableActions[i].OnStart(this);

                m_AvailableActions[i].IsActive = false;//Make sure all actions are inactive at start.     
            }

            for (int x = 0; x < m_AvailableGoals.Count; x++)
                m_AvailableGoals[x].OnStart();

            m_Fallback.OnStart(this);
            m_Fallback.IsActive = false;
        }
    }
}