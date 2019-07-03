using System.Collections;
using UnityEngine;

namespace UltimateSurvival.GUISystem
{
	/// <summary>
	/// Animates a window (e.g. Inventory, Crafting, Item Inspector).
	/// A panel has the following animations: Hide, Show, Refresh (optional)
	/// </summary>
	[RequireComponent(typeof(Animator))]
	public class Window : MonoBehaviour 
	{
		public enum OpenTrigger { InventoryOpened, SpecificState, Manual }

		public bool IsOpen { get; private set; }

		[Header("Animation Speed")]

		[SerializeField] 
		private float m_HideSpeed = 1.3f;

		[SerializeField] 
		private float m_ShowSpeed = 1.3f;

		[SerializeField] 
		private float m_RefreshSpeed = 1.3f;

		[Header("How Is Opened")]

		[SerializeField]
		[Tooltip("Inventory Opened - will open when the inventory is opened. " +
			"\nSpecific State - will open when the inventory opens in a specific state (e.g. Furnace-mode, Campfire-mode). " +
			"\nManual - will have to be manually opened from another script.")]
		private OpenTrigger m_OpenTrigger;

		[SerializeField]
		private ET.InventoryState m_StateTrigger = ET.InventoryState.Normal;

		private Animator m_Animator;


		public virtual void Open()
		{
			if(IsOpen)
			{
				m_Animator.SetTrigger("Refresh");
				return;
			}
				
			StopAllCoroutines();
			GetComponent<CanvasGroup>().interactable = true;
			GetComponent<CanvasGroup>().blocksRaycasts = true;
			UpdateAnimatorParams();
			m_Animator.SetTrigger("Show");
			IsOpen = true;
		}

		public virtual void Close(bool instant = false)
		{
			if(!IsOpen)
				return;

			UpdateAnimatorParams();

			if(!instant)
			{
				m_Animator.SetTrigger("Hide");
				StartCoroutine(C_DisableWithDelay());
			}
			else
			{
				m_Animator.Play("Hide", 0, 1);
				Disable();
			}

			IsOpen = false;
		}

		public void Refresh()
		{
			m_Animator.SetTrigger("Refresh");
		}

		protected virtual void Start()
		{
			IsOpen = true;
			m_Animator = GetComponent<Animator>();
			UpdateAnimatorParams();

			InventoryController.Instance.State.AddChangeListener(OnChanged_InventoryState);
			Close(true);
		}

		private void OnChanged_InventoryState()
		{
			bool isOpen = false;
			if(!InventoryController.Instance.IsClosed)
			{
				bool isManual = m_OpenTrigger == OpenTrigger.Manual;
				isOpen = !isManual && (m_OpenTrigger == OpenTrigger.InventoryOpened || InventoryController.Instance.State.Is(m_StateTrigger));
			}

			if(isOpen)
				Open();
			else
				Close();
		}

		private IEnumerator C_DisableWithDelay()
		{
			yield return new WaitForSeconds(0.5f);

			Disable();
		}

		private void Disable()
		{
			GetComponent<CanvasGroup>().interactable = false;
			GetComponent<CanvasGroup>().blocksRaycasts = false;
		}

		private void OnValidate()
		{
			UpdateAnimatorParams();
		}

		private void UpdateAnimatorParams()
		{
			if(!m_Animator)
				return;

			m_Animator.SetFloat("Hide Speed", m_HideSpeed);
			m_Animator.SetFloat("Show Speed", m_ShowSpeed);
			m_Animator.SetFloat("Refresh Speed", m_RefreshSpeed);
		}
	}
}
