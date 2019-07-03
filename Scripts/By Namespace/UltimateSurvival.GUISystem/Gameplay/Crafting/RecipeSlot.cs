using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UltimateSurvival.GUISystem
{
	/// <summary>
	/// 
	/// </summary>
	public class RecipeSlot : Selectable
	{
		public delegate void BaseAction(BaseEventData data, RecipeSlot slot);
		public delegate void PointerAction(PointerEventData data, RecipeSlot slot);

		/// <summary>  </summary>
		public event BaseAction E_Deselect;

		/// <summary>  </summary>
		public event PointerAction PointerUp;	

		/// <summary>  </summary>
		public ItemData Result { get; private set; }

		[Header("Setup")]

		[SerializeField]
		private Text m_RecipeName;

		[SerializeField] 
		private Image m_Icon;

		/// <summary>
		/// 
		/// </summary>
		public void ShowRecipeForItem(ItemData item)
		{
			var database = InventoryController.Instance.Database;
			Result = item;
            m_RecipeName.text = (item.DisplayName == string.Empty) ? item.Name : item.DisplayName;
            m_Icon.sprite = item.Icon;
		}

		public override void OnDeselect (BaseEventData data)
		{
			if(GUIController.Instance.MouseOverSelectionKeeper())
			{
				StartCoroutine(C_WaitAndSelect(1));
				return;
			}

			if(E_Deselect != null)
				E_Deselect(data, this);

			base.OnDeselect (data);
		}

		public override void OnPointerUp (PointerEventData data)
		{
			base.OnPointerUp (data);
			if(PointerUp != null)
				PointerUp(data, this);
		}

		private IEnumerator C_WaitAndSelect(int waitFrameCount)
		{
			for(int i = 0;i < waitFrameCount;i ++)
				yield return null;

			Select();
		}
	}
}
