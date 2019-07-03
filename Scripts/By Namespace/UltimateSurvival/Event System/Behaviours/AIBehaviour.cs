using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class AIBehaviour : EntityBehaviour 
	{
		public AIEventHandler AI
		{
			get 
			{
				if(!m_AI)
					m_AI = GetComponentInParent<AIEventHandler>();
				return m_AI;
			}
		}

		private AIEventHandler m_AI;
	}
}
