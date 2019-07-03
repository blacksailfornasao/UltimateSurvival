using UnityEngine;

/// <summary>
/// 
/// </summary>
public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject 
{
	public static T Instance
	{
		get 
		{
			if(!m_Instance)
			{
				var all = Resources.LoadAll<T>("");

				if(all.Length > 0)
					m_Instance = all[0];
				else
					Debug.LogErrorFormat("[ScriptableSingleton] - No object of type '{0}' was found in the project!", typeof(T).Name);
			}

			return m_Instance;
		}
	}

	private static T m_Instance;
}