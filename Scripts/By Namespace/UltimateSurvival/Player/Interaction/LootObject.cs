using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// Data to generate items as part of this collection.
	/// </summary>
	[Serializable]
	public class ItemToGenerate
	{
		[SerializeField]
		private bool m_Random;

		[SerializeField]
		private string m_CustomName;

		[SerializeField]
		[Clamp(1, 9999999)]
		private int m_StackSize = 1;


		public bool TryGenerate(out SavableItem runtimeItem)
		{
			runtimeItem = null;
			var database = InventoryController.Instance.Database;
			ItemData itemData;

			if(m_Random)
			{
				if(database.FindItemById(UnityEngine.Random.Range(0, database.GetItemCount() - 1), out itemData))
				{
					runtimeItem = new SavableItem(itemData, (int)(itemData.StackSize * 0.1f) + 1);
					return true;
				}
			}
			else if(database.FindItemByName(m_CustomName, out itemData))
			{
				runtimeItem = new SavableItem(itemData, m_StackSize);
				return true;
			}
				
			return false;
		}

		public ItemData GenerateItemData()
		{
			var database = InventoryController.Instance.Database;
			ItemData itemData = null;

			if(m_Random)
				database.FindItemById(UnityEngine.Random.Range(0, database.GetItemCount() - 1), out itemData);
			else 
				database.FindItemByName(m_CustomName, out itemData);

			return itemData;
		}
	}

	public class LootObject : InteractableObject, IInventoryTrigger
	{
		public List<ItemHolder> ItemHolders { get; private set; }

		[SerializeField]
		[Range(1, 50)]
		protected int m_Capacity = 8;

		[SerializeField]
		protected ItemToGenerate[] m_InitialItems;

		[Header("Box Opening")]

		[SerializeField]
		private Transform m_Cover;

		[SerializeField]
		private float m_OpenSpeed = 6f;

		[SerializeField]
		private float m_ClosedRotation;

		[SerializeField]
		private float m_OpenRotation = 60f;

		private float m_CurrentRotation;


		public override void OnInteract(PlayerEventHandler player)
		{
			if(enabled && InventoryController.Instance.OpenLootContainer.Try(this))
			{
				On_InventoryOpened();
				InventoryController.Instance.State.AddChangeListener(OnChanged_InventoryController_State);
			}
		}
			
		private void Start()
		{
			ItemHolders = new List<ItemHolder>();

			for(int i = 0;i < m_Capacity;i ++)
			{
				// Generate the holders.
				ItemHolders.Add(new ItemHolder());

				// Add the initial items.
				SavableItem runtimeItem;
				if(i < m_InitialItems.Length && m_InitialItems[i].TryGenerate(out runtimeItem))
					ItemHolders[i].SetItem(runtimeItem);
			}
		}

		private void OnChanged_InventoryController_State()
		{
			if(InventoryController.Instance.IsClosed)
				On_InventoryClosed();
		}

		private void On_InventoryOpened()
		{
			if(m_Cover != null)
			{
				StopAllCoroutines();
				StartCoroutine(C_OpenCover(true));
			}
		}

		private void On_InventoryClosed()
		{
			if(m_Cover != null)
			{
				StopAllCoroutines();
				StartCoroutine(C_OpenCover(false));
			}
		}
			
		private IEnumerator C_OpenCover(bool open)
		{
			float targetRotation = open ? m_OpenRotation : m_ClosedRotation;
			
			while(Mathf.Abs(targetRotation - m_CurrentRotation) > 0.1f)
			{
				m_CurrentRotation = Mathf.Lerp(m_CurrentRotation, targetRotation, Time.deltaTime * m_OpenSpeed);
				m_Cover.transform.localRotation = Quaternion.Euler(m_CurrentRotation, 0f, 0f);
				yield return null;
			}
		}
	}
}
