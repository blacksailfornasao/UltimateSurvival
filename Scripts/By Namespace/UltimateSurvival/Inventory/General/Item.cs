using System;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival
{
	[Serializable]
	public class ItemCategory
	{
		public string Name { get { return m_Name; } }

		public ItemData[] Items { get { return m_Items; } }

		[SerializeField]
		private string m_Name;

		[SerializeField]
		private ItemData[] m_Items;
	}

	/// <summary>
	/// The data / definition for an item.
	/// </summary>
	[Serializable]
	public class ItemData
	{
		public string Name { get { return m_Name; } }

        public string DisplayName { get { return m_DisplayName; } }

		public int Id { get { return m_Id; } set { m_Id = value; } }

		public string Category { get { return m_Category; } set { m_Category = value; } }

		public Sprite Icon { get { return m_Icon; } }

		public GameObject WorldObject { get { return m_WorldObject; } }

		public string[] Descriptions { get { return m_Descriptions; } }

		public int StackSize { get { return m_StackSize; } }

		public List<ItemProperty.Value> PropertyValues { get { return m_PropertyValues; } }

		public bool IsBuildable { get { return m_IsBuildable; } }

		public bool IsCraftable { get { return m_IsCraftable; } }

		public Recipe Recipe { get { return m_Recipe; } }

		[SerializeField]
		private string m_Name;

        [SerializeField]
        private string m_DisplayName;

		[SerializeField]
		private int m_Id;

		[SerializeField]
		private string m_Category;

		[SerializeField]
		private Sprite m_Icon;

		[SerializeField]
		private GameObject m_WorldObject;

		[SerializeField]
		[Multiline]
		private string[] m_Descriptions;

		[SerializeField]
		private int m_StackSize = 1;

		[SerializeField]
		private List<ItemProperty.Value> m_PropertyValues;

		[SerializeField]
		private bool m_IsBuildable;

		[SerializeField]
		private bool m_IsCraftable;

		[SerializeField]
		private Recipe m_Recipe;
	}

	/// <summary>
	/// A SavableItem is an instance of an item, which can have it's own properties, vs ItemData which is just the data for an item, just the definition.
	/// </summary>
	[Serializable]
	public class SavableItem
	{
		public Message<ItemProperty.Value> PropertyChanged = new Message<ItemProperty.Value>();

		public Message StackChanged = new Message();

		public bool Initialized { get; private set; }

		public ItemData ItemData { get; private set; }

		public int Id { get { return ItemData.Id; } }

		public string Name { get { return ItemData.Name; } }

		public int CurrentInStack { get { return m_CurrentInStack; } set { m_CurrentInStack = value; StackChanged.Send(); } }

		public List<ItemProperty.Value> CurrentPropertyValues { get { return m_CurrentPropertyValues; } }

		[SerializeField]
		private int m_CurrentInStack;

		[SerializeField]
		private List<ItemProperty.Value> m_CurrentPropertyValues;


		public static implicit operator bool(SavableItem item) { return item != null; }

		/// <summary>
		/// 
		/// </summary>
		public SavableItem(ItemData data, int currentInStack = 1, List<ItemProperty.Value> customPropertyValues = null)
		{
			CurrentInStack = Mathf.Clamp(currentInStack, 1, data.StackSize);

			if(customPropertyValues != null)
				m_CurrentPropertyValues = CloneProperties(customPropertyValues);
			else
				m_CurrentPropertyValues = CloneProperties(data.PropertyValues);
			
			ItemData = data;
			Initialized = true;

			for(int i = 0;i < m_CurrentPropertyValues.Count;i ++)
				m_CurrentPropertyValues[i].Changed.AddListener(On_PropertyChanged);
		}

		/// <summary>
		/// Must be called after the item was loaded / deserialized, (not required when the item is created at runtime, through the constructor).
		/// </summary>
		public void OnLoad(ItemDatabase itemDatabase)
		{
			if(itemDatabase)
			{
				ItemData data;
				if(itemDatabase.FindItemById(Id, out data))
				{
					ItemData = data;
					Initialized = true;

					for(int i = 0;i < m_CurrentPropertyValues.Count;i ++)
						m_CurrentPropertyValues[i].Changed.AddListener(On_PropertyChanged);
				}
				else
					Debug.LogErrorFormat("[SavableItem] - This item couldn't be initialized and will not function properly. No item with the name {0} was found in the database!", Name);
			}
			else
				Debug.LogError("[SavableItem] - This item couldn't be initialized and will not function properly. The item database provided is null!");
		}

		public string GetDescription(int index)
		{
			string description = string.Empty;
			if(index > -1 && ItemData.Descriptions.Length > index)
			{
				try
				{
					description = string.Format(ItemData.Descriptions[index], m_CurrentPropertyValues.ToArray());
				}
				catch
				{
					Debug.LogError("[SavableItem] - You tried to access a property through the item description, but the property doesn't exist. The item name is: " + Name);
				}
			}

			return description;
		}

		public bool HasProperty(string name)
		{
			if(!Initialized)
			{
				Debug.LogError("[SavableItem] - This SavableItem is not initialized, probably it was loaded and not initialized! (call OnLoad() after loading / deserializing).");
				return false;
			}

			for(int i = 0;i < m_CurrentPropertyValues.Count;i ++)
				if(m_CurrentPropertyValues[i].Name == name)
					return true;

			return false;
		}

		/// <summary>
		/// Use this if you are sure the item has this property.
		/// </summary>
		public ItemProperty.Value GetPropertyValue(string name)
		{
			ItemProperty.Value propertyValue = null;

			if(!Initialized)
			{
				Debug.LogError("[SavableItem] - This SavableItem is not initialized, probably it was loaded and not initialized! (call OnLoad() after loading / deserializing).");
				return null;
			}

			for(int i = 0;i < m_CurrentPropertyValues.Count;i ++)
				if(m_CurrentPropertyValues[i].Name == name)
				{
					propertyValue = m_CurrentPropertyValues[i];
					break;
				}

			return propertyValue;
		}

		/// <summary>
		/// Use this if you are NOT sure the item has this property.
		/// </summary>
		public bool FindPropertyValue(string name, out ItemProperty.Value propertyValue)
		{
			propertyValue = null;

			if(!Initialized)
			{
				Debug.LogError("[SavableItem] - This SavableItem is not initialized, probably it was loaded and not initialized! (call OnLoad() after loading / deserializing).");
				return false;
			}

			for(int i = 0;i < m_CurrentPropertyValues.Count;i ++)
				if(m_CurrentPropertyValues[i].Name == name)
				{
					propertyValue = m_CurrentPropertyValues[i];
					return true;
				}
					
			return false;
		}

		private List<ItemProperty.Value> CloneProperties(List<ItemProperty.Value> properties)
		{
			List<ItemProperty.Value> list = new List<ItemProperty.Value>();
			for(int i = 0;i < properties.Count;i ++)
				list.Add(properties[i].GetClone());

			return list;
		}

		private void On_PropertyChanged(ItemProperty.Value propertyValue)
		{
			PropertyChanged.Send(propertyValue);
		}
	}
}
