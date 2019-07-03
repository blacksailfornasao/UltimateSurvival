using UnityEngine;
using UnityEngine.UI;

namespace UltimateSurvival.GUISystem
{
	/// <summary>
	/// 
	/// </summary>
	public class RequiredItemRow : MonoBehaviour 
	{
		[SerializeField] Color	m_HaveEnoughColor = Color.white;
		[SerializeField] Color	m_DontHaveEnoughColor = Color.red;
		[SerializeField] Text 	m_Amount;
		[SerializeField] Text 	m_Type;
		[SerializeField] Text 	m_Total;
		[SerializeField] Text 	m_Have;


		public void Set(int amount, string type, int total, int have)
		{
			bool show = !string.IsNullOrEmpty(type);

			m_Amount.text = show ? amount.ToString() : "";
			m_Type.text = show ? type : "";
			m_Total.text = show ? total.ToString() : "";
			m_Have.text = show ? have.ToString() : "";

			if(show)
				m_Have.color = have >= total ? m_HaveEnoughColor : m_DontHaveEnoughColor;
		}
	}
}
