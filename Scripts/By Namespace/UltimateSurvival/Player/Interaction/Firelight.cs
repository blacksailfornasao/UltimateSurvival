using System.Collections;
using UnityEngine;

namespace UltimateSurvival
{
	[RequireComponent(typeof(Light))]
    public class Firelight : MonoBehaviour
    {
		[SerializeField]
		[Range(0.1f, 3f)]
		private float m_ToggleDuration = 0.5f;

		[SerializeField]
		[Range(0.1f, 3f)]
		private float m_LightIntensity = 1f;

		[SerializeField]
		[Range(0.5f, 3f)]
		private float m_LightRange = 2f;

		[SerializeField]
		[Range(0f, 3f)]
		[Tooltip("How much the light intensity changes over time.")]
		private float m_IntensityNoise = 1f;

		private Light m_Light;
		private bool m_IsInTransition;
		private bool m_TargetState;


		public void Toggle(bool toggle)
		{
			m_TargetState = toggle;
			StopAllCoroutines();
			StartCoroutine(C_Toggle(toggle));
		}

		private void Awake()
		{
			m_Light = GetComponent<Light>();
		}

		private void Update()
		{
			if(!m_IsInTransition && m_TargetState)
				m_Light.intensity = m_LightIntensity + Mathf.PerlinNoise(Time.time, Time.time + 3f) * m_IntensityNoise;
		}

		private IEnumerator C_Toggle(bool toggle)
		{
			m_IsInTransition = true;

			float endTime = Time.time + m_ToggleDuration;
			while(Time.time < endTime)
			{
				// Intensity.
				m_Light.intensity = Mathf.MoveTowards(
					m_Light.intensity, 
					toggle ? m_LightIntensity : 0f, 
					Time.deltaTime * m_LightIntensity / m_ToggleDuration);

				// Range.
				m_Light.range = Mathf.MoveTowards(
					m_Light.range, 
					toggle ? m_LightRange : 0f, 
					Time.deltaTime * m_LightIntensity / m_ToggleDuration);

				yield return null;
			}

			m_IsInTransition = false;
		}
    }
}
