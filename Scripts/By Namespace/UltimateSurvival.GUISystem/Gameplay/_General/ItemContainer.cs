using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UltimateSurvival.GUISystem
{
	/// <summary>
	/// Used to display an item container graphically.
	/// </summary>
	public sealed class ItemContainer : GUIBehaviour
	{
		/// <summary> Sent when a slot displayer is refreshed (Usually when it's item is updated / removed / added).</summary>
		public Message<Slot> Slot_Refreshed = new Message<Slot>();

		/// <summary> Raised when a slot displayer that is part of this collection, has the pointer down on it. </summary>
		public event Action<PointerEventData, Slot> Slot_PointerDown;

		/// <summary> Raised when a slot displayer that is part of this collection, has the pointer up on it. </summary>
		public event Action<PointerEventData, Slot> Slot_PointerUp;

		/// <summary> Raised when a slot displayer that is part of this collection, is selected. </summary>
		public event Action<BaseEventData, Slot> Slot_Select;

		/// <summary> Raised when IBeginDragHandler.OnBeginDrag is called on a child slot displayer. </summary>
		public event DragAction Slot_BeginDrag;

		/// <summary> Raised when IDragHandler.OnDrag is called on a child slot displayer. </summary>
		public event DragAction Slot_Drag;

		/// <summary> Raised when IEndDragHandler.OnEndDrag is called on a child slot displayer. </summary>
		public event DragAction Slot_EndDrag;

		public bool IsOpen { get { return m_SetUp && (!m_Window || m_Window.IsOpen); } }

		public string Name { get { return _Name; } }

		public List<Slot> Slots { get; private set; }

		[SerializeField] 
		private string _Name = "";
	
		[SerializeField]
		[Tooltip("It is optional. If you assign a window, the open state will be taken from the window, otherwise the container will always be considered open.")]
		private Window m_Window;

		[Header("Slots")]

		[SerializeField]
		[Tooltip("All the created slots will be based on this template.")]
		private Slot m_SlotTemplate;

		[SerializeField]
		[Tooltip("The parent of the slots, usually it has attached a GridLayoutGroup, HorizontalLayoutGroup, etc, so they are automatically arranged.")]
		private Transform m_SlotsParent;

		[SerializeField]
		[Range(0, 100)]
		private int m_PreviewSize;
	
		[Header("Required Stuff")]

		[SerializeField]
		[Reorderable]
		private ReorderableStringList m_RequiredCategories;

		[SerializeField]
		[Reorderable]
		private ReorderableStringList m_RequiredProperties;

		private List<ItemHolder> m_ItemHolders;
		private List<Slot> m_Slots;
		private bool m_SetUp;


		public void Setup(ItemHolder itemholder)
		{
			Setup(new List<ItemHolder>() { itemholder });
		}

		public void Setup(List<ItemHolder> itemHolders)
		{
			if(!Application.isPlaying)
			{
				Debug.LogError("You can't create the container when the application is not playing.", this);
				return;
			}

			if(m_SlotTemplate == null)
			{
				Debug.LogError("You tried to create slots for this container, but the slot template is null / not assigned in the inspector!", this);
				return;
			}

			if(!m_SlotsParent || !m_SlotTemplate)
				return;

			if(Slots == null)
			{
				Slots = new List<Slot>();
				GetComponentsInChildren<Slot>(Slots);
			}

			m_ItemHolders = itemHolders;

			bool previousState = m_SlotTemplate.gameObject.activeSelf;
			m_SlotTemplate.gameObject.SetActive(true);

			PrepareGUIForSlots(m_SlotsParent, m_SlotTemplate);

			m_SlotTemplate.gameObject.SetActive(previousState);

			m_SetUp = true;
		}

		public bool HasItem(SavableItem item)
		{
			for(int i = 0;i < m_ItemHolders.Count;i ++)
				if(item == m_ItemHolders[i])
					return true;

			return false;
		}

		/// <summary>
		/// Tries to add an amount of items in this collection.
		/// </summary>
		/// <param name="itemData"> The data of the item you want to add. </param>
		/// <param name="amount"> How many items of this type you want to add?. </param>
		/// <param name="added"> This value represents the amount of items that were added, if the collection was almost full, some of the items might've not been added. </param>
		public bool TryAddItem(ItemData itemData, int amount, out int added)
		{
			added = 0;
			CollectionUtils.AddItem(itemData, amount, Slots, out added);

			//if(added != amount)
				//InventoryController.Instance.Try_DropItem(new SavableItem(itemData, amount - added));

			return added > 0;
		}

		/// <summary>
		/// Tries to add an amount of items in this collection.
		/// </summary>
		/// <param name="itemData">The data of the item you want to add.</param>
		/// <param name="amount">How many items of this type you want to add?.</param>
		/// <param name="added">This value represents the amount of items were added, if the collection was almost full, some of the items might've not been added.</param>
		public bool TryAddItem(ItemData itemData, int amount)
		{
			int added = 0;
			CollectionUtils.AddItem(itemData, amount, Slots, out added);

			//if(added != amount)
			//	InventoryController.Instance.Try_DropItem(new SavableItem(itemData, amount - added));

			return added > 0;
		}

		public bool TryAddItem(string name, int amount, out int added)
		{
			added = 0;
			ItemData itemData;
			if(InventoryController.Instance.Database.FindItemByName(name, out itemData))
				CollectionUtils.AddItem(itemData, amount, Slots, out added);

			//if(added != amount)
			//	InventoryController.Instance.Try_DropItem(new SavableItem(itemData, amount - added));

			return added > 0;
		}

		public bool TryAddItem(string name, int amount)
		{
			int added = 0;
			ItemData itemData;
			if(InventoryController.Instance.Database.FindItemByName(name, out itemData))
				CollectionUtils.AddItem(itemData, amount, Slots, out added);

			//if(added != amount)
			//	InventoryController.Instance.Try_DropItem(new SavableItem(itemData, amount - added));

			return added > 0;
		}

		/// <summary>
		/// Tries to add a specific item in this collection.
		/// </summary>
		/// <param name="item">The runtime representation of the item.</param>
		public bool TryAddItem(SavableItem item)
		{
			if(item == null)
				return false;

			int added = 0;
			CollectionUtils.AddItem(item.ItemData, item.CurrentInStack, Slots, out added, item.CurrentPropertyValues);

			//if(added != item.CurrentInStack)
			//	InventoryController.Instance.Try_DropItem(new SavableItem(item.ItemData, item.CurrentInStack - added, item.CurrentPropertyValues));

			return added > 0;
		}

		/// <summary>
		/// Removes a specific item, if it's found in this collection.
		/// </summary>
		public bool TryRemoveItem(SavableItem item)
		{
			Slot targetSlot = Slots.Find((Slot slot)=> { return slot.CurrentItem == item; });
			if(targetSlot)
			{
				targetSlot.ItemHolder.SetItem(null);
				return true;
			}
		
			return false;
		}

		/// <summary>
		/// Removes a specific amount of items, of a specific type.
		/// </summary>
		public void RemoveItems(string itemName, int amount, out int removed)
		{
			CollectionUtils.RemoveItems(itemName, amount, Slots, out removed);
		}

		/// <summary>
		/// Removes a specific amount of items, of a specific type.
		/// </summary>
		public void RemoveItems(string itemName, int amount)
		{
			int removed;
			CollectionUtils.RemoveItems(itemName, amount, Slots, out removed);
		}

		public void AddAllFrom(ItemContainer container)
		{
			for(int i = 0;i < container.Slots.Count;i ++)
			{
				if(container.Slots[i].HasItem)
				{
					bool added = TryAddItem(container.Slots[i].CurrentItem);
					if(added)
						container.Slots[i].ItemHolder.SetItem(null);
				}
			}
		}

		public int GetItemCount(int itemID)
		{
			int count = 0;
			for(int i = 0;i < Slots.Count;i ++)
				if(Slots[i].HasItem && Slots[i].CurrentItem.Id == itemID)
					count += Slots[i].CurrentItem.CurrentInStack;

			return count;
		}

		public int GetItemCount(string itemName)
		{
			int count = 0;
			for(int i = 0;i < Slots.Count;i ++)
				if(Slots[i].HasItem && Slots[i].CurrentItem.ItemData.Name == itemName)
					count += Slots[i].CurrentItem.CurrentInStack;

			return count;
		}

		public void ApplyAll()
		{
			if(Application.isPlaying)
				return;
			
			ApplyTemplate();
			ApplyRequiredStuff();
		}

		public void ApplyTemplate()
		{
			if(Application.isPlaying)
				return;

			Transform parent = m_SlotsParent;
			Slot template = m_SlotTemplate;

			if(!parent || !template)
				return;

			bool previousState = template.gameObject.activeSelf;
			template.gameObject.SetActive(true);

			RemoveSlots(parent, template);
			CreateSlots(parent, template);

			template.gameObject.SetActive(previousState);
		}

		public void ApplyRequiredStuff()
		{
			if(Application.isPlaying)
				return;

			foreach(var slot in GetComponentsInChildren<Slot>())
			{
				slot.RequiredCategories = m_RequiredCategories;
				slot.RequiredProperties = m_RequiredProperties;
			}
		}

		private void Awake()
		{
			Slots = new List<Slot>();
			GetComponentsInChildren<Slot>(Slots);
		}

		private void On_Slot_PointerDown(PointerEventData data, Slot slot)
		{
			if(Slot_PointerDown != null)
				Slot_PointerDown(data, slot);

			if(Input.GetKey(KeyCode.LeftShift) && data.button == PointerEventData.InputButton.Left)
			{
				//print("pressed shift and mouse0");

				if(_Name == "Inventory")
				{
					var lootContainer = Controller.GetContainer("Loot");
					if(lootContainer.IsOpen)
					{
						bool added = lootContainer.TryAddItem(slot.CurrentItem);
						if(added)
							slot.ItemHolder.SetItem(null);
					}
					else
					{
						var hotbarContainer = Controller.GetContainer("Hotbar");
						bool added = hotbarContainer.TryAddItem(slot.CurrentItem);
						if(added)
							slot.ItemHolder.SetItem(null);
					}
				}
				else
				{
					var inventoryContainer = Controller.GetContainer("Inventory");
					bool added = inventoryContainer.TryAddItem(slot.CurrentItem);
					if(added)
						slot.ItemHolder.SetItem(null);
				}
			}
		}

		private void On_Slot_PointerUp(PointerEventData data, Slot slot)
		{
			if(Slot_PointerUp != null)
				Slot_PointerUp(data, slot);
		}
		private void On_Slot_Select(BaseEventData data, Slot slot)
		{
			if(Slot_Select != null)
				Slot_Select(data, slot);
		}

		private void On_Slot_BeginDrag(PointerEventData data, Slot slot)
		{
			if(Slot_BeginDrag != null)
				Slot_BeginDrag(data, slot, this);
		}

		private void On_Slot_Drag(PointerEventData data, Slot slot)
		{
			if(Slot_Drag != null)
				Slot_Drag(data, slot, this);
		}

		private void On_Slot_EndDrag(PointerEventData data, Slot slot)
		{
			if(Slot_EndDrag != null)
				Slot_EndDrag(data, slot, this);
		}

		private void On_Slot_Refreshed(Slot slot)
		{
			Slot_Refreshed.Send(slot);
		}

		private void ActivateSlots(Transform parent, Slot template, int count, bool active)
		{
			for(int i = 0;i < count;i ++)
			{
				int currentIndex = parent.childCount - i - 1;
				if(parent.GetChild(currentIndex) != template.transform)
					parent.GetChild(currentIndex).gameObject.SetActive(active);
			}
		}

		private void PrepareGUIForSlots(Transform parent, Slot template)
		{
			OnSlotsDiscarded();

			// Enable as many slots as required to satisfy the opened collection.
			ActivateSlots(parent, template, Mathf.Clamp(m_ItemHolders.Count, 0, Slots.Count), true);

			// If more slots are needed, create them.
			if(m_ItemHolders.Count > Slots.Count)
			{
				int spawnCount = m_ItemHolders.Count - Slots.Count;

				// Create new ones if it's required.
				for(int i = 0;i < spawnCount;i ++)
				{
					var slot = Instantiate<Slot>(template);
					slot.transform.SetParent(parent);
					slot.transform.localPosition = Vector3.zero;
					slot.transform.localScale = Vector3.one;

					Slots.Add(slot);
				}
			}
			// If there are too many slots, disable the surplus.
			else if(m_ItemHolders.Count < Slots.Count)
				ActivateSlots(parent, template, Slots.Count - m_ItemHolders.Count, false);

			//print(string.Format("Slots Data Count: {0} | Slots Count: {1}", m_SlotsData.Count, Slots.Count));

			for(int i = 0;i < m_ItemHolders.Count;i ++)
				Slots[i].LinkWithHolder(m_ItemHolders[i]);

			OnSlotsCreated();
		}

		private void OnSlotsDiscarded()
		{
			foreach(var slot in Slots)
			{
				slot.PointerDown -= On_Slot_PointerDown;
				slot.PointerUp -= On_Slot_PointerUp;
				slot.E_Select -= On_Slot_Select;
				slot.BeginDrag -= On_Slot_BeginDrag;
				slot.Drag -= On_Slot_Drag;
				slot.EndDrag -= On_Slot_EndDrag;
				slot.Refreshed.RemoveListener(On_Slot_Refreshed);
			}
		}

		private void OnSlotsCreated()
		{
			foreach(var slot in Slots)
			{
				slot.PointerDown += On_Slot_PointerDown;
				slot.PointerUp += On_Slot_PointerUp;
				slot.E_Select += On_Slot_Select;
				slot.BeginDrag += On_Slot_BeginDrag;
				slot.Drag += On_Slot_Drag;
				slot.EndDrag += On_Slot_EndDrag;
				slot.Refreshed.AddListener(On_Slot_Refreshed);
			}
		}

		private void RemoveSlots(Transform parent, Slot template)
		{
			// Remove the current slots.
			int childCount = parent.childCount;

			for(int i = 0;i < childCount;i ++)
				if(parent.GetChild(parent.childCount - 1) != template.transform)
					DestroyImmediate(parent.GetChild(parent.childCount - 1).gameObject);
		}

		private void CreateSlots(Transform parent, Slot template)
		{
			// Create the new ones.
			for(int i = 0;i < m_PreviewSize;i ++)
			{
				var slot = Instantiate<Slot>(template);
				slot.transform.SetParent(parent);
				slot.transform.localPosition = Vector3.zero;
				slot.transform.localScale = Vector3.one;
			}
		}
	}
}
