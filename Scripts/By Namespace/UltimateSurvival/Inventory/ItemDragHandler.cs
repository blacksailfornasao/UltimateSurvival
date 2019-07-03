using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UltimateSurvival.GUISystem;

namespace UltimateSurvival
{
	public delegate void DragAction(PointerEventData data, Slot slot, ItemContainer collection);
	public delegate void DropAction(ItemContainer initialCollection, Slot initialSlot, SavableItem item);

	/// <summary>
	/// 
	/// </summary>
	public class ItemDragHandler : MonoSingleton<ItemDragHandler>
	{
		/// <summary>Raised when the player drops the item on something else than a slot.</summary>
		public event DropAction	PlayerDroppedItem;

		/// <summary></summary>
		public bool IsDragging { get { return m_Dragging; } }

		[SerializeField] 
		private float m_DraggedItemScale = 0.85f;

		[SerializeField] 
		private float m_DraggedItemAlpha = 0.75f;

		[SerializeField] 
		private KeyCode m_SplitKey = KeyCode.LeftShift;

		private ItemContainer[] m_AllCollections;
		private bool m_Dragging;
		private SavableItem	m_DraggedItem;
		private RectTransform m_DraggedItemRT;
		private Canvas m_Canvas;
		private RectTransform m_ParentCanvasRT;
		private Vector2 m_DragOffset;


