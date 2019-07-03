using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateSurvival.GUISystem
{
	/// <summary>
	/// 
	/// </summary>
	public class SmeltingStationGUI : MonoBehaviour 
	{
		public Value<bool> IsBurning = new Value<bool>(false);

		[SerializeField]
		private SmeltingStationType m_Type;

		[SerializeField]
		private ItemContainer m_LootContainer;

		[SerializeField]
		private Image m_ProgressBar;

		private SmeltingStation m_CurrentStation;

		private ItemContainer m_Input;
		private ItemContainer m_Fuel;


		private void Awake()
		{
			m_Input = transform.FindDeepChild("Input").GetComponent<ItemContainer>();
			m_Fuel = transform.FindDeepChild("Fuel").GetComponent<ItemContainer>();
		}

		private void Start()
		{
			if(m_Type == SmeltingStationType.Furnace)
				InventoryController.Instance.OpenFurnace.SetTryer(Try_OpenSmeltingStation);
			else if(m_Type == SmeltingStationType.Campfire)
				InventoryController.Instance.OpenCampfire.SetTryer(Try_OpenSmeltingStation);

			InventoryController.Instance.State.AddChangeListener(OnChanged_InventoryState);
		}

		private bool Try_OpenSmeltingStation(SmeltingStation station)
		{
			if(InventoryController.Instance.IsClosed)
			{
				bool hasOpened = false;

				if(m_Type == SmeltingStationType.Furnace)
					hasOpened = InventoryController.Instance.SetState.Try(ET.InventoryState.Furnace);
				else if(m_Type == SmeltingStationType.Campfire)
					hasOpened = InventoryController.Instance.SetState.Try(ET.InventoryState.Campfire);

				if(hasOpened)
				{
					m_CurrentStation = station;
					station.IsBurning.AddChangeListener(OnChanged_IsBurning);

					station.Progress.AddChangeListener(OnChanged_Progress);
					m_ProgressBar.fillAmount = m_CurrentStation.Progress.Get();

					m_Input.Setup(station.InputSlot);
					m_Fuel.Setup(station.FuelSlot);
					m_LootContainer.Setup(station.LootSlots);

					IsBurning.Set(m_CurrentStation.IsBurning.Get());

					return true;
				}
			}

			return false;
		}

		private void OnChanged_Progress()
		{
			m_ProgressBar.fillAmount = m_CurrentStation.Progress.Get();
		}

		private void OnChanged_InventoryState()
		{
			if(m_CurrentStation && InventoryController.Instance.IsClosed)
			{
				m_CurrentStation.IsBurning.RemoveChangeListener(OnChanged_IsBurning);
				m_CurrentStation.Progress.RemoveChangeListener(OnChanged_Progress);
				m_CurrentStation = null;
			}
		}

		private void OnChanged_IsBurning()
		{
			IsBurning.Set(m_CurrentStation.IsBurning.Get());
		}
	}
}
