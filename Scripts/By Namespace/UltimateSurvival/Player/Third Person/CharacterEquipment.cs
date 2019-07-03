using System;
using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class CharacterEquipment : MonoBehaviour 
	{
		/// <summary>
		/// 
		/// </summary>
		[Serializable]
		public class EquipmentPiece
		{
			public ItemHolder CorrespondingHolder { get; set; }

			public string ItemName { get { return m_ItemName; } }
			public GameObject Object { get { return m_Object; } }

			[SerializeField]
			private string m_ItemName;

			[SerializeField]
			private GameObject m_Object;
		}

		[SerializeField] 
		private EquipmentPiece[] m_EquipmentPieces;

		[SerializeField] 
		private GameObject m_Underwear;


		private void Start()
		{
			InventoryController.Instance.EquipmentChanged.AddListener(On_EquipmentChanged);
		}

		private void On_EquipmentChanged(ItemHolder holder)
		{
			foreach(var piece in m_EquipmentPieces)
			{
				if(holder.HasItem && piece.ItemName == holder.CurrentItem.ItemData.Name)
				{
					piece.CorrespondingHolder = holder.HasItem ? holder : null;
					piece.Object.SetActive(holder.HasItem);

					if(holder.HasItem && m_Underwear && holder.CurrentItem.HasProperty("Disable Underwear"))
						m_Underwear.SetActive(false);
				}
				else if(piece.CorrespondingHolder == holder)
				{
					piece.CorrespondingHolder = null;
					piece.Object.SetActive(false);
				}
			}

			HandleUnderwear(holder);
		}

		private void HandleUnderwear(ItemHolder holder)
		{
			if(!m_Underwear || m_Underwear.activeSelf)
				return;

			bool showUnderwear = true;

			foreach(var piece in m_EquipmentPieces)
			{
				if(piece.CorrespondingHolder && piece.CorrespondingHolder.HasItem && piece.CorrespondingHolder.CurrentItem.HasProperty("Disable Underwear"))
					showUnderwear = false;
			}

			if(showUnderwear)
				m_Underwear.SetActive(true);
		}
	}
}
