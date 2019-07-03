using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UltimateSurvival.GUISystem;

namespace UltimateSurvival
{
	/// <summary>
	///
	/// </summary>
	public class InventoryController : MonoSingleton<InventoryController>
	{
		/// <summary></summary>
		public Value<ET.InventoryState> State = new Value<ET.InventoryState>(ET.InventoryState.Closed);

		/// <summary></summary>
		public Attempt<ET.InventoryState> SetState = new Attempt<ET.InventoryState>();

		/// <summary></summary>
		public Attempt<SmeltingStation> OpenFurnace = new Attempt<SmeltingStation>();

		/// <summary></summary>
		public Attempt<SmeltingStation> OpenCampfire = new Attempt<SmeltingStation>();

		/// <summary></summary>
		public Attempt<LootObject> OpenLootContainer = new Attempt<LootObject>();
		 
		/// <summary></summary>
		public Attempt<Anvil> OpenAnvil = new Attempt<Anvil>();

		/// <summary>This is an attempt to add a new item to the crafting queue.</summary>
		public Attempt<CraftData> CraftItem = new Attempt<CraftData>();

		/// <summary></summary>
		public Message<ItemHolder> EquipmentChanged = new Message<ItemHolder>(); 

		/// <summary>Is the inventory closed?</summary>
		public bool IsClosed { get { return State.Is(ET.InventoryState.Closed); } }

		/// <summary>Stores all the item and recipe definitions.</summary>
		public ItemDatabase Database { get { return m_ItemDatabase; } }

		[SerializeField]
		[Tooltip("The inventory cannot function without this, as some operations, like ADD, LOAD require a database.")]
		private ItemDatabase m_ItemDatabase;

		[Header("Item Collections")]

		[SerializeField]
		[Range(1, 48)]
		private int m_InventorySize = 24;

		[SerializeField]
		[Range(1, 12)]
		private int m_HotbarSize = 6;

		[SerializeField]
		[Reorderable]
		private ReorderableStringList m_EquipmentList;

		[Header("Item Drop")]

		[SerializeField]
		private Vector3 m_DropOffset = new Vector3(0f, 0f, 0.8f);

		[SerializeField]
		private float m_DropAngularFactor = 150f;

		[SerializeField]
		private float m_DropSpeed = 8f;

		private PlayerEventHandler m_Player;
		private ItemContainer[] m_AllCollections;
		private float m_LastTimeToggledInventory;

		private List<ItemHolder> m_InventoryCollection;
		private List<ItemHolder> m_HotbarCollection;
		private List<ItemHolder> m_EquipmentHolders;


		/// <summary>
		/// Tries to add an amount of items in a certain collection.
		/// </summary>
		/// <param name="itemID"> The name of the item you want to add. </param>
		/// <param name="amount"> How many items of this type you want to add?. </param>
		/// <param name="collection"> Name the collection in which you want the item to be added, eg."Inventory". </param>
		/// <param name="added"> This value represents the amount of items were added, if the collection was almost full, some of the items might've not been added. </param>
		public bool AddItemToCollection(int itemID, int amount, string collection, out int added)
		{
			added = 0;

			if(!enabled)
				return false;
			
			for(int i = 0;i < m_AllCollections.Length;i ++)
			{
				if(m_AllCollections[i].Name == collection)
				{
					// Will be true if at least one item was added.
					bool wasAdded = false;
					ItemData itemData;

					if(m_ItemDatabase.FindItemById(itemID, out itemData))
						wasAdded = m_AllCollections[i].TryAddItem(itemData, amount, out added);

					return wasAdded;
				}
			}

			Debug.LogWarningFormat(this, "No collection with the name '{0}' was found! No item added.", collection);
			return false;
		}

		/// <summary>
		/// Tries to add an amount of items in a certain collection.
		/// </summary>
		/// <param name="itemID">The id of the item you want to add.</param>
		/// <param name="amount">How many items of this type you want to add?.</param>
		/// <param name="collection">Name the collection in which you want the item to be added, eg."Inventory".</param>
		/// <param name="added">This value represents the amount of items were added, if the collection was almost full, some of the items might've not been added.</param>
		public bool AddItemToCollection(string itemName, int amount, string collection, out int added)
		{
			added = 0;

			if(!enabled)
				return false;

			for(int i = 0;i < m_AllCollections.Length;i ++)
			{
				if(m_AllCollections[i].Name == collection)
				{
					// Will be true if at least one item was added.
					bool wasAdded = false;
					ItemData itemData;

					if(m_ItemDatabase.FindItemByName(itemName, out itemData))
						wasAdded = m_AllCollections[i].TryAddItem(itemData, amount, out added);

					return wasAdded;
				}
			}

			Debug.LogWarningFormat(this, "No collection with the name '{0}' was found! No item added.", collection);
			return false;
		}

		public int GetItemCount(string name)
		{
			// TODO: We shouldn't have access to GUI functions here.
			return GUIController.Instance.GetContainer("Inventory").GetItemCount(name);
		}

		/// <summary>
		/// Removes the item if it exists in the inventory, if not, the method will return false.
		/// </summary>
		public bool TryRemoveItem(SavableItem item)
		{
			if(!enabled)
				return false;
			
			for(int i = 0;i < m_AllCollections.Length;i ++)
			{
				bool removed = m_AllCollections[i].TryRemoveItem(item);
				if(removed)
					return true;
			}

			return false;
		}

		public void RemoveItems(string itemName, int amount = 1)
		{
			// TODO: We shouldn't have access to GUI functions here.
			var inventory = GUIController.Instance.GetContainer("Inventory");
			inventory.RemoveItems(itemName, amount);
		}

