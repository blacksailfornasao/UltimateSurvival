using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UltimateSurvival.GUISystem;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class LootItem
	{
		/// <summary></summary>
		public string ItemName { get { return m_ItemName; } }

		/// <summary>Spawn chance of this item (0 to 100)</summary>
		public float SpawnChance { get { return m_SpawnChance; } }

		[SerializeField]
		private string m_ItemName;

		[SerializeField]
		[Range(0f, 100f)]
		private float m_SpawnChance;

		[SerializeField]
		private int m_MinAmount;

		[SerializeField]
		private int m_MaxAmount;


		/// <summary>
		/// 
		/// </summary>
		public void AddToInventory(out int added, float amountFactor)
		{
			added = 0;

			int amountToAdd = Mathf.CeilToInt(Random.Range(m_MinAmount, m_MaxAmount) * amountFactor);
			if(amountToAdd > 0)
				InventoryController.Instance.AddItemToCollection(m_ItemName, amountToAdd, "Inventory", out added);
		}

		/// <summary>
		/// 
		/// </summary>
		public GameObject CreatePickup(Vector3 position, Quaternion rotation)
		{
			ItemData itemData;
			bool canCreatePickup = 
				InventoryController.Instance.Database.FindItemByName(m_ItemName, out itemData) &&
				itemData.WorldObject;

			if(canCreatePickup)
			{
				GameObject pickup = (GameObject)GameObject.Instantiate(itemData.WorldObject, position, rotation);
				var pickupComponent = pickup.GetComponent<ItemPickup>();
				if(pickupComponent)
					pickupComponent.ItemToAdd.CurrentInStack = Random.Range(m_MinAmount, m_MaxAmount);

				return pickup;
			}

			return null;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class MineableObject : MonoBehaviour
	{
		/// <summary> </summary>
		public float Health { get { return m_CurrentHealth; } }

		/// <summary> </summary>
		public Message Destroyed = new Message();

		[SerializeField]
		private FPTool.ToolPurpose m_RequiredToolPurpose;

		[Range(0.01f, 1f)]
		[SerializeField]
		private float m_Resistance = 0.5f;

		[Header("On Destroy")]

		[SerializeField]
		private GameObject m_DestroyedObject;

		[SerializeField]
		private Vector3 m_DestroyedObjectOffset;

		[Header("Loot")]

		[SerializeField]
		private LootItem[] m_Loot;

		[SerializeField]
		private bool m_ShowGatherMessage = true;

		[SerializeField]
		private Color m_MessageColor = Color.grey;

		[SerializeField]
		private Color m_LootNameColor = Color.yellow;

		private float m_CurrentHealth = 100f;


		/// <summary>
		/// Gives the player the loot.
		/// </summary>
		public void OnToolHit(FPTool.ToolPurpose[] toolPurposes, float damage, float efficiency)
		{
			// Is the tool used good for destroying this type of object?
			bool hasTheRightTool = false;

			foreach(var purpose in toolPurposes)
				if(purpose == m_RequiredToolPurpose)
				{
					hasTheRightTool = true;
					break;
				}

			// If so, do damage, and also give the player loot.
			if(hasTheRightTool)
			{
				ReceiveDamage(damage);

				if(m_Loot.Length > 0)
					GiveLootToPlayer(efficiency);
			}
		}

		private void GiveLootToPlayer(float amountFactor)
		{
			int randomItemIndex = ProbabilityUtils.RandomChoiceFollowingDistribution(GetLootProbabilities());
			var randomItem = m_Loot[randomItemIndex];

			int added;
			randomItem.AddToInventory(out added, amountFactor);
			string colorToHex = ColorUtils.ColorToHex(m_LootNameColor);

			if(m_ShowGatherMessage && added > 0)
				MessageDisplayer.Instance.PushMessage(
					string.Format("Gathered <color={0}>{1}</color> x {2}", colorToHex, randomItem.ItemName, added),m_MessageColor);

			//print("Added loot with index: " + randomItemIndex);
		}

		/// <summary>
		/// 
		/// </summary>
		private void ReceiveDamage(float damage)
		{
			m_CurrentHealth += -damage * (1f - m_Resistance);

			// The object doesn't have any health left.
			if(m_CurrentHealth < Mathf.Epsilon)
				DestroyObject();
		}

		private void DestroyObject()
		{
			if(m_DestroyedObject)
				Instantiate(m_DestroyedObject, transform.position + transform.TransformVector(m_DestroyedObjectOffset), Quaternion.identity);

			Destroy(gameObject);

			if(Destroyed != null)
				Destroyed.Send();

			//print("[DestructibleObject] - The object was completely destroyed!");
		}

		/// <summary>
		/// 
		/// </summary>
		private List<float> GetLootProbabilities()
		{
			List<float> probabilities = new List<float>();
			for(int i = 0;i < m_Loot.Length;i ++)
				probabilities.Add(m_Loot[i].SpawnChance);

			return probabilities;
		}
	}
}
