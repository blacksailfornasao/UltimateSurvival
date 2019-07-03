using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival.GUISystem
{
	/// <summary>
	/// A simple dummy class that holds a list of game objects, and has a name.
	/// </summary>
	public class ObjectHolder
	{
		public string Name { get { return m_Name; } }

		public List<GameObject> ObjectList { get { return m_ObjectList; } }

		private string m_Name;
		private List<GameObject> m_ObjectList;


		public static implicit operator bool(ObjectHolder holder)
		{
			return holder != null;
		}

		public ObjectHolder(string name, List<GameObject> objectList)
		{
			m_Name = name;
			m_ObjectList = objectList;
		}

		public void ActivateObjects(bool active)
		{
			for(int i = 0;i < m_ObjectList.Count;i ++)
				m_ObjectList[i].SetActive(active);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class CraftingList : MonoBehaviour 
	{
		/// <summary></summary>
		public Message RecipesGenerated = new Message();

		/// <summary></summary>
		public List<ObjectHolder> RecipesByCategory { get { return m_RecipesByCategory; } }

		[SerializeField]
		private CraftingMenu m_CraftingMenu;

		[SerializeField] 
		private RectTransform m_ListTransform;

		[SerializeField]
		private RecipeSlot m_RecipeTemplate;

		private List<ObjectHolder> m_RecipesByCategory = new List<ObjectHolder>();


		private void Awake() 
		{
			m_CraftingMenu.SelectedCategory.AddChangeListener(OnChanged_SelectedCategory);
			m_CraftingMenu.SelectedCategory.AddChangeListener(OnChanged_Category);
		}
			
		private void Start()
		{
			GenerateRecipes();
			m_ListTransform.anchoredPosition = new Vector2(m_ListTransform.anchoredPosition.x, 0f);
		}

		private void OnChanged_SelectedCategory()
		{
			m_ListTransform.anchoredPosition = new Vector2(m_ListTransform.anchoredPosition.x, 0f);
		}

		private void OnChanged_Category()
		{
			var selected = m_CraftingMenu.SelectedCategory.Get();

			foreach(var category in m_RecipesByCategory)
				category.ActivateObjects(selected.HasCategory(category.Name));
		}

		private void GenerateRecipes()
		{
			var database = InventoryController.Instance.Database;
			m_RecipeTemplate.gameObject.SetActive(true);

			foreach(var category in database.Categories)
			{
				var objectList = new List<GameObject>();
				var holder = new ObjectHolder(category.Name, objectList);
				m_RecipesByCategory.Add(holder);

				foreach(var item in category.Items)
				{
					if(item.IsCraftable)
					{
						var recipe = item.Recipe;

						var newRecipe = ((GameObject)Instantiate(m_RecipeTemplate.gameObject, m_ListTransform.position, m_ListTransform.rotation, m_ListTransform)).GetComponent<RecipeSlot>();
						newRecipe.name = string.Format("Recipe Slot ({0})", item.Name);
						newRecipe.ShowRecipeForItem(item);

						objectList.Add(newRecipe.gameObject);
					}
				}
			}

			m_RecipeTemplate.gameObject.SetActive(false);
			RecipesGenerated.Send();
		}
	}
}
