using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class AudioUtils : MonoBehaviour 
	{
		public Value<Gunshot> LastGunshot = new Value<Gunshot>(null);

		private Dictionary<AudioSource, Coroutine> m_LevelSetters = new Dictionary<AudioSource, Coroutine>();

		[SerializeField]
		private AudioSource m_2DAudioSource;


		public void Play2D(AudioClip clip, float volume)
		{
			if(m_2DAudioSource)
				m_2DAudioSource.PlayOneShot(clip, volume);
		}

		/// <summary>
		/// 
		/// </summary>
		public AudioSource CreateAudioSource(string name, Transform parent, Vector3 localPosition, bool is2D, float startVolume, float minDistance) 
		{
			GameObject audioObject = new GameObject(name, typeof(AudioSource));

			if(!parent) 
				parent = transform;
			
			audioObject.transform.parent = parent;
			audioObject.transform.localPosition = localPosition;
			AudioSource audioSource = audioObject.GetComponent<AudioSource>();
			audioSource.volume = startVolume;
			audioSource.spatialBlend = is2D ? 0f : 1f;
			audioSource.minDistance = minDistance;

			return audioSource;
		}
			
		/// <summary>
		/// 
		/// </summary>
		public void LerpVolumeOverTime(AudioSource audioSource, float targetVolume, float speed) 
		{
			if(m_LevelSetters.ContainsKey(audioSource)) 
			{
				if(m_LevelSetters[audioSource] != null)
					StopCoroutine(m_LevelSetters[audioSource]);
				
				m_LevelSetters[audioSource] = StartCoroutine(C_LerpVolumeOverTime(audioSource, targetVolume, speed));
			} 
			else 
				m_LevelSetters.Add(audioSource, StartCoroutine(C_LerpVolumeOverTime(audioSource, targetVolume, speed)));
		}

		/// <summary>
		/// 
		/// </summary>
		private IEnumerator C_LerpVolumeOverTime(AudioSource audioSource, float volume, float speed) 
		{
			while(audioSource != null && Mathf.Abs(audioSource.volume - volume) > 0.01f) 
			{
				audioSource.volume = Mathf.MoveTowards(audioSource.volume, volume, Time.deltaTime * speed);
				yield return null;
			}

			m_LevelSetters.Remove(audioSource);
		}
	}

	public class Gunshot
	{
		public Vector3 Position { get; private set; }
		public EntityEventHandler EntityThatShot { get; private set; }


		public Gunshot(Vector3 position, EntityEventHandler entityThatShot = null)
		{
			Position = position;
			EntityThatShot = entityThatShot;
		}
	}
}