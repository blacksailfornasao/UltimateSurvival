using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UltimateSurvival.GUISystem
{
	/// <summary>
	/// 
	/// </summary>
	public class CharacterEquipmentHandler : MonoBehaviour
	{
		[SerializeField] 
		private AudioSource	m_AudioSource;

		[SerializeField] 
		private SoundPlayer m_EquipAudio;


		private void Start()
		{
			foreach(var slot in GetComponentsInChildren<Slot>())
				slot.ItemHolder.Updated.AddListener(On_ItemHolder_Updated);
		}

		private void On_ItemHolder_Updated(ItemHolder holder)
		{
			InventoryController.Instance.EquipmentChanged.Send(holder);

			if(holder.HasItem)
				m_EquipAudio.Play(ItemSelectionMethod.RandomlyButExcludeLast, m_AudioSource, 1f);
		}
	}
}
