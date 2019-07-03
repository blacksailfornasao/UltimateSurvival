using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UltimateSurvival.GUISystem
{
	/// <summary>
	/// 
	/// </summary>
	[RequireComponent(typeof(ItemContainer))]
	public class Hotbar : GUIBehaviour
	{
		[SerializeField]
		[Range(0, 100)]
		private int m_FirstSelected;

		[Header("Navigation")]

		[SerializeField]
		private bool m_EnableScrolling = true;

		[SerializeField]
		[HideSwitchAttribute("m_EnableScrolling", true)]
		[ClampAttribute(0f, 10f)]
		private float m_ScrollThreeshold = 0.3f;

		[SerializeField]
		private bool m_SelectByDigits = true;

		[Header("Selection Graphics")]

		[SerializeField] 
		private float m_SelectedSlotScale = 1f;

		[SerializeField]
		private Color m_FrameColor = Color.cyan;

		[SerializeField]
		private Sprite	m_FrameSprite;

		private ItemContainer m_HotbarContainer;
		private List<Slot> m_HotbarSlots;

		private Slot m_SelectedSlot;
		private int m_LastIndex;
		private float m_CurScrollValue;

		private Image m_Frame;
	

		private void Awake()
		{
			m_HotbarContainer = GetComponent<ItemContainer>();

			m_HotbarContainer.Slot_Refreshed.AddListener(On_Slot_Refreshed);
			m_HotbarContainer.Slot_PointerUp += On_Slot_PointerUp;
		}

		private IEnumerator Start()
		{
			Player.DestroyEquippedItem.SetTryer(Try_DestroyEquippedItem);
			Player.Sleep.AddStopListener(()=> TrySelectSlot(0));

			m_HotbarSlots = m_HotbarContainer.Slots;

			m_Frame = GUIUtils.CreateImageUnder("Frame", GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
			m_Frame.sprite = m_FrameSprite;
			m_Frame.color = m_FrameColor;
			m_Frame.enabled = false;

			yield return null;

			TrySelectSlot(m_FirstSelected);
		}

		private bool Try_DestroyEquippedItem()
		{
			var equippedItem = Player.EquippedItem.Get();

			if(equippedItem && m_HotbarContainer.HasItem(equippedItem))
				Player.ChangeEquippedItem.Try(null, true);

			return m_HotbarContainer.TryRemoveItem(equippedItem);
		}

		private void On_Slot_PointerUp (PointerEventData data, Slot displayer)
		{
			bool canSelectDisplayer = displayer != m_SelectedSlot && data.pointerCurrentRaycast.gameObject == displayer.gameObject;
			if(canSelectDisplayer)
				TrySelectSlot(m_HotbarSlots.IndexOf(displayer));
		}

		private void On_Slot_Refreshed (Slot slot)
		{
			if(slot == m_SelectedSlot)
				TrySelectSlot(m_HotbarSlots.IndexOf(m_SelectedSlot));
		}
			
		private void Update()
		{
			if(m_SelectByDigits && !Player.SelectBuildable.Active)
			{
				if(Input.anyKeyDown)
				{
					int keyNumber;
					if(int.TryParse(Input.inputString, out keyNumber))
						TrySelectSlot(keyNumber - 1);
				}
			}

			if(m_EnableScrolling && !Player.SelectBuildable.Active && InventoryController.Instance.IsClosed)
			{
				var playerScrollValue = Player.ScrollValue.Get();

				m_CurScrollValue = Mathf.Clamp(m_CurScrollValue + playerScrollValue, -m_ScrollThreeshold, m_ScrollThreeshold);

				if(Mathf.Abs(m_CurScrollValue - m_ScrollThreeshold * Mathf.Sign(playerScrollValue)) < Mathf.Epsilon)
				{
					m_CurScrollValue = 0f;
					m_LastIndex = (int)Mathf.Repeat(m_LastIndex + (playerScrollValue >= 0f ? 1 : -1), m_HotbarSlots.Count);

					TrySelectSlot(m_LastIndex);
				}
			}
		}
			
		private void TrySelectSlot(int index)
		{
			index = Mathf.Clamp(index, 0, m_HotbarSlots.Count - 1);

			var newSlot = m_HotbarSlots[index];
			Player.ChangeEquippedItem.Try(newSlot.CurrentItem, false);
			
			// Deselect the old slot.
			if(m_SelectedSlot)
				m_SelectedSlot.SetScale(Vector3.one, 0.2f);

			// And select the new one.
			m_SelectedSlot = newSlot;
			m_SelectedSlot.SetScale(Vector3.one * m_SelectedSlotScale, 0.1f);

			m_Frame.enabled = true;
			var frameRT = m_Frame.GetComponent<RectTransform>();
			frameRT.SetParent(m_SelectedSlot.transform);
			frameRT.localPosition = Vector2.zero;
			frameRT.sizeDelta = m_SelectedSlot.GetComponent<RectTransform>().sizeDelta;
			frameRT.localScale = Vector3.one;
		}
	}
}