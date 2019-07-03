using System;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateSurvival.GUISystem
{
	/// <summary>
	/// 
	/// </summary>
	public class CraftingMenu : MonoBehaviour 
	{
		/// <summary></summary>
		public Value<CraftingCategory> SelectedCategory = new Value<CraftingCategory>(null);

		[SerializeField]
		private CraftingCategory m_FirstSelected;

		[SerializeField] 
		private GameObject m_SelectionHighlight;

		[SerializeField] 
		private Text m_CategoryName;

		private CraftingCategory[] m_Categories;
		private GameObject m_SpawnedBackground;


		private void Awake()
		{
			m_Categories = GetComponentsInChildren<CraftingCategory>();
			if(m_Categories.Length > 0)
			{
				foreach(var category in m_Categories)
					category.Selected.AddListener(On_CategorySelected);
			}
			else
				Debug.LogWarning("No categories were found as children, this menu is useless!", this);
		}

		private void Start()
		{
			// Prepare the background object for the selected category.
			if(m_SelectionHighlight)
			{
				m_SpawnedBackground = (GameObject)Instantiate(m_SelectionHighlight, m_Categories[0].transform.parent);
				m_SpawnedBackground.transform.localScale = Vector3.one;
				m_SpawnedBackground.SetActive(false);
			}

			if(m_FirstSelected)
			{
				SelectedCategory.Set(m_FirstSelected);
				On_CategorySelected(m_FirstSelected);
			}	
		}

		private void On_CategorySelected(CraftingCategory selectedCategory)
		{
			if(m_SelectionHighlight)
			{
				if(!m_SpawnedBackground.activeSelf)
					m_SpawnedBackground.SetActive(true);
				
				if(m_SpawnedBackground.transform.GetSiblingIndex() > 0)
					m_SpawnedBackground.transform.SetAsFirstSibling();
				
				m_SpawnedBackground.transform.position = selectedCategory.transform.position;
			}

			if(m_CategoryName)
				m_CategoryName.text = selectedCategory.DisplayName;

			SelectedCategory.Set(selectedCategory);
		}
	}
}
