using System.Collections.Generic;

namespace UltimateSurvival.Editor
{
	using UnityEditor;
	using UnityEngine;

	public class SelectableHandle
	{
		/// <summary>
		/// The id of the most recently processed handle.
		/// </summary>
		public static int LastHandleID;

		// Internal state for DragHandle()
		private static int m_HandleHash = "SelectableHandle".GetHashCode();


		public static bool DoHandle(Vector3 position, Quaternion rotation, float handleSize, Handles.DrawCapFunction capFunc, Color color)
		{
			bool mouseDown = false;

			int id = GUIUtility.GetControlID(m_HandleHash, FocusType.Passive);
			LastHandleID = id;

			Vector3 screenPosition = Handles.matrix.MultiplyPoint(position);
			Matrix4x4 cachedMatrix = Handles.matrix;

			var eventType = Event.current.GetTypeForControl(id);
			if(eventType == EventType.MouseDown)
			{
				if (HandleUtility.nearestControl == id && Event.current.button == 0)
				{
					Event.current.Use();
					mouseDown = true;
				}
			}
			else if(eventType == EventType.Repaint)
			{
				Color previousColor = Handles.color;
				Handles.color = color;

				Handles.matrix = Matrix4x4.identity;
				capFunc(id, screenPosition, rotation, handleSize);
				Handles.matrix = cachedMatrix;

				Handles.color = previousColor;
			}
			else if(eventType == EventType.Layout)
			{
				Handles.matrix = Matrix4x4.identity;
				HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(screenPosition, handleSize));
				Handles.matrix = cachedMatrix;
			}

			return mouseDown;
		}
	}
}