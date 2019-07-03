using System;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival
{
	[Serializable]
	public class ItemHolder
	{
		/// <summary>Sent when this holder is updated (e.g. when the item has changed, or it was deleted).</summary>
		public Message<ItemHolder> Updated = new Message<ItemHolder>();

		public bool HasItem { get { return CurrentItem != null; } }

		public SavableItem CurrentItem { get; private set; }


		public static implicit operator bool(ItemHolder holder)
		{
			return holder != null;
		}

		public bool TryAddItem(ItemData itemData, int amount, out int added, List<ItemProperty.Value> customPropertyValues = null)
		{
			added = 0;

			if(HasItem && itemData.Id != CurrentItem.Id)
				return false;

			if(!HasItem)
			{
				CurrentItem = new SavableItem(itemData, 1, customPropertyValues);
				CurrentItem.CurrentInStack = 0;
				CurrentItem.PropertyChanged.AddListener(On_PropertyChanged);
				CurrentItem.StackChanged.AddListener(On_StackChanged);
			}

			int oldValue = CurrentItem.CurrentInStack;
			int surplus = amount + oldValue - itemData.StackSize;
			int currentInStack = oldValue;

			if(surplus <= 0)
				currentInStack += amount;
			else
				currentInStack = itemData.StackSize;

			CurrentItem.CurrentInStack = currentInStack;
			added = currentInStack - oldValue;

			Updated.Send(this);

			return added > 0;
		}

		/// <summary>
		/// 
		/// </summary>
		public void SetItem(SavableItem item)
		{
			if(CurrentItem)
			{
				CurrentItem.PropertyChanged.RemoveListener(On_PropertyChanged);
				CurrentItem.StackChanged.RemoveListener(On_StackChanged);
			}

			CurrentItem = item;

			if(CurrentItem)
			{
				CurrentItem.PropertyChanged.AddListener(On_PropertyChanged);
				CurrentItem.StackChanged.AddListener(On_StackChanged);
			}

			Updated.Send(this);
		}

		/// <summary>
		/// 
		/// </summary>
		public void RemoveFromStack(int amount, out int removed)
		{
			removed = 0;

			if(!HasItem)
				return;

			if(amount >= CurrentItem.CurrentInStack)
			{
				removed = CurrentItem.CurrentInStack;
				SetItem(null);

				return;
			}

			int oldStack = CurrentItem.CurrentInStack;
			CurrentItem.CurrentInStack = Mathf.Max(CurrentItem.CurrentInStack - amount, 0);
			removed = oldStack - CurrentItem.CurrentInStack;

			Updated.Send(this);
		}

		/// <summary>
		/// 
		/// </summary>
		public void RemoveFromStack(int amount)
		{
			if(!HasItem)
				return;

			int oldStack = CurrentItem.CurrentInStack;
			CurrentItem.CurrentInStack = Mathf.Max(CurrentItem.CurrentInStack - amount, 0);

			Updated.Send(this);
		}

		private void On_PropertyChanged(ItemProperty.Value propertyValue)
		{
			Updated.Send(this);
		}

		private void On_StackChanged()
		{
			Updated.Send(this);
		}
	}
}
