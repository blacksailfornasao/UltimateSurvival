using UnityEngine;

namespace UltimateSurvival.AI.Actions
{
	[System.Serializable]
    public class Patrol : PointBased
    {
		[SerializeField]
		private Vector2 m_WaitTime = new Vector2(8f, 15f);

		[Header("Audio")]

		[SerializeField]
		private SoundPlayer m_Audio;

		[SerializeField]
		[Clamp(0f, 15f)]
		private float m_AudioPlayInterval = 1f;

		private float m_NextMoveTime;
		private bool m_Waiting;
		private Vector3? m_SuspectedTarget = new Vector3?(Vector3.zero);
		private AIBrain m_Brain;
		private float m_LastAudioPlayTime;


        public override void OnStart(AIBrain brain)
        {
            m_Priority = 0;
            m_IsInterruptable = true;
            m_RepeatType = ET.ActionRepeatType.Repetitive;

            base.OnStart(brain);

			m_Brain = brain;
        }

        public override void Activate(AIBrain brain)
		{ 
			brain.Settings.Movement.MoveTo(m_PointPositions[m_CurrentIndex]);

			brain.AI.ChangeHealth.AddListener(OnSucceded_ChangeHealth);

			GameController.Audio.LastGunshot.AddChangeListener(OnChanged_LastGunshot);
		}

        public override void OnUpdate(AIBrain brain)
        {
			if(m_SuspectedTarget.HasValue)
			{
				if(brain.Settings.Movement.CurrentDestination != m_SuspectedTarget.Value)
					brain.Settings.Movement.MoveTo(m_SuspectedTarget.Value);

				if(brain.Settings.Movement.ReachedDestination())
				{
					m_SuspectedTarget = null;
					//Debug.Log("reached destination");
				}
			}
			// Has the agent reached the current selected waypoint?
			else if(brain.Settings.Movement.ReachedDestination())
            {            
				if(!m_Waiting)
				{
					m_Waiting = true;
					m_NextMoveTime = Time.time + Random.Range(m_WaitTime.x, m_WaitTime.y);
				}

				// If we've waited enough at this waypoint...
				if(Time.time > m_NextMoveTime)
				{
					// Change the waypoint then.
	                ChangePatrolPoint();

					// Make the agent move towards the new waypoint.
	                brain.Settings.Movement.MoveTo(m_PointPositions[m_CurrentIndex], m_IsUrgent);

					m_Waiting = false;
				}
            }

			if(Time.time > m_LastAudioPlayTime + m_AudioPlayInterval)
			{
				m_LastAudioPlayTime = Time.time;
				m_Audio.Play(ItemSelectionMethod.RandomlyButExcludeLast, brain.Settings.AudioSource, 1f);
			}
        }

        public override void OnDeactivation(AIBrain brain)
        {
			brain.Settings.Animation.ToggleBool(HelpStrings.AI.ANIMATOR_PARAM_WALK, false);
			brain.Settings.Animation.ToggleBool(HelpStrings.AI.ANIMATOR_PARAM_RUN, false);

			brain.AI.ChangeHealth.RemoveListener(OnSucceded_ChangeHealth);

			GameController.Audio.LastGunshot.RemoveChangeListener(OnChanged_LastGunshot);

			brain.Settings.AudioSource.Stop();
        }

        public override bool IsDone(AIBrain brain)
		{
			return false;
		}

		private void OnSucceded_ChangeHealth(HealthEventData data)
		{
			if(data.Damager != null)
				m_SuspectedTarget = data.Damager.transform.position;
		}

		private void OnChanged_LastGunshot()
		{
			// TODO Unregister this method when the entity died.
			if(m_Brain == null)
				return;

			var lastGunshot = GameController.Audio.LastGunshot.Get();

			var distanceToGunshotSquared = (lastGunshot.Position - m_Brain.transform.position).sqrMagnitude;

			if(distanceToGunshotSquared < m_Brain.Settings.Detection.HearRange * m_Brain.Settings.Detection.HearRange)
				m_SuspectedTarget = lastGunshot.Position;
		}
    }
}