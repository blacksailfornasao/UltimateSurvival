using System;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateSurvival.GUISystem
{
	public class BuildingWheel : GUIBehaviour 
	{
		public RectTransform SelectionHighlight { get { return m_SelectionHighlight; } }

		public float Distance { get { return m_Distance; } }

		public float Offset { get { return m_Offset; } }

		public float Spacing { get { return m_Spacing; } }

		[SerializeField]
		private Window m_Window;

		[SerializeField]
		private Camera m_GUICamera;

		[SerializeField]
		private RectTransform m_SelectionHighlight;

		[SerializeField]
		private Text m_CategoryName;

		[SerializeField]
		private Text m_PieceName;

		[SerializeField]
		[Range(0f, 50f)]
		private float m_ScrollThreeshold = 1f;

		[Header("Audio")]

		[SerializeField]
		private SoundPlayer m_RefreshAudio;

		[SerializeField]
		private SoundPlayer m_SelectPieceAudio;

		[Header("Layout")]

		[SerializeField]
		private float m_Distance = 211.7f;

		[SerializeField]
		[Range(-90f, 90f)]
		private float m_Offset;

		[SerializeField]
		[Range(-90f, 90f)]
		private float m_Spacing;

		private BuildingCategory[] m_Categories;

		private BuildingCategory m_SelectedCategory = null;
		private int m_CategoryIndex = 0;

		private bool m_ChoosingPiece;
		private BuildingPiece m_SelectedPiece;
		private BuildingPiece m_HighlightedPiece;

		private float m_CategoryScrollPos;
		private float m_PieceScrollPos;


		private void Update()
		{
			if(!m_Window.IsOpen)
				return;

			float scrollValue = Player.ScrollValue.Get();

			if(m_SelectedCategory != null)
			{
				// If we're currently choosing a piece from a category, show the highlighted piece's name.
				if(m_ChoosingPiece && m_HighlightedPiece != null)
				{
					if(!m_PieceName.enabled)
						m_PieceName.enabled = true;

					m_PieceName.text = m_HighlightedPiece.PieceName;
				}
				// Otherwise show the selected piece's name, if we have one selected.
				else if(!m_ChoosingPiece)
				{
					if(!m_PieceName.enabled)
						m_PieceName.enabled = true;

					m_PieceName.text = m_SelectedPiece == null ? "" : m_SelectedPiece.PieceName;
				}
					
				if(Input.GetKeyDown(KeyCode.Mouse0))
				{
					// Close the wheel if we clicked on the None category.
					if(m_SelectedCategory.CategoryName == "None")
					{
						Player.SelectedBuildable.Set(null);
						Player.SelectBuildable.TryStop();

						m_PieceName.enabled = false;

						return;
					}

					// If we're choosing a piece, select the highlighted piece.
					if(m_ChoosingPiece)
					{
						m_SelectedPiece = m_HighlightedPiece;

						Player.SelectedBuildable.Set(m_SelectedPiece.BuildableObject);

						m_SelectPieceAudio.Play2D();
					}
						
					m_ChoosingPiece = !m_ChoosingPiece;
				}

				if(m_ChoosingPiece)
				{
					if(!m_SelectedCategory.ShowPieces)
					{
						m_SelectedCategory.ShowPieces = true;
						m_PieceScrollPos = 0f;

						m_HighlightedPiece = m_SelectedCategory.SelectFirst();
					}

					else
					{
						m_PieceScrollPos += scrollValue;
						m_PieceScrollPos = Mathf.Clamp(m_PieceScrollPos, -m_ScrollThreeshold, m_ScrollThreeshold);

						if(Mathf.Abs(m_PieceScrollPos - m_ScrollThreeshold * Mathf.Sign(scrollValue)) < Mathf.Epsilon)
						{
							m_PieceScrollPos = 0f;

							if(scrollValue > 0f)
								m_HighlightedPiece = m_SelectedCategory.SelectNext();
							else
								m_HighlightedPiece = m_SelectedCategory.SelectPrevious();
						}
					}

					return;
				}
				else if(m_SelectedCategory.ShowPieces)
					m_SelectedCategory.ShowPieces = false;
			}

			m_CategoryScrollPos += scrollValue;
			m_CategoryScrollPos = Mathf.Clamp(m_CategoryScrollPos, -m_ScrollThreeshold, m_ScrollThreeshold);

			var lastSelectedCateg = m_SelectedCategory;

			if(Mathf.Abs(m_CategoryScrollPos - m_ScrollThreeshold * Mathf.Sign(scrollValue)) < Mathf.Epsilon)
			{
				m_CategoryScrollPos = 0f;

				m_CategoryIndex = (int)Mathf.Repeat(m_CategoryIndex + (scrollValue > 0f ? 1 : -1), m_Categories.Length);
				m_SelectedCategory = m_Categories[m_CategoryIndex];
			}

			if(lastSelectedCateg != m_SelectedCategory)
			{
				m_Window.Refresh();
				m_RefreshAudio.Play2D();

				m_CategoryName.text = m_SelectedCategory.CategoryName;
			}

			if(m_SelectedCategory != null)
			{
				float angle = Offset + Spacing * m_CategoryIndex;
				m_SelectionHighlight.localPosition = (Quaternion.Euler(Vector3.back * angle) * Vector3.up) * Distance;
				m_SelectionHighlight.localRotation = Quaternion.Euler(Vector3.back * angle);
			}
		}

		private void Start()
		{
			m_Categories = GetComponentsInChildren<BuildingCategory>(false);

			Player.SelectBuildable.AddStartTryer(TryStart_SelectBuildable);
			Player.SelectBuildable.AddStopTryer(TryStop_SelectBuildable);

			InventoryController.Instance.State.AddChangeListener(OnChanged_InventoryState);

			foreach(var category in m_Categories)
				category.ShowPieces = false;

			m_PieceName.enabled = false;
		}
			
		private void OnChanged_InventoryState()
		{
			var inventoryClosed = InventoryController.Instance.IsClosed;
			if(!inventoryClosed)
			{
				while(Player.SelectBuildable.Active)
					Player.SelectBuildable.TryStop();
			}
		}

		private bool TryStart_SelectBuildable()
		{
			if(!InventoryController.Instance.IsClosed || !Player.EquippedItem.Get() || !Player.EquippedItem.Get().HasProperty("Allows Building"))
				return false;

			m_Window.Open();
			Player.ViewLocked.Set(true);

			return true;
		}

		private bool TryStop_SelectBuildable()
		{
			if(m_ChoosingPiece)
			{
				m_ChoosingPiece = false;
				return false;
			}
			else
			{
				m_Window.Close();
				Player.ViewLocked.Set(false);

				return true;
			}
		}
	}
}
