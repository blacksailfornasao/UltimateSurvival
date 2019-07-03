using System;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateSurvival.GUISystem
{
	public enum CrosshairType { Simple, Dynamic }

	[Serializable]
	public class CrosshairData
	{
		/// <summary></summary>
		public string ItemName { get { return m_ItemName; } }

		/// <summary></summary>
		public bool HideWhenAiming { get { return m_HideWhenAiming; } }

		[SerializeField]
		private string m_ItemName;

		[SerializeField]
		private bool m_HideWhenAiming = true;

		[SerializeField]
		private Color m_NormalColor = Color.white;

		[SerializeField]
		private Color m_OnEntityColor = Color.red;

		[SerializeField]
		private CrosshairType m_Type;

		[SerializeField]
		private Image m_Image;

		[SerializeField]
		private Sprite m_Sprite;

		[SerializeField]
		private Vector2 m_Size = new Vector2(64f, 64f);

		[SerializeField]
		private DynamicCrosshair m_Crosshair;

		[SerializeField]
		[Clamp(0f, 256f)]
		private float m_IdleDistance = 32f;

		[SerializeField]
		[Clamp(0f, 256f)]
		private float m_CrouchDistance = 24f;

		[SerializeField]
		[Clamp(0f, 256f)]
		private float m_WalkDistance = 36f;

		[SerializeField]
		[Clamp(0f, 256f)]
		private float m_RunDistance = 48f;

		[SerializeField]
		[Clamp(0f, 256f)]
		private float m_JumpDistance = 54f;


		public static implicit operator bool(CrosshairData cd)
		{
			return cd != null;
		}

		public void Update(PlayerEventHandler player)
		{
			var raycastInfo = player.RaycastData.Get();
			bool onEntity = false;

			if(raycastInfo && raycastInfo.HitInfo.collider && raycastInfo.HitInfo.collider.GetComponent<HitBox>())
				onEntity = true;

			if(m_Type == CrosshairType.Dynamic && m_Crosshair)
			{
				m_Crosshair.SetColor(onEntity ? m_OnEntityColor : m_NormalColor);	

				float distance = m_IdleDistance;

				if(player.Crouch.Active)
					distance = m_CrouchDistance;
				else if(player.Walk.Active)
					distance = m_WalkDistance;
				else if(player.Run.Active)
					distance = m_RunDistance;
				else if(player.Jump.Active)
					distance = m_JumpDistance;

				m_Crosshair.SetDistance(Mathf.Lerp(m_Crosshair.Distance, distance, Time.deltaTime * 10f));
			}
			else if(m_Type == CrosshairType.Simple && m_Image)
				m_Image.color = onEntity ? m_OnEntityColor : m_NormalColor;
		}

		public void SetActive(bool active)
		{
			if(m_Type == CrosshairType.Dynamic && m_Crosshair)
				m_Crosshair.SetActive(active);
			else if(m_Type == CrosshairType.Simple && m_Image)
			{
				m_Image.enabled = active;
				m_Image.sprite = m_Sprite;
				m_Image.rectTransform.sizeDelta = m_Size;
			}
		}
	}

	/// <summary> 
	/// 
	/// </summary>
	[Serializable]
	public class MessageForPlayer
	{
		[SerializeField]
		private GameObject m_Root;

		[SerializeField]
		private Text m_Text;


		/// <summary> 
		/// 
		/// </summary>
		public void Toggle(bool toggle)
		{
			m_Root.SetActive(toggle);
		}

		/// <summary> 
		/// 
		/// </summary>
		public void SetText(string message)
		{
			m_Text.text = message;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class CrosshairManager : GUIBehaviour
	{		
		[Header("Messages")]

		[SerializeField]
		private GameObject m_OpenMessage;

		[SerializeField]
		private MessageForPlayer m_GrabMessage;

		[Header("Crosshairs")]

		[SerializeField]
		private CrosshairData m_DefaultCrosshair;

		[SerializeField]
		private CrosshairData[] m_CustomCrosshairs;

		private CrosshairData m_CurrentCrosshair;
		private float m_OpenGraphicHideTime;


		private void Start()
		{
			Player.EquippedItem.AddChangeListener(OnChanged_EquippedItem);
			Player.Aim.AddStartListener(OnStart_Aim);
			Player.Aim.AddStopListener(OnStop_Aim);
			Player.RaycastData.AddChangeListener(OnChanged_RaycastObject);

			InventoryController.Instance.State.AddChangeListener(OnChanged_InventoryState);

			if(Player.EquippedItem.Get())
				OnChanged_EquippedItem();
		}

		private void Update()
		{
			if(m_CurrentCrosshair)
				m_CurrentCrosshair.Update(Player);
		}

		private void OnChanged_InventoryState()
		{
			bool inventoryClosed = InventoryController.Instance.IsClosed;

			if(m_CurrentCrosshair)
				m_CurrentCrosshair.SetActive(inventoryClosed);

			if(!inventoryClosed)
			{
				m_OpenMessage.SetActive(false);
				m_GrabMessage.Toggle(false);
			}
		}

		private void OnChanged_EquippedItem()
		{
			if(!InventoryController.Instance.IsClosed)
				return;

			if(m_CurrentCrosshair)
			{
				m_CurrentCrosshair.SetActive(false);
				m_CurrentCrosshair = null;
			}
			
			var equippedItem = Player.EquippedItem.Get();

			if(equippedItem != null)
			{
				for(int i = 0;i < m_CustomCrosshairs.Length;i ++)
					if(m_CustomCrosshairs[i].ItemName == equippedItem.ItemData.Name)
					{
						m_CurrentCrosshair = m_CustomCrosshairs[i];
						m_CurrentCrosshair.SetActive(true);

						return;
					}
			}

			m_CurrentCrosshair = m_DefaultCrosshair;
			m_CurrentCrosshair.SetActive(true);
		}

		private void OnStart_Aim()
		{
			if(m_CurrentCrosshair != null && m_CurrentCrosshair.HideWhenAiming)
				m_CurrentCrosshair.SetActive(false);
		}

		private void OnStop_Aim()
		{
			if(m_CurrentCrosshair != null && InventoryController.Instance.IsClosed)
				m_CurrentCrosshair.SetActive(true);
		}

		private void OnChanged_RaycastObject()
		{
			bool inventoryClosed = InventoryController.Instance.IsClosed;
			if(!inventoryClosed)
			{
				m_OpenMessage.SetActive(false);
				m_GrabMessage.Toggle(false);
				if(m_CurrentCrosshair)
					m_CurrentCrosshair.SetActive(false);

				return;
			}
				
			IInventoryTrigger openable = null;
			ItemPickup pickup = null;
			Building.Door door = null;
			SleepingBag bag = null;
			Lamp lamp = null;

			var raycastData = Player.RaycastData.Get();

			if(raycastData && raycastData.ObjectIsInteractable)
			{
				openable = raycastData.GameObject.GetComponent<IInventoryTrigger>();
				pickup = raycastData.GameObject.GetComponent<ItemPickup>();
				door = raycastData.GameObject.GetComponent<Building.Door>();
				bag = raycastData.GameObject.GetComponent<SleepingBag>();
				lamp = raycastData.GameObject.GetComponent<Lamp>();
			}
		
			m_OpenMessage.SetActive(openable != null);

			if(pickup != null && pickup.ItemToAdd != null)
			{
				m_GrabMessage.Toggle(true);
				m_GrabMessage.SetText(pickup.ItemToAdd.Name + (pickup.ItemToAdd.CurrentInStack > 1 ? (" x " + pickup.ItemToAdd.CurrentInStack) : ""));
			}
			else if(door != null)
			{
				m_GrabMessage.Toggle(true);
				m_GrabMessage.SetText(door.Open ? "Close the door" : "Open the door");
			}
			else if(bag != null)
			{
				m_GrabMessage.Toggle(true);
				m_GrabMessage.SetText(TimeOfDay.Instance.State.Get() == ET.TimeOfDay.Night ? "Sleep..." : "You can only sleep at night time!");
			}
			else if(lamp != null)
			{
				m_GrabMessage.Toggle(true);
				m_GrabMessage.SetText(string.Format("Turn {0}", !lamp.State ? "<color=yellow>ON</color>" : "<color=red>OFF</color>"));
			}
			else
				m_GrabMessage.Toggle(false);

			if(m_CurrentCrosshair)
			{
				bool showCrosshair = !Player.Aim.Active && openable == null && pickup == null && door == null && bag == null;
				m_CurrentCrosshair.SetActive(showCrosshair);
			}
		}
	}
}
