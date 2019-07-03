using System.Collections.Generic;
using UnityEditorInternal;
using UltimateSurvival.Building;

namespace UltimateSurvival.Editor
{
	using UnityEditor;
	using UnityEngine;

	[CustomEditor(typeof(Socket))]
	[CanEditMultipleObjects]
	public class SocketEditor : Editor 
	{
		private static bool m_InitializedSelector;
		private static Socket m_SelectedSocket;
		private static BuildingPiece m_SelectedPiece;

		private static Material m_PreviewMat;

		private Socket m_Socket;

		private bool m_EditOffset;
		private SerializedProperty m_Radius;
		private int m_SelectedPieceIdx;

		private ReorderableList m_PieceOffsets;

		private Socket.PieceOffset m_SelectedPieceOffset;
	

		public override void OnInspectorGUI ()
		{
			//base.OnInspectorGUI();

			serializedObject.Update();

			if(Application.isPlaying)
			{
				GUILayout.Label("Occupied Spaces: ", EditorStyles.boldLabel);
				EditorGUILayout.Space();

				for(int i = 0;i < m_Socket.OccupiedSpaces.Count;i ++)
					GUILayout.Label(m_Socket.OccupiedSpaces[i].ToString());
			}

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(m_Radius);
			CustomGUI.DoHorizontalLine();
			EditorGUILayout.Space();

			GUI.color = m_EditOffset ? Color.grey : Color.white;

			if(GUILayout.Button("Edit Piece Offset"))
			{
				m_EditOffset = !m_EditOffset;

                if (!m_EditOffset)
                    Tools.current = Tool.Move;
            }

			GUI.color = Color.white;

			if(!serializedObject.isEditingMultipleObjects)
				m_PieceOffsets.DoLayoutList();
			
			EditorGUILayout.Space();

			serializedObject.ApplyModifiedProperties();
		}

		private void OnEnable()
		{
			// Create the material for the preview if it's null.
			if(m_PreviewMat == null)
			{
				m_PreviewMat = new Material(Shader.Find("Transparent/Diffuse"));
				m_PreviewMat.color = new Color(0.5f, 0.6f, 0.5f, 1f);
			}

			m_Socket = target as Socket;

			m_Radius = serializedObject.FindProperty("m_Radius");

			// Initialize the piece list.
			m_PieceOffsets = new ReorderableList(serializedObject, serializedObject.FindProperty("m_PieceOffsets"));

			m_PieceOffsets.drawHeaderCallback = (Rect rect)=> GUI.Label(rect, "Supported Pieces");
			m_PieceOffsets.drawElementCallback = DrawPieceElement;
			m_PieceOffsets.onSelectCallback += OnPieceSelect;
		}

        private void OnDestroy() { Tools.hidden = false; }

		private void DrawPieceElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			EditorGUI.BeginChangeCheck();

			EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, 16f), m_PieceOffsets.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("m_Piece"));

			if(EditorGUI.EndChangeCheck())
				TrySelectPiece(index);
		}

        private void OnPieceSelect(ReorderableList list) { TrySelectPiece(list.index); }

		private void TrySelectPiece(int index)
		{
			if(m_PieceOffsets.count > 0)
			{
				index = Mathf.Clamp(index, 0, m_PieceOffsets.count - 1);

				m_SelectedPieceOffset = (target as Socket).PieceOffsets[index];

				m_PieceOffsets.index = index;
				m_SelectedPieceIdx = index;
			}
		}

		private void OnSceneGUI()
		{
			Tools.hidden = m_EditOffset;

            if (CannotDisplayMesh())
            {
                SceneView.RepaintAll();

                return;
            }

			// Get the scene camera and it's pixel rect.
			var sceneCamera = SceneView.GetAllSceneCameras()[0];
			Rect pixelRect = sceneCamera.pixelRect;

			if(HasValidPiece())
			{
				// Draw the piece handle.
				Vector3 pieceWorldPos = m_Socket.transform.position + m_Socket.transform.TransformVector(m_SelectedPieceOffset.PositionOffset);
	
				Handles.color = Color.grey;
				Handles.SphereCap(GUIUtility.GetControlID(FocusType.Passive), pieceWorldPos, m_Socket.transform.rotation, 0.15f);

				if(m_SelectedPieceOffset.Piece.MainMesh == null)
				{
					Debug.LogError("You have to assign the Main Mesh for this BuildingPiece: '" + m_SelectedPieceOffset.Piece.name + "'", this);
					return;
				}

                var mesh = m_SelectedPieceOffset.Piece.MainMesh.sharedMesh;

				// HACK
				try
				{
	                Vector3 position = m_Socket.transform.position + m_Socket.transform.TransformVector(m_Socket.PieceOffsets[m_SelectedPieceIdx].PositionOffset);
	                Quaternion rotation = m_Socket.transform.rotation * m_Socket.PieceOffsets[m_SelectedPieceIdx].RotationOffset;
	                Vector3 scale = m_SelectedPieceOffset.Piece.transform.lossyScale;

	                for (int m = 0; m < mesh.subMeshCount; m++)
	                    Graphics.DrawMesh(mesh, Matrix4x4.TRS(position, rotation, scale), m_PreviewMat, 0, sceneCamera, m);
				} catch {}

                SceneView.RepaintAll();

                // Draw the piece tools (move & rotate).
                DoPieceOffsetTools();
			}

			// Draw the inspector for the piece offset for the selected socket, so you can modify the position and rotation precisely.
			DoPieceOffsetInspectorWindow(pixelRect);

			//Debug.Log(m_SelectedPieceOffset);
		}

		private void DoPieceOffsetTools()
		{
			Vector3 pieceWorldPos = m_Socket.transform.position + m_Socket.transform.TransformVector(m_SelectedPieceOffset.PositionOffset);

			EditorGUI.BeginChangeCheck();
			var handlePos = Handles.PositionHandle(pieceWorldPos, m_Socket.transform.rotation * m_SelectedPieceOffset.RotationOffset);

			if(EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(target, "Socket");

				handlePos = RoundVector3(m_Socket.transform.InverseTransformPoint(handlePos));
				m_SelectedPieceOffset.PositionOffset = handlePos;
			}
		}

		private void DoPieceOffsetInspectorWindow(Rect pixelRect)
		{
			Color color = Color.white;
			GUI.backgroundColor = color;

			var windowRect = new Rect(16f, 32f, 256f, 112f);
			Rect totalRect = new Rect(windowRect.x, windowRect.y - 16f, windowRect.width, windowRect.height);

			GUI.backgroundColor = Color.white;
			GUI.Window(1, windowRect, DrawPieceOffsetInspector, "Piece Offset");

			Event e = Event.current;

			if(totalRect.Contains(e.mousePosition))
			{
				HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

				if(e.type != EventType.Layout && e.type != EventType.Repaint)
					e.Use();
			}
		}

		private void DrawPieceOffsetInspector(int windowID)
		{
			if(!HasValidPiece())
			{
				EditorGUI.HelpBox(new Rect(0f, 32f, 512f, 32f), "No valid piece selected!", MessageType.Warning);
				return;
			}
				
			var pieceOffset = m_SelectedPieceOffset;

			EditorGUI.BeginChangeCheck();

			// Position field.
			var positionOffset = EditorGUI.Vector3Field(new Rect(6f, 32f, 240f, 16f), "Position", pieceOffset.PositionOffset);

			// Rotation field.
			var rotationOffset = EditorGUI.Vector3Field(new Rect(6f, 64f, 240f, 16f), "Rotation", pieceOffset.RotationOffsetEuler);

			if(EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(target, "Socket");

				positionOffset = RoundVector3(positionOffset);
				rotationOffset = RoundVector3(rotationOffset);

				pieceOffset.PositionOffset = positionOffset;
				pieceOffset.RotationOffsetEuler = rotationOffset;
			}
		}

        private bool HasValidPiece() { return m_PieceOffsets.count != 0 && m_SelectedPieceIdx >= 0 && m_SelectedPieceOffset != null && m_SelectedPieceOffset.Piece != null; }

        private bool CannotDisplayMesh() { return (!m_EditOffset || Selection.activeGameObject == null || Selection.activeGameObject != m_Socket.gameObject); }

        private Vector3 RoundVector3(Vector3 source, int digits = 3)
		{
			source.x = (float)System.Math.Round(source.x, digits);
			if(Mathf.Approximately(source.x, 0f))
				source.x = 0f;

			source.y = (float)System.Math.Round(source.y, digits);
			if(Mathf.Approximately(source.y, 0f))
				source.y = 0f;

			source.z = (float)System.Math.Round(source.z, digits);
			if(Mathf.Approximately(source.y, 0f))
				source.y = 0f;

			return source;
		}
	}
}
