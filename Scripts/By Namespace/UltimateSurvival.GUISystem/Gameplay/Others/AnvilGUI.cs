using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateSurvival.GUISystem
{
	/// <summary>
	/// 
	/// </summary>
	public class AnvilGUI : GUIBehaviour 
	{
		[SerializeField]
		private Text m_RequiredItemsText;

		[SerializeField]
		private Color m_HasEnoughColor = Color.yellow;

		[SerializeField]
		private Color m_NotEnoughColor = Color.red;

		[SerializeField]
		private Image m_ProgressBar;

		[SerializeField]
		private Button m_RepairButton;

		private ItemContainer m_InventoryContainer;
		private ItemContainer m_InputContainer;
		private ItemContainer m_ResultContainer;

		private string m_EnoughHex;
		private string m_NotEnoughHex;

		private Anvil m_CurrentAnvil;


		private void Awake()
		{
			m_InputContainer = transform.FindDeepChild("Input").GetComponent<ItemContainer>();
			m_ResultContainer = transform.FindDeepChild("Result").GetComponent<ItemContainer>();

			InventoryController.Instance.State.AddChangeListener(OnChanged_InventoryState);
			InventoryController.Instance.OpenAnvil.SetTryer(Try_OpenAnvil);

			m_RepairButton.onClick.AddListener(On_ButtonClicked);
		}

		private void Start()
		{
			m_EnoughHex = ColorUtils.ColorToHex(m_HasEnoughColor);
			m_NotEnoughHex = ColorUtils.ColorToHex(m_NotEnoughColor);

			m_InventoryContainer = Controller.GetContainer("Inventory");
			m_InventoryContainer.Slot_Refreshed.AddListener(On_InventoryRefreshed);
		}

		private void On_ButtonClicked()
		{
			if(m_CurrentAnvil)
				m_CurrentAnvil.Repairing.TryStart();
		}

		private void OnChanged_InventoryState()
		{
			if(m_CurrentAnvil && InventoryController.Instance.IsClosed)
			{
				m_CurrentAnvil.RepairProgress.RemoveChangeListener(OnChanged_RepairProgress);
				m_CurrentAnvil.InputItemReadyForRepair.RemoveChangeListener(OnChanged_InputItemIsReadyForRepair);
				m_CurrentAnvil = null;
			}
		}

		private bool Try_OpenAnvil(Anvil anvil)
		{
			if(InventoryController.Instance.IsClosed)
			{
				bool hasOpened = InventoryController.Instance.SetState.Try(ET.InventoryState.Anvil);

				if(hasOpened)
				{
					m_CurrentAnvil = anvil;
					m_InputContainer.Setup(anvil.InputHolder);
					m_ResultContainer.Setup(anvil.ResultHolder);

					m_CurrentAnvil.RepairProgress.AddChangeListener(OnChanged_RepairProgress);
					m_CurrentAnvil.InputItemReadyForRepair.AddChangeListener(OnChanged_InputItemIsReadyForRepair);

					m_ProgressBar.fillAmount = m_CurrentAnvil.RepairProgress.Get();

					return true;
				}
			}

			return false;
		}

		private void OnChanged_RepairProgress()
		{
			m_ProgressBar.fillAmount = m_CurrentAnvil.RepairProgress.Get();
		}

		private void On_InventoryRefreshed(Slot displayer)
		{
			UpdateRequiredItemsList();
		}

		private void OnChanged_InputItemIsReadyForRepair()
		{
			UpdateRequiredItemsList();
		}
			
		private void UpdateRequiredItemsList()
		{
			if(m_CurrentAnvil && m_InputContainer.Slots[0].HasItem)
			{
				if(m_CurrentAnvil.InputItemReadyForRepair.Is(false))
					m_RequiredItemsText.text = "<size=10><i>This item doesn't require repairing...</i></size>";
				// Show the required items to repair...
				else
				{
					m_RequiredItemsText.text = string.Empty;
					var itemToRepair = m_CurrentAnvil.InputItem;
					StringBuilder builder = new StringBuilder("Requires: \n");

					for(int i = 0;i < m_CurrentAnvil.RequiredItems.Length;i ++)
					{
						var requiredItem = m_CurrentAnvil.RequiredItems[i];
						string targetColor = requiredItem.HasEnough() ? m_EnoughHex : m_NotEnoughHex;

						builder.AppendFormat("<color={0}>{1} x {2}</color> \n", targetColor, itemToRepair.Recipe.RequiredItems[i].Name, requiredItem.Needs);
					}

					m_RequiredItemsText.text = builder.ToString();
				}
			}
			else
				m_RequiredItemsText.text = "<size=10><i>Place an item here...</i></size>";
		}
	}
}
