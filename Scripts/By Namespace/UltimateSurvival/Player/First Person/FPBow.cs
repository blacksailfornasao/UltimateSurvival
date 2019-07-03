using System.Collections;
using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class FPBow : FPWeaponBase
	{
		[Header("Bow Setup")]

		[SerializeField]
		private LayerMask m_Mask;

		[SerializeField]
		private float m_MaxDistance = 50f; 

		[Header("Bow Settings")]

		[SerializeField]
		private float m_MinTimeBetweenShots = 1f;

		[Header("Arrow")]

		[SerializeField]
		private ShaftedProjectile m_ArrowPrefab;

		[SerializeField]
		private Vector3 m_SpawnOffset;

		[Header("Audio")]

		[SerializeField]
		private AudioSource m_AudioSource;

		[SerializeField]
		private SoundPlayer m_ReleaseAudio;

		[SerializeField]
		private SoundPlayer m_StretchAudio;

		private float m_NextTimeCanShoot;


		public override bool TryAttackOnce (Camera camera)
		{
			if(!Player.Aim.Active || Time.time < m_NextTimeCanShoot)
				return false;

			m_ReleaseAudio.Play(ItemSelectionMethod.Randomly, m_AudioSource, 1f);

			SpawnArrow(camera);

			m_NextTimeCanShoot = Time.time + m_MinTimeBetweenShots;
			Player.Aim.ForceStop();

			Attack.Send();

			return true;
		}

		protected override void Awake()
		{
			base.Awake();
			Player.Aim.AddStartTryer(OnTryStart_Aim);
		}

		private bool OnTryStart_Aim()
		{
			bool canStart = Time.time > m_NextTimeCanShoot || !IsEnabled;

			if(canStart && IsEnabled)
				m_StretchAudio.Play(ItemSelectionMethod.Randomly, m_AudioSource);

			return canStart;
		}

		private void SpawnArrow(Camera camera)
		{
			if(!m_ArrowPrefab)
			{
				Debug.LogErrorFormat("[{0}.FPBow] - No arrow prefab assigned in the inspector! Please assign one.", name);
				return;
			}

			Vector3 hitPoint;
			RaycastHit hitInfo;
			Ray ray = camera.ViewportPointToRay(Vector3.one * 0.5f);

			// Getting the target point.
			if(Physics.Raycast(ray, out hitInfo, m_MaxDistance, m_Mask, QueryTriggerInteraction.Ignore))
				hitPoint = hitInfo.point;
			else
				hitPoint = camera.transform.position + camera.transform.forward * m_MaxDistance;

			Vector3 position = transform.position + camera.transform.TransformVector(m_SpawnOffset);
			Quaternion rotation = Quaternion.LookRotation(hitPoint - position);

			var arrowObject = (GameObject)Instantiate(m_ArrowPrefab.gameObject, position, rotation);
			arrowObject.GetComponent<ShaftedProjectile>().Launch(Player);

			// Lower the durability...
			if(m_Durability != null)
			{
				var value = m_Durability.Float;
				value.Current --;
				m_Durability.SetValue(ItemProperty.Type.Float, value);

				if(value.Current == 0)
					Player.DestroyEquippedItem.Try();
			}
		}
	}
}
