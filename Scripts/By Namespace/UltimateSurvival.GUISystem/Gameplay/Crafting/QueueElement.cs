using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateSurvival.GUISystem
{
	/// <summary>
	/// 
	/// </summary>
	public class CraftData
	{
		public ItemData Result;
		public int Amount;
	}

	/// <summary>
	/// 
	/// </summary>
	public class QueueElement : MonoBehaviour 
	{
		public Message<QueueElement> Cancel = new Message<QueueElement>();
		public Message Complete = new Message();

		[SerializeField] 
		private Image m_Icon;

		[SerializeField] 
		private Text m_RemainedTime;

		[SerializeField] 
		private Image m_ProgressBar;

		private CraftData m_CraftData;
		private ItemData m_Result;
		private ItemContainer	m_Inventory;
		private int m_AmountToCraftRemained;
		private bool m_Initialized;


		/// <summary>
		/// Can be initialized only once.
		/// </summary>
		public void Initialize(CraftData craftData, ItemContainer inventory)
		{
			if(m_Initialized)
				return;

			foreach(var recipe in craftData.Result.Recipe.RequiredItems)
				inventory.RemoveItems(recipe.Name, recipe.Amount * craftData.Amount);

			// Add the icon.
			m_Result = craftData.Result;
			m_Icon.sprite = m_Result.Icon;

			m_RemainedTime.text = string.Format("{0}s (x{1})", (int)craftData.Result.Recipe.Duration, craftData.Amount);
			m_ProgressBar.fillAmount = 0f;
			m_AmountToCraftRemained = craftData.Amount;

			m_CraftData = craftData;
			m_Inventory = inventory;

			m_Initialized = true;
		}

		/// <summary>
		/// Will not start if it's not initialized first (That's done through Initialize()).
		/// </summary>
		public void StartCrafting()
		{
			if(!m_Initialized)
				return;

			StartCoroutine(C_Update(m_CraftData, m_Inventory));
		}

		public void CancelCrafting()
		{
			if(m_AmountToCraftRemained > 0)
			{
				foreach(var item in m_CraftData.Result.Recipe.RequiredItems)
				{
					ItemData itemData;
					InventoryController.Instance.Database.FindItemByName(item.Name, out itemData);
					m_Inventory.TryAddItem(itemData, item.Amount * m_AmountToCraftRemained);
				}
			}

			Cancel.Send(this);
			Destroy(gameObject);
		}

		private IEnumerator C_Update(CraftData craftData, ItemContainer inventory)
		{
			//float waitTimeInSeconds = 1f;
			//var waitTime = new WaitForSeconds(waitTimeInSeconds);

			while(m_AmountToCraftRemained > 0)
			{
				float remainedTime = craftData.Result.Recipe.Duration;

				while(remainedTime > 0)
				{
					yield return null;

					remainedTime = Mathf.Clamp(remainedTime - Time.deltaTime, 0, Mathf.Infinity);
					m_RemainedTime.text = string.Format("{0}s (x{1})", Mathf.Ceil(remainedTime), m_AmountToCraftRemained);
					m_ProgressBar.fillAmount = 1f - remainedTime / craftData.Result.Recipe.Duration;
				}

				// The item was successfully crafted.
				inventory.TryAddItem(m_Result, 1);
				m_AmountToCraftRemained --;

				m_ProgressBar.fillAmount = 0f;
				m_RemainedTime.text = string.Format("{0}s (x{1})", 0, m_AmountToCraftRemained);
			}
				
			Complete.Send();
			Destroy(gameObject);
		}
	}
}
