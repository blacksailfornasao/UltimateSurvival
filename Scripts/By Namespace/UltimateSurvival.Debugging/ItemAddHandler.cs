using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UltimateSurvival.GUISystem;

namespace UltimateSurvival.Debugging
{
	public class ItemAddHandler : MonoBehaviour 
	{
		[SerializeField] 
		private Dropdown m_ItemDropdown;

		[SerializeField] 
		private InputField m_AmountInput;

		[SerializeField] 
		private Button m_AddButton;


		private void Start()
		{
			m_AddButton.onClick.AddListener(TryAddItem);
			CreateItemOptions();
		}

		private void CreateItemOptions()
		{
			var allItemsData = new List<Dropdown.OptionData>();
			var database = InventoryController.Instance.Database;
			var allNames = database.GetAllItemNames();

			foreach(string itemName in allNames)
				allItemsData.Add(new Dropdown.OptionData(itemName));

			m_ItemDropdown.options = allItemsData;
			m_ItemDropdown.RefreshShownValue();
		}

		private void TryAddItem()
		{
			int amount;
			int added;
			if(int.TryParse(m_AmountInput.text, out amount))
				InventoryController.Instance.AddItemToCollection(m_ItemDropdown.value, amount, "Inventory", out added);
		}
	}
}
