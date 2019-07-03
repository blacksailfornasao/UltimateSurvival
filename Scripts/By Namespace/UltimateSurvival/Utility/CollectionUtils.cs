using System.Collections.Generic;
using UnityEngine;
using UltimateSurvival.GUISystem;

namespace UltimateSurvival
{
	/// <summary>
	/// Useful for adding items & sorting a collection.
	/// </summary>
	public static class CollectionUtils
	{
		/// <summary>
		/// Tries to add an amount of items in a specific collection.
		/// </summary>
		public static void AddItem(ItemData itemData, int amount, List<ItemHolder> itemHolders, out int added, List<ItemProperty.Value> customPropertyValues = null)
		{
			added = 0;

			// Go through each slot and see where we can add the item(s).
			for(int i = 0;i < itemHolders.Count;i ++)
			{
				int addedToSlot;
				itemHolders[i].TryAddItem(itemData, amount, out addedToSlot, customPropertyValues);
				added += addedToSlot;

				// We added all the items, we can stop now.
				if(added == amount)
					return;
			}
		}

		/// <summary>
		/// Tries to add an amount of items in a specific collection.
		/// </summary>
		public static void AddItem(ItemData itemData, int amount, List<ItemHolder> itemHolders, List<ItemProperty.Value> customPropertyValues = null)
		{
			int added = 0;

			// Go through each slot and see where we can add the item(s).
			for(int i = 0;i < itemHolders.Count;i ++)
			{
				int addedNow;

				itemHolders[i].TryAddItem(itemData, amount, out addedNow, customPropertyValues);
				added += addedNow;
				amount -= addedNow;

				// We added all the items, we can stop now.
				if(amount == 0)
					return;
			}
		}

		/// <summary>
		/// Tries to add an amount of items in a specific collection.
		/// </summary>
		public static void AddItem(ItemData itemData, int amount, List<Slot> slots, out int added, List<ItemProperty.Value> customPropertyValues = null)
		{
			added = 0;

			// Go through each slot and see where we can add the item(s).
			for(int i = 0;i < slots.Count;i ++)
			{
				int addedNow;

				slots[i].ItemHolder.TryAddItem(itemData, amount, out addedNow, customPropertyValues);
				added += addedNow;
				amount -= addedNow;

				// We added all the items, we can stop now.
				if(amount == 0)
					return;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public static void RemoveItems(string itemName, int amount, List<ItemHolder> holders, out int removed)
		{
			removed = 0;
	
			for(int i = 0;i < holders.Count;i ++)
			{
				if(holders[i].HasItem && holders[i].CurrentItem.Name == itemName)
				{
					int removedFromHolder;
					holders[i].RemoveFromStack(amount - removed, out removedFromHolder);
					removed += removedFromHolder;

					// We added all the items, we can stop now.
					if(removed == amount)
						return;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public static void RemoveItems(string itemName, int amount, List<Slot> slots, out int removed)
		{
			removed = 0;

			for(int i = 0;i < slots.Count;i ++)
			{
				if(slots[i].HasItem && slots[i].CurrentItem.Name == itemName)
				{
					int removedFromSlot;
					slots[i].ItemHolder.RemoveFromStack(amount - removed, out removedFromSlot);
					removed += removedFromSlot;

					// We added all the items, we can stop now.
					if(removed == amount)
						return;
				}
			}
		}
	}
}
