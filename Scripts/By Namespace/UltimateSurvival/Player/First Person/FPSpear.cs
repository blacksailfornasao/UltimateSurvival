using System.Collections;
using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class FPSpear : FPWeaponBase
	{
		public bool CanThrow { get { return Time.time > m_NextTimeCanThrow; } }

		[Header("Setup")]

		[SerializeField]
		private LayerMask m_Mask;

		[SerializeField]
		private float m_MaxDistance = 50f;

		[Header("Settings")]

		[SerializeField]
		private float m_MinTimeBetweenThrows = 1.5f;

		[Header("Object To Throw")]

		[SerializeField]
		private ShaftedProjectile m_SpearPrefab;

		[SerializeField]
		private Vector3 m_SpawnOffset;

		[SerializeField]
		private float m_SpawnDelay = 0.3f;

		[Header("Audio")]

		[SerializeField]
		private AudioSource m_AudioSource;

		[SerializeField]
		private SoundPlayer m_ThrowAudio;

		private float m_NextTimeCanThrow;


		public override bool TryAttackOnce (Camera camera)
		{
			if(!Player.Aim.Active || !CanThrow)
				return false;

			m_ThrowAudio.Play(ItemSelectionMethod.Randomly, m_AudioSource, 1f);

			StartCoroutine(C_ThrowWithDelay(camera, m_SpawnDelay));
			m_NextTimeCanThrow = Time.time + m_MinTimeBetweenThrows;

			Attack.Send();

			return true;
		}

		private void Start()
		{
			Player.Aim.AddStartTryer(OnTryStart_Aim);
		}

		private bool OnTryStart_Aim()
		{
			return !IsEnabled || CanThrow;
		}

		private IEnumerator C_ThrowWithDelay(Camera camera, float delay)
		{
			if(!m_SpearPrefab)
			{
				Debug.LogErrorFormat(this, "The spear prefab is not assigned in the inspector!.", name);
				yield break;
			}

			yield return new WaitForSeconds(delay);

			Vector3 hitPoint;
			RaycastHit hitInfo;
			Ray ray = camera.ViewportPointToRay(Vector3.one * 0.5f);

			// Get the target point.
			if(Physics.Raycast(ray, out hitInfo, m_MaxDistance, m_Mask, QueryTriggerInteraction.Ignore))
				hitPoint = hitInfo.point;
			else
				hitPoint = camera.transform.position + camera.transform.forward * m_MaxDistance;

			Vector3 position = transform.position + camera.transform.TransformVector(m_SpawnOffset);
			Quaternion rotation = Quaternion.LookRotation(hitPoint - position);

			var spearObject = (GameObject)Instantiate(m_SpearPrefab.gameObject, position, rotation);
			spearObject.GetComponent<ShaftedProjectile>().Launch(Player);

			// Lower the durability...
			if(m_Durability != null)
			{
				var value = m_Durability.Float;
				value.Current --;
				m_Durability.SetValue(ItemProperty.Type.Float, value);

				if(value.Current == 0)
					Player.DestroyEquippedItem.Try();
			}
				
			//Player.EquipItem.Try(null, true);
			//Player.DestroyEquippedItem.Try();
		}
	}
}
