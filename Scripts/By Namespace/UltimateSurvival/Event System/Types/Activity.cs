using System;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class Activity
	{
		public bool Active { get; private set; }

		private TryerDelegate m_StartTryers;
		private TryerDelegate m_StopTryers;
		private Action m_OnStart;
		private Action m_OnStop;


		/// <summary>
		/// This will register a method that will approve or disapprove the starting of this activity.
		/// </summary>
		public void AddStartTryer(TryerDelegate tryer)
		{
			m_StartTryers += tryer;
		}

		/// <summary>
		/// This will register a method that will approve or disapprove the stopping of this activity.
		/// </summary>
		public void AddStopTryer(TryerDelegate tryer)
		{
			m_StopTryers += tryer;
		}

		/// <summary>
		/// Will be called when this activity starts.
		/// </summary>
		public void AddStartListener(Action listener)
		{
			m_OnStart += listener;
		}

		/// <summary>
		/// Will be called when this activity stops.
		/// </summary>
		public void AddStopListener(Action listener)
		{
			m_OnStop += listener;
		}

		/// <summary>
		///
		/// </summary>
		public void ForceStart()
		{
			if(Active)
				return;

			Active = true;
			if(m_OnStart != null)
				m_OnStart();
		}

		/// <summary>
		///
		/// </summary>
		public bool TryStart()
		{
			if(Active)
				return false;

			if(m_StartTryers != null)
			{
				bool activityStarted = CallStartApprovers();

				if(activityStarted)
					Active = true;
				
				if(activityStarted && m_OnStart != null)
					m_OnStart();
				
				return activityStarted;
			}
			else
				Debug.LogWarning("[Activity] - You tried to start an activity which has no tryer (if no one checks if the activity can start, it won't start).");

			return false;
		}
			
		/// <summary>
		///
		/// </summary>
		public bool TryStop()
		{
			if(!Active)
				return false;

			if(m_StopTryers != null)
			{
				if(CallStopApprovers())
				{
					Active = false;

					if(m_OnStop != null)
						m_OnStop();
					
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// The activity will stop immediately.
		/// </summary>
		public void ForceStop()
		{
			if(!Active)
				return;
			
			Active = false;

			if(m_OnStop != null)
				m_OnStop();
		}

		private bool CallStartApprovers()
		{
			var invocationList = m_StartTryers.GetInvocationList();
			for(int i = 0;i < invocationList.Length;i ++)
			{
				if(!(bool)invocationList[i].DynamicInvoke())
					return false;
			}

			return true;
		}

		private bool CallStopApprovers()
		{
			var invocationList = m_StopTryers.GetInvocationList();
			for(int i = 0;i < invocationList.Length;i ++)
			{
				if(!(bool)invocationList[i].DynamicInvoke())
					return false;
			}

			return true;
		}
	}
}