		public bool Try_DropItem(SavableItem item, Slot parentSlot = null)
		{
			if(item && item.ItemData.WorldObject && !item.ItemData.IsBuildable)
			{
				var cameraTransform = GameController.WorldCamera.transform;
				GameObject droppedItem = Instantiate(item.ItemData.WorldObject, cameraTransform.position + cameraTransform.TransformVector(m_DropOffset), Random.rotation) as GameObject;

				var rigidbody = droppedItem.GetComponent<Rigidbody>();
				if(rigidbody)
				{
					rigidbody.angularVelocity = Random.rotation.eulerAngles * m_DropAngularFactor;
					rigidbody.AddForce(cameraTransform.forward * m_DropSpeed, ForceMode.VelocityChange);

					Physics.IgnoreCollision(m_Player.GetComponent<Collider>(), droppedItem.GetComponent<Collider>());
				}

				var pickup = droppedItem.GetComponent<ItemPickup>();
				if(pickup)
					pickup.ItemToAdd = item;

				if(parentSlot)
					parentSlot.ItemHolder.SetItem(null);

				return true;
			}

			return false;
		}

		public List<ItemHolder> GetEquipmentHolders()
		{
			return m_EquipmentHolders;
		}

		private void Awake()
		{
			if(!m_ItemDatabase)
			{
				Debug.LogError("No ItemDatabase specified, the inventory will be disabled!", this);
				enabled = false;
				return;
			}

			SetState.SetTryer(TryChange_State);

			// TODO: We shouldn't have access to GUI functions here.
			m_AllCollections = GUIController.Instance.Containers;

			// Create the inventory.
			// TODO: We shouldn't have access to GUI functions here.
			m_InventoryCollection = CreateListOfHolders(m_InventorySize);
			var inventoryCollection = GUIController.Instance.GetContainer("Inventory");
			inventoryCollection.Setup(m_InventoryCollection);

			// Create the hotbar.
			// TODO: We shouldn't have access to GUI functions here.
			m_HotbarCollection = CreateListOfHolders(m_HotbarSize);
			var hotbarCollection = GUIController.Instance.GetContainer("Hotbar");
			hotbarCollection.Setup(m_HotbarCollection);

			// Create the equipment.
			// TODO: We shouldn't have access to GUI functions here.
			m_EquipmentHolders = CreateListOfHolders(m_EquipmentList.Count);
			for(int i = 0;i < m_EquipmentList.Count;i ++)
			{
				var equipmentGUI = GUIController.Instance.GetContainer(m_EquipmentList[i]);
				if(equipmentGUI)
					equipmentGUI.Setup(new List<ItemHolder>() { m_EquipmentHolders[i] });
				else
					Debug.LogErrorFormat(this, "No GUI collection with the name '{0}' was found!", m_EquipmentList[i]);
			}

			m_Player = GameController.LocalPlayer;
			m_Player.ChangeHealth.AddListener(OnChanged_PlayerHealth);
			m_Player.Death.AddListener(On_PlayerDeath);
		}

		private void OnChanged_PlayerHealth(HealthEventData data)
		{
			if(data.Delta < 0f)
				for(int i = 0; i < m_EquipmentHolders.Count; i ++) 
				{
					if(m_EquipmentHolders[i].HasItem && m_EquipmentHolders[i].CurrentItem.HasProperty("Durability"))
					{
						var durabilityProp = m_EquipmentHolders[i].CurrentItem.GetPropertyValue("Durability");
						var floatVal = durabilityProp.Float;
						floatVal.Current --;

						durabilityProp.SetValue(ItemProperty.Type.Float, floatVal);

						if(floatVal.Current <= 0f)
							m_EquipmentHolders[i].SetItem(null);
					}
				}
		}

		private void On_PlayerDeath()
		{
			if(State.Get() != ET.InventoryState.Closed)
				SetState.Try(ET.InventoryState.Closed);

			RemoveItemsFromCollection("Inventory");
			RemoveItemsFromCollection("Hotbar");

			foreach(var col in m_EquipmentList)
				RemoveItemsFromCollection(col);
		}

		private void RemoveItemsFromCollection(string collection)
		{
			// TODO: We shouldn't have access to GUI functions here.
			var wrapper = GUIController.Instance.GetContainer(collection);

			if(!wrapper)
				return;

			foreach(var slot in wrapper.Slots)
			{
				if(slot.HasItem)
					slot.ItemHolder.SetItem(null);
			}
		}

		private void DropItemsFromCollection(string collection)
		{
			var container = GUIController.Instance.GetContainer(collection);

			if(!container)
				return;

			foreach(var slot in container.Slots)
			{
				if(slot.HasItem)
				{
					if(slot.CurrentItem.ItemData.IsBuildable)
						slot.ItemHolder.SetItem(null);
					else
						Try_DropItem(slot.CurrentItem, slot);
				}
			}
		}

		private bool TryChange_State(ET.InventoryState state)
		{
			bool stateWasChanged = false;

			if(Time.time > m_LastTimeToggledInventory + 0.5f)
			{
				m_LastTimeToggledInventory = Time.time;
				stateWasChanged = true;
			}
				
			if(stateWasChanged)
			{
				State.Set(state);
				UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
			}

			return stateWasChanged;
		}

		private List<ItemHolder> CreateListOfHolders(int size)
		{
			var slots = new List<ItemHolder>();

			for(int i = 0;i < size;i ++)
				slots.Add(new ItemHolder());

			return slots;
		}
	}
}
