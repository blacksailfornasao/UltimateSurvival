using UnityEngine;
using UnityEngine.UI;

namespace UltimateSurvival.GUISystem
{
	/// <summary>
	/// 
	/// </summary>
	public class LootContainerOpener : MonoBehaviour 
	{
		[SerializeField]
		private ItemContainer m_ItemContainer;
		
		private LootObject m_CurLootObject;


		private void Start()
		{
			InventoryController.Instance.OpenLootContainer.SetTryer(Try_OpenLootContainer);
		}

		private bool Try_OpenLootContainer(LootObject lootObject)
		{
			if(InventoryController.Instance.IsClosed)
			{
				bool hasOpened = InventoryController.Instance.SetState.Try(ET.InventoryState.Loot);

				if(hasOpened)
				{
					m_CurLootObject = lootObject;
					m_ItemContainer.Setup(lootObject.ItemHolders);

					return true;
				}
			}

			return false;
		}

		private void OnChanged_InventoryState()
		{
			if(m_CurLootObject && InventoryController.Instance.IsClosed)
				m_CurLootObject = null;
		}
	}
}
