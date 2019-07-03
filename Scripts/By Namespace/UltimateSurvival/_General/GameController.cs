using System.Collections;
using UnityEngine;
using UltimateSurvival.InputSystem;

namespace UltimateSurvival
{
	/// <summary>
	/// The game's central control point.
	/// </summary>
	public class GameController : MonoBehaviour
	{
		/// <summary> </summary>
		public static PlayerEventHandler LocalPlayer
		{ 
			get 
			{
				if(m_Player == null)
					m_Player = FindObjectOfType<PlayerEventHandler>();
				return m_Player;
			}
		}

		/// <summary> </summary>
		public static InputManager InputManager 
		{ 
			get 
			{
				if(m_InputManager == null)
					m_InputManager = FindObjectOfType<InputManager>();
				return m_InputManager;
			}
		}

		/// <summary></summary>
		public static float NormalizedTime { get; set; }

		/// <summary></summary>
		public static AudioUtils Audio { get; private set; }

		/// <summary></summary>
		public static Camera WorldCamera { get; private set; }

		/// <summary></summary>
		public static SurfaceDatabase SurfaceDatabase { get; private set; }

		/// <summary></summary>
		public static ItemDatabase ItemDatabase { get; private set; }

		public static TreeManager TerrainHelpers { get; private set; }

		[SerializeField]
		private SurfaceDatabase m_SurfaceDatabase;

		[SerializeField]
		private ItemDatabase m_ItemDatabase;

		private static InputManager m_InputManager;
		private static PlayerEventHandler m_Player;


		private void Awake()
		{
            //Shader.WarmupAllShaders();
			Audio = GetComponentInChildren<AudioUtils>();

			WorldCamera = LocalPlayer.transform.FindDeepChild("World Camera").GetComponent<Camera>();

			SurfaceDatabase = m_SurfaceDatabase;
			ItemDatabase = m_ItemDatabase;

			TerrainHelpers = GetComponent<TreeManager>();

			DontDestroyOnLoad(gameObject);
		}
	}
}
