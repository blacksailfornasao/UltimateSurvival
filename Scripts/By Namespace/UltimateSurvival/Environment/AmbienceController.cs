using UnityEngine;

namespace UltimateSurvival
{
	public class AmbienceController : MonoBehaviour 
	{
		[Header("Setup")]
		
		[SerializeField]
		private AudioSource m_DayAudioSrc;

		[SerializeField]
		private AudioSource m_NightAudioSrc;

		[Header("Ambience Volume")]

		[SerializeField]
		private AnimationCurve m_DayVolCurve;

		[SerializeField]
		private float m_DayVolFactor = 0.7f;

		[SerializeField]
		private AnimationCurve m_NightVolCurve;

		[SerializeField]
		private float m_NightVolFactor = 0.7f;


		private void Update()
		{
			m_DayAudioSrc.volume = m_DayVolCurve.Evaluate(GameController.NormalizedTime) * m_DayVolFactor;
			m_NightAudioSrc.volume = m_NightVolCurve.Evaluate(GameController.NormalizedTime) * m_NightVolFactor;
		}
	}
}
