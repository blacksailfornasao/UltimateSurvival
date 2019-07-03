using UnityEngine;
using UnityEngine.AI;

namespace UltimateSurvival.AI
{
    [System.Serializable]
    public class EntityMovement
    {
        public Vector3 CurrentDestination { get { return m_CurrentDestination; } }

        public ET.AIMovementState MovementState { get { return m_MovementState; } }

        public NavMeshAgent Agent { get { return m_Agent; } }

        [SerializeField]
        [Tooltip("Normal speed the agent will use.")]
        private float m_WalkSpeed;

        [Tooltip("Speed the agent will only use whenever an action requires it to hurry.")]
        [SerializeField]
        private float m_RunSpeed;

        private Vector3 m_CurrentDestination;
        private ET.AIMovementState m_MovementState;

        private AIBrain m_Brain;
        private NavMeshAgent m_Agent;


        public void Initialize(AIBrain brain)
        {
            m_Brain = brain;

            m_Agent = m_Brain.GetComponent<NavMeshAgent>();
        }

        public void Update(Transform transform)
        {
            //Pulls the agent's position towards where the character wants to be.
            Vector3 worldDeltaPosition = m_Agent.nextPosition - transform.position;

            if (worldDeltaPosition.magnitude > m_Agent.radius)
                m_Agent.nextPosition = transform.position + 0.9f * worldDeltaPosition;
        }

        /// <summary>
        /// Function called from different actions that allows the movement of the AI from one point to another.
        /// </summary>
        /// <param name="position"></param>
		public NavMeshPath MoveTo(Vector3 position, bool fastMove = false)
        {
			var path = new NavMeshPath();

			//NavMesh.CalculatePath(m_Brain.transform.position, position, NavMesh.AllAreas, path);

			//m_Agent.SetPath(path);
			m_Agent.SetDestination(position);
			//Debug.Log(m_Agent.pathStatus);
			//Debug.Log(path.status);
			//We assign the current target.
            m_CurrentDestination = position;

			bool runExists = (fastMove && m_Brain.Settings.Animation.ParameterExists(HelpStrings.AI.ANIMATOR_PARAM_RUN));
			bool walkExists = (m_Brain.Settings.Animation.ParameterExists(HelpStrings.AI.ANIMATOR_PARAM_WALK));

            if (runExists)
				ChangeMovementState(m_RunSpeed, HelpStrings.AI.ANIMATOR_PARAM_RUN, true, ET.AIMovementState.Running);
            else if (walkExists)
				ChangeMovementState(m_WalkSpeed, HelpStrings.AI.ANIMATOR_PARAM_WALK, true, ET.AIMovementState.Walking);

			return path;
        }

        private void ChangeMovementState(float speed, string animName, bool animValue, ET.AIMovementState newState)
        {
            m_Agent.speed = speed;

            m_Brain.Settings.Animation.ToggleBool(animName, animValue);

            m_MovementState = newState;
        }

        /// <summary>
        /// Help function called from some actions that allows to check if the AI has already arrived it's current target point(m_CurrentDestination).
        /// </summary>
        /// <returns></returns>
        public bool ReachedDestination(bool isStop = true)
        {
            if (m_Agent.remainingDistance <= m_Agent.stoppingDistance)
            {
                if (isStop)
                {
					//Find out which animation we have to toggle off.
					string toggleOffAnim = (m_MovementState == ET.AIMovementState.Running) ? HelpStrings.AI.ANIMATOR_PARAM_RUN : HelpStrings.AI.ANIMATOR_PARAM_WALK;

                    m_Brain.Settings.Animation.ToggleBool(toggleOffAnim, false);

                    m_MovementState = ET.AIMovementState.Idle;
                }

                return true;
            }

            return false;
        }
    }
}