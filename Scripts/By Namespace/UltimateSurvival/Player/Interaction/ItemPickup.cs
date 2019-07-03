using UnityEngine;
using UltimateSurvival.GUISystem;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class ItemPickup : InteractableObject
	{
		public enum PickupMethod
		{
			WalkOver,
			OnInteract
		}

		/// <summary> </summary>
		public SavableItem ItemToAdd { get; set; }

		[SerializeField]
		private PickupMethod m_PickupMethod;

		[SerializeField]
		private string m_DefaultItem;

		[SerializeField]
		private int m_DefaultAmount = 1;

		[SerializeField]
		private AudioClip m_OnDestroySound;

		[SerializeField]
		private float m_OnDestroyVolume = 0.5f;


		public override void OnInteract(PlayerEventHandler player)
		{
			if(m_PickupMethod == PickupMethod.WalkOver || !ItemToAdd)
				return;

			// TODO: We shouldn't have access to GUI functions here.
			var inventory = GUIController.Instance.GetContainer("Inventory");
			bool added = inventory.TryAddItem(ItemToAdd);
		
			if(added)
			{
				if(m_OnDestroySound)
					GameController.Audio.Play2D(m_OnDestroySound, m_OnDestroyVolume);

				MessageDisplayer.Instance.PushMessage(string.Format("Picked up <color=yellow>{0}</color> x {1}", ItemToAdd.Name, ItemToAdd.CurrentInStack));
			}

			Destroy(gameObject);
		}

		private void Awake()
		{
			var database = InventoryController.Instance.Database;

			ItemData itemData;
			if(database && database.FindItemByName(m_DefaultItem, out itemData))
				ItemToAdd = new SavableItem(itemData, m_DefaultAmount);
		}
	}
}
