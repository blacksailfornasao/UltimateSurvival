using System;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateSurvival.GUISystem
{
	public class BuildingCategory : MonoBehaviour 
	{
		public string CategoryName { get { return m_CategoryName; } }

		public Vector2 DesiredOffset { get { return m_DesiredOffset; } }

		public bool ShowPieces { get { return m_ShowPieces; } set { foreach(var piece in m_Pieces) piece.gameObject.SetActive(value); m_ShowPieces = value; } }

		public float Distance { get { return m_Distance; } }

		public float Offset { get { return m_Offset; } }

		public float Spacing { get { return m_Spacing; } }

		[SerializeField]
		private string m_CategoryName;

		[SerializeField]
		private Vector2 m_DesiredOffset;

		[SerializeField]
		[Range(0.5f, 2f)]
		private float m_SelectionScale = 1.1f;

		[Header("Layout")]

		[SerializeField]
		private float m_Distance = 211.7f;
	
		[SerializeField]
		[Range(-90f, 90f)]
		private float m_Offset;

		[SerializeField]
		[Range(-90f, 90f)]
		private float m_Spacing;

		private BuildingPiece[] m_Pieces;
		private bool m_ShowPieces;

		private BuildingPiece m_HighlightedPiece;
		private float m_ClosestPieceAngle;

		private int m_CurrentIndex;


		public BuildingPiece SelectFirst()
		{
			if(m_Pieces.Length > 0)
			{
				Select(0);
				return m_HighlightedPiece;
			}
			else
				return null;
		}

		public BuildingPiece SelectNext()
		{
			Select(m_CurrentIndex + 1);

			return m_HighlightedPiece;
		}

		public BuildingPiece SelectPrevious()
		{
			Select(m_CurrentIndex - 1);

			return m_HighlightedPiece;
		}

		private void Select(int index)
		{
			m_CurrentIndex = (int)Mathf.Repeat(index, m_Pieces.Length);

			m_HighlightedPiece = m_Pieces[m_CurrentIndex];

			for(int i = 0;i < m_Pieces.Length;i ++)
			{
				if(m_Pieces[i] == m_HighlightedPiece)
				{
					m_Pieces[i].transform.localScale = m_SelectionScale * Vector3.one;
					m_Pieces[i].SetCustomColor(new Color(0f, 1f, 0f, 0.85f));
				}
				else
				{
					m_Pieces[i].transform.localScale = Vector3.one;
					m_Pieces[i].SetDefaultColor();
				}
			}
		}
			
		private void Awake()
		{
			m_Pieces = GetComponentsInChildren<BuildingPiece>();
		}
	}
}
