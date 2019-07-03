using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UltimateSurvival.GUISystem
{
	/// <summary>
	/// 
	/// </summary>
	public class RecipeInspector : MonoBehaviour
	{
		public RecipeSlot InspectedSlot { get; private set; }

		[Header("Setup")]

		[SerializeField]
		private ItemContainer m_Inventory;

		[SerializeField] 
		private CraftingList m_CraftingList;

		[SerializeField] 
		private Window m_Window;

		[SerializeField] 
		private int m_MaxCraftAmount = 999;

		[Header("GUI Elements")]

		[SerializeField] 
		private Text m_ItemName;

		[SerializeField] 
		private Text m_Description;

		[SerializeField] 
		private Image m_Icon;

		[SerializeField]
		private RequiredItemRow[] m_RequiredItemsTable;

		[SerializeField] 
		private Text m_TotalTime;

		[SerializeField] 
		private Text m_DesiredAmount;

		private ItemData m_InspectedItem;
		private int m_CurrentDesiredAmount;


		/// <summary>
		/// 
		/// </summary>
		public void Try_StartCrafting()
		{
			bool canStartCrafting = m_CurrentDesiredAmount > 0;

			// Determine if we have enough amounts of "ingredients" to start crafting. 
			foreach(var requiredItem in m_InspectedItem.Recipe.RequiredItems)
			{
				if(requiredItem.Amount * m_CurrentDesiredAmount > m_Inventory.GetItemCount(requiredItem.Name))
				{
					canStartCrafting = false;
					break;
				}
			}

			if(canStartCrafting)
			{
				CraftData craftData = new CraftData() { Result = m_InspectedItem, Amount = m_CurrentDesiredAmount };
				bool wasAddedToQueue = InventoryController.Instance.CraftItem.Try(craftData);

				if(wasAddedToQueue)
				{
					m_CurrentDesiredAmount = 1;
					ShowRecipeInfo(m_InspectedItem);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void TryIncreaseAmount()
		{
			if(m_InspectedItem == null)
				return;
			
			m_CurrentDesiredAmount ++;
			ShowRecipeInfo(m_InspectedItem);
		}

		/// <summary>
		/// 
		/// </summary>
		public void TryDecreaseAmount()
		{
			if(m_InspectedItem == null)
				return;

			m_CurrentDesiredAmount = Mathf.Clamp(m_CurrentDesiredAmount - 1, 1, m_MaxCraftAmount);
			ShowRecipeInfo(m_InspectedItem);
		}

		private void Awake()
		{
			if(!m_CraftingList)
				Debug.LogError("Please assign the Crafting List in the inspector!", this);

			m_CraftingList.RecipesGenerated.AddListener(On_RecipesGenerated);
			m_TotalTime.text = "";
		}

		/// <summary>
		/// 
		/// </summary>
		private void On_RecipesGenerated()
		{
			foreach(var holder in m_CraftingList.RecipesByCategory)
			{
				foreach(var obj in holder.ObjectList)
				{
					var slot = obj.GetComponent<RecipeSlot>();
					if(slot)
						slot.PointerUp += On_Slot_PointerUp;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void On_Slot_PointerUp(BaseEventData data, RecipeSlot slot)
		{
			bool shouldOpenUp = 
				!InventoryController.Instance.IsClosed &&
				EventSystem.current.currentSelectedGameObject == slot.gameObject;
			
			if(shouldOpenUp)
			{
				if(m_Window)
					m_Window.Open();
	
				InspectedSlot = slot;
				m_InspectedItem = slot.Result;

				m_CurrentDesiredAmount = 1;
				ShowRecipeInfo(m_InspectedItem);

				slot.E_Deselect += On_Slot_Deselect;
			}
			else
				On_Slot_Deselect(data, slot);
		}

		/// <summary>
		/// 
		/// </summary>
		private void ShowRecipeInfo(ItemData itemData)
		{
			ItemDatabase database = InventoryController.Instance.Database;

            // Name.
            m_ItemName.text = (itemData.DisplayName == string.Empty) ? itemData.Name : itemData.DisplayName;

            // Description.
            m_Description.text = itemData.Descriptions[0];

			// Icon.
			m_Icon.sprite = itemData.Icon;

			// Required items table.
			for(int i = 0;i < m_RequiredItemsTable.Length;i ++)
			{
				var requiredItemRow = m_RequiredItemsTable[i];

				if(itemData.Recipe.RequiredItems.Length > i)
				{
					var requiredItem = itemData.Recipe.RequiredItems[i];
					int total = requiredItem.Amount * Mathf.Clamp(m_CurrentDesiredAmount, 1, int.MaxValue);
					requiredItemRow.Set(requiredItem.Amount, requiredItem.Name, total, m_Inventory.GetItemCount(requiredItem.Name));
				}
				else
					requiredItemRow.Set(0, "", 0, 0);
			}

			// Total required time.
			m_TotalTime.text = string.Format("ETA: {0}s", (itemData.Recipe.Duration * m_CurrentDesiredAmount).ToString());

			// Desired amount.
			m_DesiredAmount.text = (m_CurrentDesiredAmount > 0 ? "x" : "") + m_CurrentDesiredAmount.ToString();
		}
			
		/// <summary>
		/// 
		/// </summary>
		private void On_Slot_Deselect(BaseEventData data, RecipeSlot deselectedSlot)
		{
			StartCoroutine(C_CheckNextSelection());
		}

		/// <summary>
		/// 
		/// </summary>
		private IEnumerator C_CheckNextSelection()
		{
			yield return null;

			RecipeSlot selectedSlot = null;
			var currentSelected = EventSystem.current.currentSelectedGameObject;
			if(currentSelected)
				selectedSlot = currentSelected.GetComponent<RecipeSlot>();
			
			if(!currentSelected || !selectedSlot)
			{
				if(m_Window)
					m_Window.Close();

				InspectedSlot = null;
			}
		}
	}
}
