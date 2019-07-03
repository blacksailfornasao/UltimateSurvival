using System;
using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class FPHitscanEventHandler : MonoBehaviour
	{
		public Message<string> AnimEvent_SpawnObject = new Message<string>();

		[SerializeField]
		private AudioSource m_AudioSource;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_Volume = 0.7f;

		[SerializeField]
		private AudioClip[] m_Sounds;


		public void PlaySound(int index)
		{
			if(m_AudioSource && m_Sounds.Length > 0)
				m_AudioSource.PlayOneShot(m_Sounds[Mathf.Clamp(index, 0, m_Sounds.Length - 1)], m_Volume);
		}

		public void SpawnObject(string name)
		{
			AnimEvent_SpawnObject.Send(name);	
		}
	}
}
