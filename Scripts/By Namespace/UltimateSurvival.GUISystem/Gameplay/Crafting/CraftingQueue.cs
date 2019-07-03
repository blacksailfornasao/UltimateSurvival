using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateSurvival.GUISystem
{
	/// <summary>
	/// 
	/// </summary>
	public class CraftingQueue : MonoBehaviour
	{
		[SerializeField] 
		private ItemContainer m_Inventory;

		[SerializeField] 
		private QueueElement m_QueueElementTemplate;

		[SerializeField] 
		private Transform m_QueueParent;

		[SerializeField]
		private int m_MaxElements = 8;

		private List<QueueElement> m_Queue = new List<QueueElement>();
		private QueueElement m_ActiveElement;


		private void Start()
		{
			if(!m_Inventory)
			{
				Debug.LogError("The inventory is not assigned as a reference in the inspector!", this);
				return;
			}

			m_QueueElementTemplate.gameObject.SetActive(false);
			InventoryController.Instance.CraftItem.SetTryer(Try_CraftItem);
		}

		private bool Try_CraftItem(CraftData craftData)
		{
			int elementCount = GetComponentsInChildren<QueueElement>().Length;

			if(elementCount < m_MaxElements)
			{
				var newElement = Instantiate<QueueElement>(m_QueueElementTemplate);
				newElement.gameObject.SetActive(true);
				newElement.transform.SetParent(m_QueueParent);
				newElement.transform.SetAsFirstSibling();
				newElement.transform.localPosition = Vector3.zero;
				newElement.transform.localScale = Vector3.one;

				newElement.Initialize(craftData, m_Inventory);
				newElement.Cancel.AddListener(On_CraftingCanceled);

				if(elementCount == 0)
				{
					newElement.StartCrafting();
					newElement.Complete.AddListener(StartNext);
					m_ActiveElement = newElement;
				}
				else
					m_Queue.Insert(0, newElement);
				
				return true;
			}

			return false;
		}

		private void StartNext()
		{
			if(m_Queue.Count > 0)
			{
				var next = m_Queue[m_Queue.Count - 1];
				m_Queue.Remove(next);

				next.StartCrafting();
				next.Complete.AddListener(StartNext);
				m_ActiveElement = next;
			}
		}

		private void On_CraftingCanceled(QueueElement queueElement)
		{
			if(m_Queue.Contains(queueElement))
				m_Queue.Remove(queueElement);

			if(queueElement == m_ActiveElement)
				StartNext();
		}
	}
}
