using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour 
	{
		public static T Instance 
		{ 
			get
			{
				if(m_Instance == null)
					m_Instance = FindObjectOfType<T>();

				return m_Instance;
			}
		}

		private static T m_Instance;
	}
}
