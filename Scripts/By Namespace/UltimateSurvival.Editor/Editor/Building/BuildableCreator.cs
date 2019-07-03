using UnityEngine;
using UnityEngine.AI;
using UltimateSurvival.AI;

namespace UltimateSurvival.Editor
{
	using UnityEditor;

	public class BuildableCreator : ScriptableWizard 
	{
		[SerializeField]
		private GameObject m_Model;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_Resistance = 0.9f;

		[SerializeField]
		private bool m_IsObstacleForAI;


		[MenuItem("Tools/Ultimate Survival/Add/Buildable Creator", false, 10)]
		private static void CreateWizard()
		{
			ScriptableWizard.DisplayWizard("Buildable Creator", typeof(BuildableCreator));
		}

		private void OnWizardCreate()
		{
			if(m_Model == null)
				return;
			
			Undo.RecordObject(m_Model, "");
			m_Model.layer = LayerMask.NameToLayer("Building Piece");

			// Main collider
			Undo.AddComponent<BoxCollider>(m_Model);

			// entity event handler
			Undo.AddComponent<EntityEventHandler>(m_Model);

			// Entity Death Handler
			var deathHandler = Undo.AddComponent<EntityDeathHandler>(m_Model);
			var deathHandlerSer = new SerializedObject(deathHandler);

			deathHandlerSer.Update();
	
			deathHandlerSer.FindProperty("m_DestroyTimer").floatValue = 0f;

			deathHandlerSer.ApplyModifiedProperties();

			// Piece Vitals
			var pieceVitals = Undo.AddComponent<PieceVitals>(m_Model);
			var pieceVitalsSer = new SerializedObject(pieceVitals);

			pieceVitalsSer.Update();

			pieceVitalsSer.FindProperty("m_Resistance").floatValue = m_Resistance;

			pieceVitalsSer.ApplyModifiedProperties();

			// Nav Mesh Obstacle
			if(m_IsObstacleForAI)
			{
				var obstacle = Undo.AddComponent<NavMeshObstacle>(m_Model);
				obstacle.carving = true;
			}

			// Building Piece
			Undo.AddComponent<Building.BuildingPiece>(m_Model);
		}
	}
}
