using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UltimateSurvival.GUISystem
{
	[Serializable]
	public class DurabilityBar
	{
		public bool Active { get { return m_Active; } }

		[SerializeField]
		private GameObject m_Background;

		[SerializeField]
		private Image m_Bar;

		[SerializeField]
		private Gradient m_ColorGradient;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_Durability;

		private bool m_Active = true;


		public void SetActive(bool active)
		{
			m_Background.SetActive(active);
			m_Bar.enabled = active;
			m_Active = active;
		}

		public void SetFillAmount(float fillAmount)
		{
			m_Bar.color = m_ColorGradient.Evaluate(fillAmount);
			m_Bar.fillAmount = fillAmount;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class Slot : Selectable, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public delegate void BaseAction(BaseEventData data, Slot wrapper);
		public delegate void PointerAction(PointerEventData data, Slot wrapper);

		/// <summary></summary>
		public Message<Slot> Refreshed = new Message<Slot>();

		public event BaseAction E_Select;

		public event BaseAction E_Deselect;

		public event PointerAction PointerDown;

		public event PointerAction PointerUp;

		public event PointerAction BeginDrag;

		public event PointerAction Drag;

		public event PointerAction EndDrag;

		/// <summary></summary>
		public ItemHolder ItemHolder { get; private set; }

		/// <summary></summary>
		public ItemContainer Parent { get; private set; }

		/// <summary></summary>
		public bool	HasItem { get { return ItemHolder && ItemHolder.HasItem; } }

		/// <summary></summary>
		public SavableItem CurrentItem { get { return ItemHolder ? ItemHolder.CurrentItem : null; } }

		/// <summary></summary>
		public ReorderableStringList RequiredCategories { get { return m_RequiredCategories; } set { m_RequiredCategories = value; } }

		/// <summary></summary>
		public ReorderableStringList RequiredProperties { get { return m_RequiredProperties; } set { m_RequiredProperties = value; } }

		[Header("Setup")]

		[SerializeField]
		private Image m_ItemIcon;

		[SerializeField]
		private Text m_StackDisplayer;

		[SerializeField]
		private DurabilityBar m_DurabilityBar;

		[Header("Required Stuff")]

		[SerializeField]
		[Reorderable]
		private ReorderableStringList m_RequiredCategories;

		[SerializeField]
		[Reorderable]
		private ReorderableStringList m_RequiredProperties;
	
		private Coroutine m_ScaleSetter;


		public bool AllowsItem(SavableItem item)
		{
			bool isOfRequiredCategory = false;
			bool hasRequiredProperties = true;

			if(m_RequiredProperties.Count > 0)
			{
				for (int i = 0; i < m_RequiredProperties.Count; i++) 
					if(!item.HasProperty(m_RequiredProperties[i]))
					{
						hasRequiredProperties = false;
						break;
					}
			}

			if(m_RequiredCategories.Count > 0)
			{
				for (int i = 0; i < m_RequiredCategories.Count; i++) 
					if(m_RequiredCategories[i] == item.ItemData.Category)
					{
						isOfRequiredCategory = true;
						break;
					}
			}
			else
				isOfRequiredCategory = true;

			return isOfRequiredCategory && hasRequiredProperties;
		}

		public void LinkWithHolder(ItemHolder holder)
		{
			if(ItemHolder)
				ItemHolder.Updated.RemoveListener(On_ItemHolder_Updated);

			ItemHolder = holder;
			Refresh();

			ItemHolder.Updated.AddListener(On_ItemHolder_Updated);
		}

		/// <summary>
		/// Refreshes so that it displays the correct item data.
		/// </summary>
		public void Refresh()
		{
			m_ItemIcon.enabled = HasItem;
			m_StackDisplayer.enabled = HasItem && CurrentItem.CurrentInStack > 1;
			m_DurabilityBar.SetActive(HasItem && CurrentItem.HasProperty("Durability"));

			//interactable = HasItem;

			if(m_ItemIcon.enabled)
				m_ItemIcon.sprite = CurrentItem.ItemData.Icon;
			
			if(m_StackDisplayer.enabled)
				m_StackDisplayer.text = string.Format("x{0}", CurrentItem.CurrentInStack);

			if(m_DurabilityBar.Active)
				m_DurabilityBar.SetFillAmount(CurrentItem.GetPropertyValue("Durability").Float.Ratio);

			Refreshed.Send(this);
		}

		/// <summary>
		/// Will return a clone of this slot, without the background (useful when you want to simulate item dragging).
		/// </summary>
		public RectTransform GetDragTemplate(SavableItem forItem, float alpha)
		{
			var template = Instantiate<Slot>(this);

			// HACK: We shouldn't know about the frame object, only the hotbar script should know.
			var frame = template.transform.FindDeepChild("Frame");
			if(frame != null)
				Destroy(frame.gameObject);

			template.enabled = false;
			template.image.enabled = false;

			template.m_ItemIcon.enabled = true;

			template.m_StackDisplayer.enabled = forItem.CurrentInStack > 1;
			template.m_StackDisplayer.text = string.Format("x{0}", forItem.CurrentInStack);

			template.m_DurabilityBar.SetActive(forItem.HasProperty("Durability"));

			var group = template.gameObject.AddComponent<CanvasGroup>();
			group.alpha = alpha;

			return template.GetComponent<RectTransform>();
		}

		public void SetScale(Vector3 localScale, float duration)
		{
			if(IsDestroyed())
				return;
			
			if(m_ScaleSetter != null)
				StopCoroutine(m_ScaleSetter);
			m_ScaleSetter = StartCoroutine(C_SetScale(localScale, duration));
		}

		public override void OnSelect (BaseEventData data)
		{
			base.OnSelect (data);
			if(E_Select != null)
				E_Select(data, this);
		}

		public override void OnDeselect(BaseEventData data)
		{
			if(HasItem && !InventoryController.Instance.IsClosed && GUIController.Instance.MouseOverSelectionKeeper())
			{
				StartCoroutine(C_WaitAndSelect(1));
				return;
			}

			if(E_Deselect != null)
				E_Deselect(data, this);

			base.OnDeselect(data);
		}
			
		public override void OnPointerDown (PointerEventData data)
		{
			base.OnPointerDown (data);
			if(PointerDown != null)
				PointerDown(data, this);
		}

		public override void OnPointerUp (PointerEventData data)
		{
			base.OnPointerUp (data);
			if(PointerUp != null)
				PointerUp(data, this);
		}

		public void OnBeginDrag(PointerEventData data)
		{
			if(BeginDrag != null)
				BeginDrag(data, this);
		}

		public void OnDrag(PointerEventData data)
		{
			if(Drag != null)
				Drag(data, this);
		}

		public void OnEndDrag(PointerEventData data)
		{
			if(EndDrag != null)
				EndDrag(data, this);

			if(data.pointerCurrentRaycast.gameObject != gameObject)
			{
				EventSystem.current.SetSelectedGameObject(null);
				DoStateTransition(HasItem ? SelectionState.Normal : SelectionState.Disabled, true);
			}
		}
			
		protected override void Awake()
		{
			base.Awake();

			if(!Application.isPlaying)
				return;

			Parent = GetComponentInParent<ItemContainer>();

			m_ItemIcon.enabled = false;
			m_StackDisplayer.enabled = false;
			m_DurabilityBar.SetActive(false);
			//interactable = false;
		}

		private void On_ItemHolder_Updated(ItemHolder holder)
		{
			Refresh();
		}
			
		private IEnumerator C_SetScale(Vector3 localScale, float duration)
		{
			float finishTime = Time.time + duration;
			float scaleChangeSpeed = (localScale - transform.localScale).magnitude / duration;

			while(Time.time < finishTime)
			{
				transform.localScale = Vector3.MoveTowards(transform.localScale, localScale, scaleChangeSpeed * Time.deltaTime);
				yield return null;
			}
		}

		private IEnumerator C_WaitAndSelect(int waitFrameCount)
		{
			for(int i = 0;i < waitFrameCount;i ++)
				yield return null;

			Select();
		}
	}
}
