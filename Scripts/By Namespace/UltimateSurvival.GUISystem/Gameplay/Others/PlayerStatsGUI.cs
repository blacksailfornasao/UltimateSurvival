using UnityEngine;
using UnityEngine.UI;

namespace UltimateSurvival.GUISystem
{
	public class PlayerStatsGUI : GUIBehaviour
	{
		[SerializeField]
		private Text m_DefenseText;


		private void Start()
		{
			Player.Defense.AddChangeListener(OnChanged_Defense);
		}

		private void OnChanged_Defense()
		{
			m_DefenseText.text = Player.Defense.Get() + "%";
		}
	}
}
