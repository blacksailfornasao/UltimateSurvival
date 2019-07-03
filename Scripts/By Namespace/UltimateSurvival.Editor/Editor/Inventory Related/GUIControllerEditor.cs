using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UltimateSurvival.GUISystem;

namespace UltimateSurvival.Editor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(GUIController))]
	public class GUIControllerEditor : UnityEditor.Editor 
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if(!Application.isPlaying)
			{
				EditorGUILayout.Space();
				EditorGUILayout.HelpBox("Will assign the font to all the Text components.", MessageType.Info);
				if(GUILayout.Button("Assign Font"))
				{
					var controller = serializedObject.targetObject as GUIController;
					foreach(var text in controller.GetComponentsInChildren<Text>())
						text.font = controller.Font;
				}

				EditorGUILayout.Space();
				EditorGUILayout.HelpBox("Equivalent with pressing 'Apply' for all collections.", MessageType.Info);

				if(GUILayout.Button("Apply For All Collections"))
				{
					var controller = serializedObject.targetObject as GUIController;
					controller.ApplyForAllCollections();
				}
			}
		}
	}
}
