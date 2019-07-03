using UnityEngine;
using UnityEngine.UI;

namespace UltimateSurvival.GUISystem
{
	public class PlayerVitalsGUI : GUIBehaviour
	{
		[SerializeField]
		private Image m_HealthBar;

		[SerializeField]
		private Image m_StaminaBar;

		[SerializeField]
		private Image m_ThirstBar;

		[SerializeField]
		private Image m_HungerBar;


		private void Start()
		{
			Player.Health.AddChangeListener(OnChanged_Health);
			Player.Stamina.AddChangeListener(OnChanged_Stamina);
			Player.Thirst.AddChangeListener(OnChanged_Thirst);
			Player.Hunger.AddChangeListener(OnChanged_Hunger);
		}

		private void OnChanged_Health()
		{
			m_HealthBar.fillAmount = Player.Health.Get() / 100f;
		}

		private void OnChanged_Stamina()
		{
			m_StaminaBar.fillAmount = Player.Stamina.Get() / 100f;
		}

		private void OnChanged_Thirst()
		{
			m_ThirstBar.fillAmount = Player.Thirst.Get() / 100f;
		}

		private void OnChanged_Hunger()
		{
			m_HungerBar.fillAmount = Player.Hunger.Get() / 100f;
		}
	}
}
