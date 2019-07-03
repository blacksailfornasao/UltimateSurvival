using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateSurvival.GUISystem
{
	/// <summary>
	/// Animates the fire icon when the smelting station is working.
	/// </summary>
	[RequireComponent(typeof(Image))]
	[RequireComponent(typeof(Animator))]
	public class FlamesIcon : MonoBehaviour 
	{
		[SerializeField] 
		private SmeltingStationGUI m_SmeltingStationGUI;

		private Animator m_Animator;


		private void Start()
		{
			m_Animator = GetComponent<Animator>();
			m_SmeltingStationGUI.IsBurning.AddChangeListener(OnChanged_SmeltingStation_IsBurning);
			InventoryController.Instance.State.AddChangeListener(OnChanged_InventoryController_State);
		}

		private void OnChanged_InventoryController_State()
		{
			if(InventoryController.Instance.IsClosed)
				m_Animator.SetBool("Loop", false);
			else if(m_SmeltingStationGUI.IsBurning.Get())
				m_Animator.SetBool("Loop", true);
		}

		private void OnChanged_SmeltingStation_IsBurning()
		{
			m_Animator.SetBool("Loop", m_SmeltingStationGUI.IsBurning.Get());
		}
	}
}
