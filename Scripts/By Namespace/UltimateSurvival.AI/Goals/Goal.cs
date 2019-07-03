using UnityEngine;

namespace UltimateSurvival.AI.Goals
{
    public class Goal : ScriptableObject
    {
        public float Priority { get { return m_Priority; } }

		public StateData GoalState { get { return m_GoalState; } }

        [SerializeField] [ShowOnly]
        protected float m_Priority;

        [SerializeField]
        protected Vector2 m_PriorityRange;

        private StateData m_GoalState = new StateData();

        public virtual void OnStart() { }

        public virtual void RecalculatePriority(AIBrain brain) { }
    }
}