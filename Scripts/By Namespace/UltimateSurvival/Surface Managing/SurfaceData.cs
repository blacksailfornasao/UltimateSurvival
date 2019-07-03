using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class SurfaceData
	{
		const int BULLET_DECAL_POOL_SIZE = 100;
		const int BULLET_IMPACT_FX_POOL_SIZE = 100;
		const int CHOP_FX_POOL_SIZE = 25;
		const int HIT_FX_POOL_SIZE = 25;

		/// <summary>The name of this surface.</summary>
		public string Name { get { return m_Name; } }

		public bool IsPenetrable { get { return m_IsPenetrable; } }

		[SerializeField]
		private string m_Name;

		[SerializeField]
		private Texture[] m_Textures;

		[Header("Footsteps")]

		[SerializeField]
		private SoundPlayer m_FootstepSounds;

		[SerializeField]
		private SoundPlayer m_JumpSounds;

		[SerializeField]
		private SoundPlayer m_LandSounds;

		[Header("Bullet Impact")]

		[SerializeField]
		private SoundPlayer m_BulletImpactSounds;

		[SerializeField]
		private PooledObject[] m_BulletDecals;

		[SerializeField]
		private PooledObject[] m_BulletImpactFX;

		[Header("Chop & Hit")]

		[SerializeField]
		private SoundPlayer m_ChopSounds;

		[SerializeField]
		private SoundPlayer m_HitSounds;

		[SerializeField]
		private PooledObject[] m_ChopFX;

		[SerializeField]
		private PooledObject[] m_HitFX;

		[Header("Penetration")]

		[SerializeField]
		private bool m_IsPenetrable;

		[SerializeField]
		private SoundPlayer m_SpearPenetrationSounds;

		[SerializeField]
		private SoundPlayer m_ArrowPenetrationSounds;

		private int m_LastPlayedFootstep;


		/// <summary>
		/// Is the texture part of this surface?
		/// </summary>
		public bool HasTexture(Texture texture)
		{
			for(int i = 0;i < m_Textures.Length;i ++)
				if(m_Textures[i] == texture)
					return true;

			return false;
		}

		/// <summary>
		/// Will play a sound from the array of sounds, based on the chosen selection method.
		/// </summary>
		/// <param name="selectionMethod">How should the audio clip be selected?... at random? consecutively? (see ClipSelectionMethod description for more info)</param>
		public void PlaySound(ItemSelectionMethod selectionMethod, SoundType soundType, float volumeFactor = 1f, AudioSource audioSource = null)
		{
			if(soundType == SoundType.BulletImpact)
				m_BulletImpactSounds.Play(selectionMethod, audioSource, volumeFactor);
			else if(soundType == SoundType.Footstep)
				m_FootstepSounds.Play(selectionMethod, audioSource, volumeFactor);
			else if(soundType == SoundType.Jump)
				m_JumpSounds.Play(selectionMethod, audioSource, volumeFactor);
			else if(soundType == SoundType.Land)
				m_LandSounds.Play(selectionMethod, audioSource, volumeFactor);
			else if(soundType == SoundType.Chop)
				m_ChopSounds.Play(selectionMethod, audioSource, volumeFactor);
			else if(soundType == SoundType.Hit)
				m_HitSounds.Play(selectionMethod, audioSource, volumeFactor);
			else if(soundType == SoundType.SpearPenetration)
				m_SpearPenetrationSounds.Play(selectionMethod, audioSource, volumeFactor);
			else if(soundType == SoundType.ArrowPenetration)
				m_ArrowPenetrationSounds.Play(selectionMethod, audioSource, volumeFactor);
		}

		/// <summary>
		/// Will play a sound from the array of sounds, based on the chosen selection method.
		/// NOTE: Will use the AudioSource.PlayClipAtPoint() method, which doesn't include pitch variation.
		/// </summary>
		/// <param name="selectionMethod">How should the audio clip be selected?... at random? consecutively? (see ClipSelectionMethod description for more info)</param>
		public void PlaySound(ItemSelectionMethod selectionMethod, SoundType soundType, float volumeFactor = 1f, Vector3 position = default(Vector3))
		{
			if(soundType == SoundType.BulletImpact)
				m_BulletImpactSounds.PlayAtPosition(selectionMethod, position, volumeFactor);
			else if(soundType == SoundType.Footstep)
				m_FootstepSounds.PlayAtPosition(selectionMethod, position, volumeFactor);
			else if(soundType == SoundType.Jump)
				m_JumpSounds.PlayAtPosition(selectionMethod, position, volumeFactor);
			else if(soundType == SoundType.Land)
				m_LandSounds.PlayAtPosition(selectionMethod, position, volumeFactor);
			else if(soundType == SoundType.Chop)
				m_ChopSounds.PlayAtPosition(selectionMethod, position, volumeFactor);
			else if(soundType == SoundType.Hit)
				m_HitSounds.PlayAtPosition(selectionMethod, position, volumeFactor);
			else if(soundType == SoundType.SpearPenetration)
				m_SpearPenetrationSounds.PlayAtPosition(selectionMethod, position, volumeFactor);
			else if(soundType == SoundType.ArrowPenetration)
				m_ArrowPenetrationSounds.PlayAtPosition(selectionMethod, position, volumeFactor);
		}

		/// <summary>
		/// 
		/// </summary>
		public void CreateBulletDecal(Vector3 position, Quaternion rotation, Transform parent = null)
		{
			if(m_BulletDecals.Length == 0)
				return;

			var decalPrefab = m_BulletDecals[Random.Range(0, m_BulletDecals.Length)];

			if(decalPrefab)
				GameObject.Instantiate(decalPrefab, position, rotation, parent);
			else
				Debug.LogWarningFormat("[({0}) SurfaceData] - A decal object was found null, please check the surface database for missing decals.", Name);
		}

		/// <summary>
		/// 
		/// </summary>
		public void CreateBulletImpactFX(Vector3 position, Quaternion rotation, Transform parent = null)
		{
			if(m_BulletImpactFX.Length == 0)
				return;

			var impactFXPrefab = m_BulletImpactFX[Random.Range(0, m_BulletImpactFX.Length)];

			if(impactFXPrefab)
				GameObject.Instantiate(impactFXPrefab, position, rotation, parent);
			else
				Debug.LogWarningFormat("[({0}) SurfaceData] - A bullet impact FX prefab was found null, please check the surface database for missing effects.", Name);
		}

		/// <summary>
		/// 
		/// </summary>
		public void CreateHitFX(Vector3 position, Quaternion rotation, Transform parent = null)
		{
			if(m_HitFX.Length == 0)
				return;

			var hitFXPrefab = m_HitFX[Random.Range(0, m_HitFX.Length)];

			if(hitFXPrefab)
				GameObject.Instantiate(hitFXPrefab, position, rotation, parent);
			else
				Debug.LogWarningFormat("[({0}) SurfaceData] - A hit FX prefab was found null, please check the surface database for missing effects.", Name);
		}

		/// <summary>
		/// 
		/// </summary>
		public void CreateChopFX(Vector3 position, Quaternion rotation, Transform parent = null)
		{
			if(m_ChopFX.Length == 0)
				return;

			var chopFXPrefab = m_ChopFX[Random.Range(0, m_ChopFX.Length)];

			if(chopFXPrefab)
				GameObject.Instantiate(chopFXPrefab, position, rotation, parent);
			else
				Debug.LogWarningFormat("[({0}) SurfaceData] - A chop FX prefab was found null, please check the surface database for missing effects.", Name);		}

	}
}