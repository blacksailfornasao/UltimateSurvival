using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival.AI
{
    [System.Serializable]
    public class EntityDetection
    {
        public int ViewRadius { get { return m_ViewRadius; } }

        public int ViewAngle { get { return m_ViewAngle; } }

		public int HearRange { get { return m_HearRange; } }

		public GameObject LastChasedTarget { get; set; }

        public List<GameObject> VisibleTargets { get { return m_VisibleTargets; } }

        public List<GameObject> StillInRangeTargets { get { return m_StillInRangeTargets; } }

        [SerializeField]
        [Tooltip("Time it takes to look for targets again.")]
        private float m_TargetSearchDelay;

		[SerializeField]
		private Transform m_Eyes;

        [SerializeField]
        [Range(0, 360)]
        [Tooltip("Angle at which it can spot a player.")]
        private int m_ViewAngle = 120;

        [SerializeField]
        [Tooltip("Radius around the AI at which it can spot a player")]
        private int m_ViewRadius = 3;

		[SerializeField]
		[Clamp(0, 300)]
		private int m_HearRange = 50;

        [SerializeField]
        [Tooltip("Used for finding only specific types of targets.")]
        private LayerMask m_SpotMask;

        [SerializeField]
        [Tooltip("Used to know what objects can be blocking our view from the AI.")]
        private LayerMask m_ObstacleMask;

		// The targets the AI is currently seeing.
        private List<GameObject> m_VisibleTargets = new List<GameObject>();

        private List<GameObject> m_StillInRangeTargets = new List<GameObject>();

        private Transform m_Transform;
        private float m_LastTargetFindTime;


        public void Initialize(Transform transform) 
		{ 
			m_Transform = transform; 
		}

        public void Update(AIBrain brain)
        {
            if (m_TargetSearchDelay == 0)
                return;

            if (Time.time - m_LastTargetFindTime >= m_TargetSearchDelay)
            {
                m_VisibleTargets = GetVisibleTargets();
                GetTargetsStillInRange();

                m_LastTargetFindTime = Time.time;

				StateData.OverrideValue(HelpStrings.AI.IS_PLAYER_IN_SIGHT, HasTarget(), brain.WorldState);
            }
        }

		/// <summary>
		/// Used to check if the AI has a target currently.
		/// </summary>
		/// <returns></returns>
		public bool HasTarget() 
		{
			return m_StillInRangeTargets.Count > 0;
		}

		public bool HasVisibleTarget()
		{
			return m_VisibleTargets.Count > 0;
		}

		public Transform GetRandomTarget() 
		{ 
			return m_StillInRangeTargets[Random.Range(0, m_StillInRangeTargets.Count)].transform; 
		}

        private void GetTargetsStillInRange()
        {
            Collider[] collsInRange = GetCollidersInRange();

            for (int i = 0; i < collsInRange.Length; i++)
            {
                if (m_VisibleTargets.Contains(collsInRange[i].gameObject) && !m_StillInRangeTargets.Contains(collsInRange[i].gameObject))
                    m_StillInRangeTargets.Add(collsInRange[i].gameObject);
            }

            for (int x = 0; x < m_StillInRangeTargets.Count; x++)
            {
                bool foundEqual = false;

                for (int y = 0; y < collsInRange.Length; y++)
                {
                    if (collsInRange[y].gameObject == m_StillInRangeTargets[x])
                        foundEqual = true;
                }

                if (!foundEqual)
                    m_StillInRangeTargets.Remove(m_StillInRangeTargets[x]);
            }
        }

        private List<GameObject> GetVisibleTargets()
        {
            List<GameObject> targets = new List<GameObject>();
            Collider[] targetsColliders = GetCollidersInRange();

			// Loop trough the targets we found.
            for (int i = 0; i < targetsColliders.Length; i++)
            {
				if(targetsColliders[i].GetComponent<PlayerEventHandler>() == null)
					continue;

                Transform target = targetsColliders[i].transform;
				Vector3 targetPos = target.position + Vector3.up;

				// Get the direction to the target.
				Vector3 dirToTarget = (targetPos - m_Eyes.position).normalized;

				// Check if it's inside the viewAngle of the agent.
				if (Vector3.Angle(m_Eyes.forward, dirToTarget) < m_ViewAngle / 2)
                {
					// Check the distance.
					float distToTarget = Vector3.Distance(m_Eyes.position, targetPos);
			
					// See if something is blocking the view to the target.
					if(!Physics.Raycast(m_Eyes.position, dirToTarget, distToTarget, m_ObstacleMask))
                        targets.Add(target.gameObject);
                }
            }

            return targets;
        }

        private Collider[] GetCollidersInRange() 
		{
			return Physics.OverlapSphere(m_Transform.position, m_ViewRadius, m_SpotMask); 
		}
    }
}