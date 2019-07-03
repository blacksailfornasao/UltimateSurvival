using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UltimateSurvival.GUISystem
{
	public class ItemInspector : GUIBehaviour
	{
		public Slot InspectedSlot { get; private set; }

		[Header("Setup")]

		[SerializeField] 
		private Camera m_GUICamera;

		[SerializeField] 
		private Window m_Window;

		[Header("Item Info")]

		[SerializeField] 
		private Text m_ItemName;

		[SerializeField] 
		private Text m_MainDescription;

		[SerializeField] 
		private Text m_SecondaryDescription;

		[SerializeField] 
		private Image m_Icon;

		[SerializeField] 
		private DurabilityBar m_DurabilityBar;

		[SerializeField] 
		private Text m_Magazine;

		[Header("Actions")]

		[SerializeField]
		private Button m_DropButton;

		[SerializeField]
		private Button m_ConsumeButton;

		[SerializeField]
		private Button m_DismantleButton;

		[Header("Audio")]

		[SerializeField]
		private SoundPlayer m_ItemDropAudio;

		[SerializeField]
		private SoundPlayer m_DismantleAudio;

		[SerializeField]
		private float m_ConsumeVolume = 0.6f;

		private ItemContainer[] m_InspectableContainers;
		private ItemContainer m_InventoryContainer;
		private float m_LastConsumeTime;


		private void Awake()
		{
			m_DismantleButton.onClick.AddListener(On_DismantleClicked);
			m_DropButton.onClick.AddListener(On_DropClicked);
			m_ConsumeButton.onClick.AddListener(On_ConsumeClicked);
		}

		private void Start()
		{
			m_InspectableContainers = Controller.Containers;

			foreach(var container in m_InspectableContainers)
				container.Slot_PointerUp += On_Slot_PointerUp;
			
			m_InventoryContainer = Controller.GetContainer("Inventory");
		}

		private void On_Slot_PointerUp(BaseEventData data, Slot slot)
		{
			bool shouldOpenUp = 
				!InventoryController.Instance.IsClosed &&
				slot.HasItem && 
				EventSystem.current.currentSelectedGameObject == slot.gameObject;

			if(shouldOpenUp)
			{
				if(m_Window)
					m_Window.Open();

				InspectedSlot = slot;
				ShowInfo(slot.CurrentItem);

				InspectedSlot.E_Deselect += (BaseEventData d, Slot sd)=> StartCoroutine(C_WaitAndSelect());
				InspectedSlot.ItemHolder.Updated.AddListener(On_InspectedHolderUpdated);
			}
			else
			{
				StopAllCoroutines();
				StartCoroutine(C_WaitAndSelect());
			}
		}

		private void ShowInfo(SavableItem item)
		{
			// Name.
			m_ItemName.text = (item.ItemData.DisplayName == string.Empty) ? item.ItemData.Name : item.ItemData.DisplayName;

			// Main description.
			if(item.ItemData.Descriptions.Length > 0)
				m_MainDescription.text = item.GetDescription(0);
			else
				m_MainDescription.text = "";

			// Secondary description.
			if(item.ItemData.Descriptions.Length > 1)
				m_SecondaryDescription.text = item.GetDescription(1);
			else
				m_SecondaryDescription.text = "";

			// Icon.
			m_Icon.sprite = item.ItemData.Icon;

			// Durability bar.
			if(item.HasProperty("Durability"))
			{
				if(!m_DurabilityBar.Active)
					m_DurabilityBar.SetActive(true);
				
				m_DurabilityBar.SetFillAmount(item.GetPropertyValue("Durability").Float.Ratio);
			}
			else if(m_DurabilityBar.Active)
				m_DurabilityBar.SetActive(false);

			// Magazine.
			ItemProperty.Value property;
			if(item.FindPropertyValue("Magazine", out property))
			{
				var magazine = property.IntRange;
				m_Magazine.text = "Magazine: " + magazine.ToString();
			}
			else
				m_Magazine.text = "";

			// Consume action.
			m_ConsumeButton.gameObject.SetActive(item.HasProperty("Can Consume"));

			// Dismantle action.
			m_DismantleButton.gameObject.SetActive(item.HasProperty("Can Dismantle"));
		}

		private void On_InspectedHolderUpdated(ItemHolder holder)
		{
			if(!holder.HasItem)
			{
				m_Window.Close();

				try
				{
					InspectedSlot.ItemHolder.Updated.RemoveListener(On_InspectedHolderUpdated);
					InspectedSlot = null;
				}
				catch
				{
					// HACK Don't know why it gives null reference object error...
				}
			}
		}

		private void On_DismantleClicked()
		{
			var item = InspectedSlot.CurrentItem;
			InspectedSlot.ItemHolder.SetItem(null);

			var requiredItems = item.ItemData.Recipe.RequiredItems;
			for(int i = 0;i < requiredItems.Length;i ++)
			{
				var amountToGive = Mathf.RoundToInt(requiredItems[i].Amount * item.GetPropertyValue("Durability").Float.Ratio * 0.6f) + 1;
				m_InventoryContainer.TryAddItem(requiredItems[i].Name, amountToGive);
			}

			MessageDisplayer.Instance.PushMessage(string.Format("<color=yellow>{0}</color> has been dismantled", item.Name));
			m_DismantleAudio.Play2D();
		}

		private void On_DropClicked()
		{
			var equippedItem = Controller.Player.EquippedItem.Get();
			var itemToDrop = InspectedSlot.CurrentItem;

			if(equippedItem == itemToDrop)
				Controller.Player.ChangeEquippedItem.Try(null, true);

			if(InventoryController.Instance.Try_DropItem(InspectedSlot.CurrentItem, InspectedSlot))
			{
				m_ItemDropAudio.Play2D();
				EventSystem.current.SetSelectedGameObject(null);
			}
		}

		private void On_ConsumeClicked()
		{
			var item = InspectedSlot.CurrentItem;

			if(item.HasProperty("Can Consume") && Time.time - m_LastConsumeTime > 3f)
			{
				// TODO: Have a better way of doing this.
				if(item.HasProperty("Thirst Change") && (Player.Thirst.Get() < 100f || Player.Thirst.Get() > 0f))
					Player.Thirst.Set(Player.Thirst.Get() + item.GetPropertyValue("Thirst Change").RandomInt.RandomValue);

				if(item.HasProperty("Hunger Change") && (Player.Hunger.Get() < 100f || Player.Hunger.Get() > 0f))
					Player.Hunger.Set(Player.Hunger.Get() + item.GetPropertyValue("Hunger Change").RandomInt.RandomValue);

				if(item.HasProperty("Health Change"))
				{
					Player.ChangeHealth.Try(new HealthEventData(item.GetPropertyValue("Health Change").RandomInt.RandomValue));
				}

				if(item.HasProperty("Consume Sound"))
					GameController.Audio.Play2D(item.GetPropertyValue("Consume Sound").Sound, m_ConsumeVolume);

				if(item.CurrentInStack == 1)
				{
					InspectedSlot.ItemHolder.SetItem(null);
					EventSystem.current.SetSelectedGameObject(null);
				}
				else
					item.CurrentInStack --;

				m_LastConsumeTime = Time.time;
			}
		}

		private IEnumerator C_WaitAndSelect()
		{
			yield return null;

			var currentSelected = EventSystem.current.currentSelectedGameObject;
			Slot selectedSlot = null;
			if(currentSelected)
				selectedSlot = currentSelected.GetComponent<Slot>();
			
			if(!currentSelected || !selectedSlot || !selectedSlot.HasItem)
			{
				if(m_Window)
					m_Window.Close();
		
				InspectedSlot = null;
			}
		}
	}
}
