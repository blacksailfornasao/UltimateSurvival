using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public enum FootType { Left, Right }

	/// <summary>
	/// Will play a footstep sound when the character travels enough distance on a surface.
	/// </summary>
	public class FootstepPlayer : PlayerBehaviour
	{
		[Header("General")]

		[SerializeField] 
		private ItemSelectionMethod m_FootstepSelectionMethod;

		[SerializeField] 
		private float m_LandSpeedThreeshold = 3f;

		[SerializeField]
		private LayerMask m_Mask;

		[SerializeField]
		private AudioSource m_AudioSource;

		[Header("Distance Between Steps")]

		[SerializeField] 
		private float m_WalkStepLength = 1.7f;	

		[SerializeField] 
		private float m_RunStepLength = 2f;		

		[Header("Volume Factors")]	

		[SerializeField] 
		private float m_WalkVolumeFactor = 0.5f;	

		[SerializeField] 
		private float m_RunVolumeFactor = 1f;				

		private AudioSource m_LeftFootSource;				
		private AudioSource m_RightFootSource;	
		private FootType m_LastFroundedFoot;	
		private float m_AccumulatedDistance;


		private void Start()
		{
			// Make sure we get notified when the player jumps or lands.
			Player.Jump.AddStartListener(OnStart_PlayerJump);
			Player.Land.AddListener(On_PlayerLanded);

			m_LeftFootSource = GameController.Audio.CreateAudioSource("Left Foot Footstep", transform, new Vector3(-0.2f, 0f, 0f), false, 1f, 3f);
			m_RightFootSource = GameController.Audio.CreateAudioSource("Right Foot Footstep", transform, new Vector3(0.2f, 0f, 0f), false, 1f, 3f);
		}

		private void FixedUpdate()
		{
			// Don't do anything if the player isn't grounded.
			if(!Player.IsGrounded.Get()) 
				return;

			// Update the distance accumulated based on the player's current speed.
			m_AccumulatedDistance += Player.Velocity.Get().magnitude * Time.fixedDeltaTime;

			// Get the step distance we should aim for, is different for crouching, walking and sprinting.
			float stepDistance = GetStepLength();

			// If we accumulated enough distance since the last footstep, play the sound and reset the counter.
			if(m_AccumulatedDistance > stepDistance) 
			{
				PlayFootstep();
				m_AccumulatedDistance = 0f;
			}
		}

		/// <summary>
		/// This method will play a footstep sound based on the selected method, and at a specific volume.
		/// </summary>
		private void PlayFootstep() 
		{
			var data = GetDataFromBelow();
			if(data == null)
				return;

			// Alternate which audio source should be used (left foot or right foot).
			AudioSource targetSource = null;
			targetSource = (m_LastFroundedFoot == FootType.Left ? m_RightFootSource : m_LeftFootSource);
			m_LastFroundedFoot = (m_LastFroundedFoot == FootType.Left ? FootType.Right : FootType.Left);

			// Get the volumeFactor (it is different for crouching, walking and sprinting, for example when crouching the volume should be low).
			float volumeFactor = GetVolumeFactor();
		
			data.PlaySound(m_FootstepSelectionMethod, SoundType.Footstep, volumeFactor, targetSource);
		}

		/// <summary>
		/// Calculates the target step length based on whether we are walking, crouching or sprinting.
		/// </summary>
		private float GetStepLength()
		{
			float targetDistance = m_WalkStepLength;
			if(Player.Run.Active) 
				targetDistance = m_RunStepLength;

			return targetDistance;
		}

		/// <summary>
		/// Calculates the target volume based on whether player is walking, crouching or sprinting.
		/// </summary>
		private float GetVolumeFactor() 
		{
			float targetVolume = m_WalkVolumeFactor;
			if(Player.Run.Active) 
				targetVolume = m_RunVolumeFactor;

			return targetVolume;
		}

		private void OnStart_PlayerJump()
		{
			var data = GetDataFromBelow();
			if(data == null)
				return;

			data.PlaySound(ItemSelectionMethod.RandomlyButExcludeLast, SoundType.Jump, 1f, m_AudioSource);
		}

		private void On_PlayerLanded(float landSpeed)
		{
			var data = GetDataFromBelow();
			if(data == null)
				return;

			// Don't play the landing clip when the landing speed is low.
			bool landingWasHard = landSpeed >= m_LandSpeedThreeshold;
			if(landingWasHard) 
			{
				data.PlaySound(ItemSelectionMethod.RandomlyButExcludeLast, SoundType.Land, 1f, m_AudioSource);
				m_AccumulatedDistance = 0f;
			}
		}

		private SurfaceData GetDataFromBelow()
		{
			if(!GameController.SurfaceDatabase)
			{
				Debug.LogWarning("No surface database found! can't play any footsteps...", this);
				return null;
			}
				
			Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
			var surfaceData = GameController.SurfaceDatabase.GetSurfaceData(ray, 1f, m_Mask);

			//print(surfaceData.Name);

			return surfaceData;
		}
	}
}

