using System.Collections;
using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class FPTorch : FPObject
	{
		[Header("Torch Settings")]

		[SerializeField]
		[Tooltip("The torch light.")]
		private Light m_Light;

		[SerializeField]
		[Tooltip("The parent of all the torch particle systems.")]
		private ParticleSystem m_MainSystem;

		[SerializeField]
		[Range(0f, 10f)]
		[Tooltip("The light intensity of the torch.")]
		private float m_LightIntensity = 2f;

		[SerializeField]
		[Range(0f, 20f)]
		[Tooltip("The light range of the torch.")]
		private float m_LightRange = 10f;

		[SerializeField]
		[Range(0.1f, 1f)]
		[Tooltip("Aproximately how much time it takes to draw / equip the torch.")]
		private float m_LightDrawDuration = 0.5f;

		[SerializeField]
		[Clamp(0f, Mathf.Infinity)]
		[Tooltip("How much the light intensity changes over time.")]
		private float m_IntensityNoise = 1f;

		[Header("Audio")]

		[SerializeField]
		private AudioSource m_AudioSource;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_Volume;

		private bool m_IsInTransition;


		public override void On_Draw(SavableItem correspondingItem)
		{
			base.On_Draw(correspondingItem);
			StopAllCoroutines();
			StartCoroutine(C_ToggleTorch(true));
		}

		public override void On_Holster()
		{
			base.On_Holster();

			if(gameObject.activeSelf)
			{
				StopAllCoroutines();
				StartCoroutine(C_ToggleTorch(false));
			}
		}

		private void Start()
		{
			m_Light.intensity = m_LightIntensity;
			m_Light.range = m_LightRange;
		}

		private void Update()
		{
			if(!m_IsInTransition)
				m_Light.intensity = m_LightIntensity + Mathf.PerlinNoise(Time.time, Time.time + 3f) * m_IntensityNoise;
		}

		private IEnumerator C_ToggleTorch(bool toggle)
		{
			m_IsInTransition = true;
			if(toggle)
			{
				m_MainSystem.Play(true);
				GameController.Audio.LerpVolumeOverTime(m_AudioSource, m_Volume, 1f / m_LightDrawDuration);
			}
			else
			{
				m_MainSystem.Stop(true);
				GameController.Audio.LerpVolumeOverTime(m_AudioSource, 0f, 1f / m_LightDrawDuration);
			}

			float endTime = Time.time + m_LightDrawDuration;
			while(Time.time < endTime)
			{
				// Intensity.
				m_Light.intensity = Mathf.MoveTowards(
					m_Light.intensity, 
					toggle ? m_LightIntensity : 0f, 
					Time.deltaTime * m_LightIntensity / m_LightDrawDuration);

				// Range.
				m_Light.range = Mathf.MoveTowards(
					m_Light.range, 
					toggle ? m_LightRange : 0f, 
					Time.deltaTime * m_LightIntensity / m_LightDrawDuration);

				yield return null;
			}

			m_IsInTransition = false;
		}

		private void OnValidate()
		{
			if(m_Light)
			{
				m_Light.intensity = m_LightIntensity;
				m_Light.range = m_LightRange;
			}

			if(m_AudioSource)
				m_AudioSource.volume = m_Volume;
		}
	}
}