		private void Start()
		{
			// TODO: We shouldn't have access to GUI functions here.
			m_AllCollections = GUIController.Instance.Containers;
			m_Canvas = GUIController.Instance.Canvas;
			m_ParentCanvasRT = m_Canvas.GetComponent<RectTransform>();

			foreach(var collection in m_AllCollections)
			{
				collection.Slot_BeginDrag += On_Slot_BeginDrag;
				collection.Slot_Drag += On_Slot_Drag;
				collection.Slot_EndDrag += On_Slot_EndDrag;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void On_Slot_BeginDrag(PointerEventData data, Slot slot, ItemContainer collection)
		{
			if(!slot.HasItem || InventoryController.Instance.IsClosed)
				return;

			m_Dragging = true;
			SavableItem itemUnderPointer = slot.CurrentItem;

			// Stack splitting.
			if(Input.GetKey(m_SplitKey) && itemUnderPointer.CurrentInStack > 1)
			{
				int initialAmount = itemUnderPointer.CurrentInStack;
				int half = initialAmount / 2;
				itemUnderPointer.CurrentInStack = initialAmount - half;

				m_DraggedItem = new SavableItem(itemUnderPointer.ItemData, half, itemUnderPointer.CurrentPropertyValues);
				slot.Refresh();
			}
			else
			{
				slot.ItemHolder.SetItem(null);
				m_DraggedItem = itemUnderPointer;
			}

			m_DraggedItemRT = slot.GetDragTemplate(m_DraggedItem, m_DraggedItemAlpha);
			m_DraggedItemRT.SetParent(m_ParentCanvasRT, true);
			m_DraggedItemRT.localScale = Vector3.one * m_DraggedItemScale;

			Camera cam = data.pressEventCamera;
			Vector3 worldPoint;
			if(RectTransformUtility.ScreenPointToWorldPointInRectangle(m_ParentCanvasRT, (Vector3)data.position, cam, out worldPoint)) 
				m_DragOffset = slot.transform.position - worldPoint;

			//Debug.Break();
		}

		/// <summary>
		/// 
		/// </summary>
		private void On_Slot_Drag(PointerEventData data, Slot initialSlot, ItemContainer collection)
		{
			if(!m_Dragging)
				return;

			Camera cam = data.pressEventCamera;
			Vector2 localPoint;
			if(RectTransformUtility.ScreenPointToLocalPointInRectangle(m_ParentCanvasRT, data.position, cam, out localPoint))
				m_DraggedItemRT.localPosition = localPoint + (Vector2)m_ParentCanvasRT.InverseTransformVector(m_DragOffset);
		}
			
		/// <summary>
		/// 
		/// </summary>
		private void On_Slot_EndDrag(PointerEventData data, Slot initialSlot, ItemContainer collection)
		{
			if(!m_Dragging)
				return;

			var slots = collection.Slots;

			GameObject objectUnderPointer = data.pointerCurrentRaycast.gameObject;
			Slot slotUnderPointer = null;
			if(objectUnderPointer)
				slotUnderPointer = objectUnderPointer.GetComponent<Slot>();
			
			// Is there a slot under our pointer?
			if(slotUnderPointer)
			{
				// See if the slot allows this type of item.
				if(slotUnderPointer.AllowsItem(m_DraggedItem))
				{
					// If the slot is empty...
					if(!slotUnderPointer.HasItem)
					{
						if(slotUnderPointer.ItemHolder)
							slotUnderPointer.ItemHolder.SetItem(m_DraggedItem);
						else
							Debug.LogError("You tried to drop an item over a Slot which is not linked with any holder.", this);
					}
					// If the slot is not empty...
					else
					{
						SavableItem itemUnderPointer = slotUnderPointer.CurrentItem;

						// Can we stack the items?
						bool canStackItems = (itemUnderPointer.Id == m_DraggedItem.Id && itemUnderPointer.ItemData.StackSize > 1 && itemUnderPointer.CurrentInStack < itemUnderPointer.ItemData.StackSize);
						if(canStackItems)
							OnItemsAreStackable(slotUnderPointer, initialSlot);	
						else
							On_ItemNotStackable(slotUnderPointer, initialSlot);
					}
				}
				else
					PutItemBack(initialSlot);

				initialSlot.Refresh();
			}
			// If the player didn't drop it on a slot...
			else
			{
				if(PlayerDroppedItem != null)
					PlayerDroppedItem(collection, initialSlot, m_DraggedItem);

				if(!InventoryController.Instance.Try_DropItem(m_DraggedItem))
					collection.TryAddItem(m_DraggedItem);
			}

			Destroy(m_DraggedItemRT.gameObject);
			m_DraggedItem = null;
			m_Dragging = false;

			//initialSlot.interactable = initialSlot.HasItem;
		}

		/// <summary>
		/// 
		/// </summary>
		private void OnItemsAreStackable(Slot slotUnderPointer, Slot initialSlot)
		{
			// Add as much as possible to the item's stack.
			int added;
			slotUnderPointer.ItemHolder.TryAddItem(m_DraggedItem.ItemData, m_DraggedItem.CurrentInStack, out added, m_DraggedItem.CurrentPropertyValues);

			// Add the remained items too.
			int remainedToAdd = m_DraggedItem.CurrentInStack - added;
			if(remainedToAdd > 0)
			{
				if(initialSlot.HasItem)
					slotUnderPointer.Parent.TryAddItem(m_DraggedItem.ItemData, remainedToAdd);
				else
					initialSlot.ItemHolder.SetItem(new SavableItem(m_DraggedItem.ItemData, remainedToAdd, m_DraggedItem.CurrentPropertyValues));
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void On_ItemNotStackable(Slot slotUnderPointer, Slot initialSlot)
		{
			if(!initialSlot.AllowsItem(slotUnderPointer.CurrentItem))
			{
				PutItemBack(initialSlot);
				return;
			}

			// Swap the items because they are of different kinds / not stackable / reached maximum stack size.
			SavableItem temp = slotUnderPointer.CurrentItem;
			if(!initialSlot.HasItem)
			{
				slotUnderPointer.ItemHolder.SetItem(m_DraggedItem);
				initialSlot.ItemHolder.SetItem(temp);
			}
			else
			{
				// Add as much as possible to the item's stack.
				int added;
				initialSlot.Parent.TryAddItem(m_DraggedItem.ItemData, m_DraggedItem.CurrentInStack, out added);

				// Add the remained items too.
				int remainedToAdd = m_DraggedItem.CurrentInStack - added;
				if(remainedToAdd > 0)
					initialSlot.Parent.TryAddItem(m_DraggedItem.ItemData, remainedToAdd);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void PutItemBack(Slot initialSlot)
		{
			if(initialSlot.HasItem)
				initialSlot.Parent.TryAddItem(m_DraggedItem);
			else
				initialSlot.ItemHolder.SetItem(m_DraggedItem);
		}
	}
}
