using System;
using System.Collections.Generic;
using UltimateSurvival.AI;

namespace UltimateSurvival.Editor
{
	using UnityEditor;
	using UnityEngine;

	[CustomEditor(typeof(AISettings))]
	public class AISettingsEditor : Editor 
	{
		private AISettings m_AISettings;


		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			CustomGUI.DoHorizontalLine();
			EditorGUILayout.Space();

			GUILayout.Label("Debugging", EditorStyles.boldLabel);

			if(!Application.isPlaying)
			{
				EditorGUILayout.HelpBox("Debug info will be shown at play-time!", MessageType.Info);
				return;
			}
				
			GUILayout.Label("Has Target: " + m_AISettings.Detection.HasTarget());
			GUILayout.Label("Has Target: " + m_AISettings.Detection.HasVisibleTarget());

			if(m_AISettings.Detection.HasTarget())
			{
				Rect rect = EditorGUILayout.GetControlRect();
				rect.x += 16f;

				GUI.Label(rect, "Target: " + m_AISettings.Detection.StillInRangeTargets[0].name);
			}
		}

		private void OnEnable()
		{
			m_AISettings = serializedObject.targetObject as AISettings;
		}
	}
}
