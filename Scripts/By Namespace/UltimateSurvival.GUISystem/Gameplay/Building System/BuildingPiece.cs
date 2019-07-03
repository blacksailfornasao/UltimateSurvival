using UnityEngine;
using UnityEngine.UI;

namespace UltimateSurvival.GUISystem
{
	public class BuildingPiece : MonoBehaviour 
	{
		public string PieceName { get { return m_PieceName; } }

		public Sprite Icon { get { return m_Image.sprite; } }

		public Vector2 DesiredOffset { get { return m_DesiredOffset; } }

		public UltimateSurvival.Building.BuildingPiece BuildableObject { get { return m_BuildableObject; } }

		[SerializeField]
		private string m_PieceName;

		[SerializeField]
		private Vector2 m_DesiredOffset;

		[SerializeField]
		private UltimateSurvival.Building.BuildingPiece m_BuildableObject;

		private Image m_Image;
		private Color m_DefaultColor;


		public void SetCustomColor(Color color)
		{
			m_Image.color = color;
		}

		public void SetDefaultColor()
		{
			m_Image.color = m_DefaultColor;
		}

		private void Awake()
		{
			m_Image = GetComponent<Image>();
			m_DefaultColor = m_Image.color;
		}
	}
}
