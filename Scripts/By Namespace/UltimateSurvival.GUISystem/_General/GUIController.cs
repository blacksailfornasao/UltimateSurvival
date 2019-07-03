using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateSurvival.GUISystem
{
	public class GUIController : MonoSingleton<GUIController>
	{
		public PlayerEventHandler Player { get; private set; }

		/// <summary>The main Canvas that's used for the GUI elements.</summary>
		public Canvas Canvas { get { return m_Canvas; } }

		/// <summary>All the item collections that are part of the GUI.</summary>
		public ItemContainer[] Containers { get; private set; }

		public Font Font { get { return m_Font; } }

		[Header("Setup")]

		[SerializeField]
		private Canvas m_Canvas;

		[SerializeField]
		private Camera m_GUICamera;

		[SerializeField]
		private Font m_Font;

		[SerializeField]
		[Reorderable]
		[Tooltip("If the player clicks while on those rects, the current selection will not be lost.")]
		private ReorderableRectTransformList m_SelectionBlockers;

		[Header("Audio")]

		[SerializeField]
		private AudioClip m_InventoryOpenClip;

		[SerializeField]
		private AudioClip m_InventoryCloseClip;


		public ItemContainer GetContainer(string name)
		{
			for(int i = 0;i < Containers.Length;i ++)
				if(Containers[i].Name == name)
					return Containers[i];

			Debug.LogWarning("No container with the name " + name + " found!");

			return null;
		}

		public bool MouseOverSelectionKeeper()
		{
			for (int i = 0; i < m_SelectionBlockers.Count; i++) 
			{
				if(!m_SelectionBlockers[i].gameObject.activeSelf)
					continue;
				
				bool containsPoint = RectTransformUtility.RectangleContainsScreenPoint(m_SelectionBlockers[i], Input.mousePosition, m_GUICamera);
				if(containsPoint)
					return true;
			}

			return false;
		}

		public void ApplyForAllCollections()
		{
			foreach(var collection in GetComponentsInChildren<ItemContainer>(true))
				collection.ApplyAll();
		}

		private void Awake()
		{
			Containers = GetComponentsInChildren<ItemContainer>(true);
			Player = GameController.LocalPlayer;

			DontDestroyOnLoad(gameObject);
		}

		private void Start()
		{
			InventoryController.Instance.State.AddChangeListener(OnChanged_InventoryState);
		}

		private void OnChanged_InventoryState()
		{
			if(!InventoryController.Instance.IsClosed)
				GameController.Audio.Play2D(m_InventoryOpenClip, 0.6f);
			else
				GameController.Audio.Play2D(m_InventoryCloseClip, 0.6f);
		}
	}
}
