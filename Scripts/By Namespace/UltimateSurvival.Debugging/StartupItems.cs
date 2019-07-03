using System;
using UnityEngine;
using UltimateSurvival.GUISystem;

namespace UltimateSurvival.Debugging
{
	public class StartupItems : MonoBehaviour 
	{
		[SerializeField]
		[Reorderable]
		private ReorderableItemToAddList m_InventoryItems;

		[SerializeField]
		[Reorderable]
		private ReorderableItemToAddList m_HotbarItems;


		private void Start()
		{
			foreach(ItemToAdd item in m_InventoryItems)
			{
				int added;
				InventoryController.Instance.AddItemToCollection(item.Name, item.Count, "Inventory", out added);
			}
				
			foreach(ItemToAdd item in m_HotbarItems)
			{
				int added;
				InventoryController.Instance.AddItemToCollection(item.Name, item.Count, "Hotbar", out added);
			}
		}

		[Serializable]
		public class ItemToAdd
		{
			public string Name { get { return m_Name; } }
			public int Count { get { return m_Count; } }

			[SerializeField]
			private string m_Name;

			[SerializeField]
			[Clamp(1, 9999)]
			private int m_Count = 1;
		}
	}
}
