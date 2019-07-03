using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UltimateSurvival.GUISystem;

namespace UltimateSurvival.Editor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(EntityVitals))]
	public class EntityStatusEditor : UnityEditor.Editor 
	{
		public override void OnInspectorGUI()
		{
			if(Application.isPlaying)
			{
				EditorGUILayout.Space();

				float healthValue = (serializedObject.targetObject as EntityVitals).Entity.Health.Get();
				EditorGUILayout.HelpBox(string.Format("Health: {0}", healthValue), MessageType.Info);
				Repaint();
			}

			base.OnInspectorGUI();
		}
	}
}
