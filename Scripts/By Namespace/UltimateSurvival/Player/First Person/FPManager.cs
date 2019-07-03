using System.Collections;
using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class FPManager : PlayerBehaviour 
	{
		[SerializeField]
		private Camera m_WorldCamera;

		[SerializeField]
		private Camera m_FPCamera;

		[Header("Aiming")]

		[SerializeField]
		[Range(0f, 100f)]
		private float m_NormalFOV = 75f;

		[SerializeField]
		[Range(0f, 100f)]
		private float m_AimFOV = 45f;

		[SerializeField]
		[Clamp(0f, 9999f)]
		private float m_FOVSetSpeed = 30f;

		[SerializeField]
		[Range(0.1f, 1f)]
		private float m_AimSpeedMultiplier = 0.6f;

		[Header("Equipping")]

		[SerializeField]
		[Range(0f, 3f)]
		private float m_DrawTime = 0.7f;

		[SerializeField]
		[Range(0f, 3f)]
		private float m_HolsterTime = 0.5f;

		private FPObject[] m_Objects;
		private FPObject m_EquippedObject;
		//private FPMotion m_EquippedObjMotion;
		private FPWeaponBase m_EquippedWeapon;

		private float m_NextTimeCanEquip;
		private bool m_WaitingToEquip;
		private bool m_WaitingToDisable;
		private float m_NextTimeCanDisable;

		private Coroutine m_FOVSetter;


		private void Awake()
		{
			Player.ChangeEquippedItem.SetTryer(Try_ChangeEquippedItem);

			Player.AttackOnce.SetTryer(()=> OnTry_Attack(false));
			Player.AttackContinuously.SetTryer(()=> OnTry_Attack(true));
			Player.Aim.AddStartTryer(TryStart_Aim);
			Player.Aim.AddStopListener(OnStop_Aim);

			Player.Sleep.AddStopListener(OnStop_Sleep);

			Player.IsCloseToAnObject.AddChangeListener(OnChanged_IsCloseToAnObject);

			m_Objects = GetComponentsInChildren<FPObject>(true);

			foreach(var obj in m_Objects)
			{
				obj.On_Holster();
				TryDisableObject(obj.gameObject, false);
			}
		}

		private void Update()
		{
			if(m_WaitingToDisable && Time.time > m_NextTimeCanDisable)
			{
				TryDisableObject(m_EquippedObject.gameObject, true);
				m_WaitingToDisable = false;
			}

			if(m_WaitingToEquip && Time.time > m_NextTimeCanEquip)
			{
				TryEquipItem();
				m_WaitingToEquip = false;
			}
		}

		private bool Try_ChangeEquippedItem(SavableItem item, bool instantly)
		{
			if(Player.EquippedItem.Get() == item)
				return true;

			// Register the object for equipping.
			m_WaitingToEquip = true;
			m_NextTimeCanEquip = Time.time;

			if(!instantly && m_EquippedObject != null)
				m_NextTimeCanEquip += m_HolsterTime;

			// Register the current equipped object for disabling.
			if(m_EquippedObject != null)
			{
				m_EquippedObject.On_Holster();

				m_WaitingToDisable = true;
				m_NextTimeCanDisable = Time.time;

				if(!instantly)
					m_NextTimeCanDisable += m_HolsterTime;
			}

			Player.EquippedItem.Set(item);

			return true;
		}

		private void TryEquipItem()
		{
			var item = Player.EquippedItem.Get();
			if(item == null)
				return;

			foreach(var obj in m_Objects)
			{
				if(obj.ObjectName == item.Name)
				{
					obj.gameObject.SetActive(true);

					obj.On_Draw(item);

					m_EquippedObject = obj;
					m_EquippedWeapon = m_EquippedObject as FPWeaponBase;
					//m_EquippedObjMotion = m_EquippedObject.GetComponent<FPMotion>();

					m_FPCamera.fieldOfView = m_EquippedObject.TargetFOV;

					break;
				}
			}
		}

		private void TryDisableObject(GameObject obj, bool isCurrent = false)
		{
			if(obj == null)
				return;

			obj.gameObject.SetActive(true);
			obj.gameObject.SetActive(false);

			if(isCurrent)
			{
				//m_EquippedObjMotion = null;
				m_EquippedObject = null;
				m_EquippedWeapon = null;
			}
		}

		private void OnStop_Sleep()
		{
			if(m_EquippedObject)
				m_EquippedObject.On_Draw(Player.EquippedItem.Get());
		}

		private void OnChanged_IsCloseToAnObject()
		{
			if(m_EquippedWeapon != null && Player.IsCloseToAnObject.Get() && !m_EquippedWeapon.UseWhileNearObjects && Player.Aim.Active)
				Player.Aim.ForceStop();
		}

		private bool TryStart_Aim()
		{
			bool canStartAiming = 
				Player.NearLadders.Count == 0 &&
				(!Player.IsCloseToAnObject.Get() || (m_EquippedWeapon && m_EquippedWeapon.UseWhileNearObjects)) &&
				m_EquippedObject && Time.time > m_NextTimeCanEquip + m_DrawTime &&
				InventoryController.Instance.IsClosed && 
				!Player.Run.Active;

			if(canStartAiming && m_EquippedObject as FPHitscan)
			{
				if(m_FOVSetter != null)
					StopCoroutine(m_FOVSetter);

				m_FOVSetter = StartCoroutine(C_SetFOV(m_AimFOV));
			}

			if(canStartAiming)
				Player.MovementSpeedFactor.Set(m_AimSpeedMultiplier);
			
			return canStartAiming;
		}

		private void OnStop_Aim()
		{
			if(m_FOVSetter != null)
				StopCoroutine(m_FOVSetter);

			m_FOVSetter = StartCoroutine(C_SetFOV(m_NormalFOV));

			Player.MovementSpeedFactor.Set(1f);
		}

		private IEnumerator C_SetFOV(float targetFOV)
		{
			while(Mathf.Abs(m_WorldCamera.fieldOfView - targetFOV) > Mathf.Epsilon)
			{
				m_WorldCamera.fieldOfView = Mathf.MoveTowards(m_WorldCamera.fieldOfView, targetFOV, Time.deltaTime * m_FOVSetSpeed);
				yield return null;
			}
		}
			
		private bool OnTry_Attack(bool continuously)
		{
			if(m_EquippedWeapon == null)
				return false;

			bool tooCloseCondition = Player.IsCloseToAnObject.Get() && !m_EquippedWeapon.UseWhileNearObjects;
			bool canTryToAttack = 
				Player.NearLadders.Count == 0 &&
				!tooCloseCondition &&
				!Player.Run.Active && 
				InventoryController.Instance.IsClosed;

			if(canTryToAttack && Time.time > m_EquippedObject.LastDrawTime + m_DrawTime)
			{
				bool attackWasSuccessful;

				if(continuously)
					attackWasSuccessful = m_EquippedWeapon.TryAttackContinuously(m_WorldCamera);
				else
					attackWasSuccessful = m_EquippedWeapon.TryAttackOnce(m_WorldCamera);
				
				return attackWasSuccessful;
			}

			return false;
		}
	}
}
