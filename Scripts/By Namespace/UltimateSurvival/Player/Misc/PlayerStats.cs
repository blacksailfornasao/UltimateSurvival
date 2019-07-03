using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival
{
	public class PlayerStats : PlayerBehaviour 
	{
		private List<ItemHolder> m_EquipmentHolders;


		private void Start()
		{
			InventoryController.Instance.EquipmentChanged.AddListener(On_EquipmentChanged);
			m_EquipmentHolders = InventoryController.Instance.GetEquipmentHolders();
		}

		private void On_EquipmentChanged(ItemHolder holder)
		{
			int defense = 0;
			
			// Calculate the total defense now.
			foreach(var h in m_EquipmentHolders)
			{
				if(h.HasItem && h.CurrentItem.HasProperty("Defense"))
					defense += h.CurrentItem.GetPropertyValue("Defense").Int.Current;
			}

			Player.Defense.Set(defense);
		}
	}
}
