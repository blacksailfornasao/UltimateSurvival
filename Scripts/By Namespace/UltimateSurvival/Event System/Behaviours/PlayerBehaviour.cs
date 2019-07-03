using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class PlayerBehaviour : EntityBehaviour 
	{
		/// <summary></summary>
		public PlayerEventHandler Player
		{
			get 
			{
				if(!m_Player)
					m_Player = GetComponent<PlayerEventHandler>();
				if(!m_Player)
					m_Player = GetComponentInParent<PlayerEventHandler>();
				
				return m_Player;
			}
		}

		private PlayerEventHandler m_Player;
	}
}
