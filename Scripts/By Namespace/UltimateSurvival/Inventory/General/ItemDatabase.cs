using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// Represents an asset that stores all the user-defined items.
	/// </summary>
	public class ItemDatabase : ScriptableObject
	{
		public ItemCategory[] Categories { get { return m_Categories; } }

		[SerializeField]
		private ItemCategory[] m_Categories;

		[SerializeField]
		private ItemProperty.Definition[] m_ItemProperties;


		/// <summary>
		/// 
		/// </summary>
		public bool FindItemById(int id, out ItemData itemData)
		{
			for(int i = 0;i < m_Categories.Length;i ++)
			{
				var category = m_Categories[i];
				for(int j = 0;j < category.Items.Length;j ++)
				{
					if(category.Items[j].Id == id)
					{
						itemData = category.Items[j];
						return true;
					}
				}
			}

			itemData = null;
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		public bool FindItemByName(string name, out ItemData itemData)
		{
			for(int i = 0;i < m_Categories.Length;i ++)
			{
				var category = m_Categories[i];
				for(int j = 0;j < category.Items.Length;j ++)
				{
					if(category.Items[j].Name == name)
					{
						itemData = category.Items[j];
						return true;
					}
				}
			}

			itemData = null;
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		public bool FindRecipeById(int id, out Recipe recipe)
		{
			for(int i = 0;i < m_Categories.Length;i ++)
			{
				var category = m_Categories[i];
				for(int j = 0;j < category.Items.Length;j ++)
				{
					if(category.Items[j].Id == id)
					{
						recipe = category.Items[j].Recipe;
						return true;
					}
				}
			}

			recipe = null;
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		public bool FindRecipeByName(string name, out Recipe recipe)
		{
			for(int i = 0;i < m_Categories.Length;i ++)
			{
				var category = m_Categories[i];
				for(int j = 0;j < category.Items.Length;j ++)
				{
					if(category.Items[j].Name == name && category.Items[j].IsCraftable)
					{
						recipe = category.Items[j].Recipe;
						return true;
					}
				}
			}

			recipe = null;
			return false;
		}

		public List<string> GetAllItemNames()
		{
			List<string> names = new List<string>();

			for(int i = 0;i < m_Categories.Length;i ++)
			{
				var category = m_Categories[i];
				for(int j = 0;j < category.Items.Length;j ++)
				{
					names.Add(category.Items[j].Name);
				}
			}

			return names;
		}

		public int GetItemCount()
		{
			int count = 0;
			
			for(int c = 0;c < m_Categories.Length;c ++)
				count += m_Categories[c].Items.Length;

			return count;
		}
			
		private void OnValidate()
		{
			int currentID = 0;
			foreach(var category in m_Categories)
			{
				for(int j = 0;j < category.Items.Length;j ++)
				{
					category.Items[j].Id = currentID;
					category.Items[j].Category = category.Name;

					currentID ++;
				}
			}
		}
	}
}
