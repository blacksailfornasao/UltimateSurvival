using UnityEngine;

namespace UltimateSurvival.GUISystem
{
	public class GUIBehaviour : MonoBehaviour 
	{
		public GUIController Controller 
		{
			get
			{
				if(m_Controller == null)
					m_Controller = GetComponentInParent<GUIController>();

				return m_Controller;
			}
		}

		public PlayerEventHandler Player { get { return Controller ? Controller.Player : null; } }

		private GUIController m_Controller = null;
	}
}
