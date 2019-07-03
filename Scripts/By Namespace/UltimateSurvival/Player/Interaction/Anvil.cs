using System.Collections;
using UnityEngine;
using UltimateSurvival.GUISystem;

namespace UltimateSurvival
{
	

	/// <summary>
	/// 
	/// </summary>
	public class Anvil : InteractableObject, IInventoryTrigger
	{
		public class ItemToRepair
		{
			public Recipe Recipe { get; set; }
			public ItemProperty.Value DurabilityProperty { get; set; }
		}

		public struct RequiredItem 
		{
			public string Name { get; private set; }
			public int Needs { get; private set; }
			public int Has { get; private set; }


			public RequiredItem(string name, int needs, int has)
			{
				Name = name;
				Needs = needs;
				Has = has;
			}

			public bool HasEnough()
			{
				return Has >= Needs;
			}
		}

		const float UPDATE_INTERVAL = 0.1f;

		public Activity Repairing = new Activity();

		public Value<float> RepairProgress = new Value<float>(0f);

		public Value<bool> InputItemReadyForRepair = new Value<bool>(false);

		public Message RepairFinished = new Message();

		public float RequirementRatio { get { return m_RequirementRatio; } }

		public ItemHolder InputHolder { get; private set; }

		public ItemHolder ResultHolder { get; private set; }

		public ItemToRepair InputItem { get; private set; }

		public RequiredItem[] RequiredItems { get; private set; }

		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("1 - the requirements will be the same as if crafting the item (when durability is low).\n" +
			"0 - the requirements will be very low.")]
		private float m_RequirementRatio = 0.6f;

		[Header("Audio")]

		[SerializeField]
		private AudioSource m_AudioSource;

		[SerializeField]
		private SoundPlayer m_RepairAudio;

		private WaitForSeconds m_UpdateInterval;

		private ItemContainer m_Inventory;


		public override void OnInteract(PlayerEventHandler player)
		{
			InventoryController.Instance.OpenAnvil.Try(this);
		}

		private void Start()
		{
			InputItem = new ItemToRepair();
			InputHolder = new ItemHolder();
			ResultHolder = new ItemHolder();

			InputHolder.Updated.AddListener(On_InputHolderUpdated);

			// TODO: We shouldn't have access to GUI functions here.
			m_Inventory = GUIController.Instance.GetContainer("Inventory");
			m_Inventory.Slot_Refreshed.AddListener(On_InventorySlotRefreshed);

			Repairing.AddStartTryer(TryStart_Repairing);
			Repairing.AddStopListener(OnStop_Repairing);
			RepairFinished.AddListener(On_RepairFinished);

			m_UpdateInterval = new WaitForSeconds(UPDATE_INTERVAL);
		}

		private void On_InventorySlotRefreshed(Slot slot)
		{
			if(InputItemReadyForRepair.Is(true))
			{
				CalculateRequiredItems(InputItem.Recipe, InputItem.DurabilityProperty.Float.Ratio);
				InputItemReadyForRepair.SetAndForceUpdate(true);
			}

			if(Repairing.Active && InputItemReadyForRepair.Get() && !HasRequiredItems())
				Repairing.ForceStop();
		}

		private bool TryStart_Repairing()
		{
			if(InputItemReadyForRepair.Is(false) || !HasRequiredItems())
				return false;

			StartCoroutine(C_Repair());
			return true;
		}

		private void OnStop_Repairing()
		{
			StopAllCoroutines();
			RepairProgress.Set(0f);
		}

		private void On_RepairFinished()
		{
			var item = InputHolder.CurrentItem;
			InputHolder.SetItem(null);
			ResultHolder.SetItem(item);

			foreach(var reqItem in RequiredItems)
				m_Inventory.RemoveItems(reqItem.Name, reqItem.Needs);
		}

		private void On_InputHolderUpdated(ItemHolder holder)
		{
			ItemProperty.Value durabilityProperty = null;

			if(holder.HasItem && holder.CurrentItem.HasProperty("Durability"))
				durabilityProperty = holder.CurrentItem.GetPropertyValue("Durability");
			
			if(durabilityProperty != null && holder.CurrentItem.ItemData.IsCraftable && durabilityProperty.Float.Ratio != 1f)
			{
				if(holder.CurrentItem.ItemData.IsCraftable)
				{
					InputItem.Recipe = holder.CurrentItem.ItemData.Recipe;
					InputItem.DurabilityProperty = durabilityProperty;
					CalculateRequiredItems(InputItem.Recipe, InputItem.DurabilityProperty.Float.Ratio);

					InputItemReadyForRepair.SetAndForceUpdate(true);
				
					return;
				}
			}

			// If we're here it means the item in the input slot cannot be repaired, or it doesn't exist.
			//InputItem.DurabilityProperty = null;
			//InputItem.Recipe = null;
		
			InputItemReadyForRepair.SetAndForceUpdate(false);

			if(Repairing.Active)
				Repairing.ForceStop();
		}

		private void CalculateRequiredItems(Recipe recipe, float durabilityRatio)
		{
			RequiredItems = new RequiredItem[recipe.RequiredItems.Length];

			for(int i = 0;i < recipe.RequiredItems.Length;i ++)
			{
				int has = m_Inventory.GetItemCount(recipe.RequiredItems[i].Name);
				int needs = Mathf.RoundToInt(recipe.RequiredItems[i].Amount * (1f - durabilityRatio) * m_RequirementRatio) + 1;
				RequiredItems[i] = new RequiredItem(recipe.RequiredItems[i].Name, needs, has); 
			}
		}

		private bool HasRequiredItems()
		{
			// Check if the inventory as all the required items.
			for(int i = 0;i < RequiredItems.Length;i ++)
			{
				if(!RequiredItems[i].HasEnough())
				{
					if(Repairing.Active)
						Repairing.ForceStop();

					return false;
				}
			}

			return true;
		}

		private IEnumerator C_Repair()
		{
			float requiredTime = InputItem.Recipe.Duration * (1f - InputItem.DurabilityProperty.Float.Ratio);
			float elapsedTime = 0f;

			//print("required time: " + requiredTime);

			while(elapsedTime < requiredTime)
			{
				yield return m_UpdateInterval;

				elapsedTime += UPDATE_INTERVAL;
				RepairProgress.Set(elapsedTime / requiredTime);
			}

			RepairProgress.Set(0f);

			var durability = InputItem.DurabilityProperty.Float;
			durability.Current = durability.Default;

			RepairFinished.Send();

			InputItem.DurabilityProperty.SetValue(ItemProperty.Type.Float, durability);
		}
	}
}
