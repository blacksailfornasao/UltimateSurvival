using System;
using System.Collections.Generic;
using UltimateSurvival.AI;

namespace UltimateSurvival.Editor
{
	using UnityEditor;
	using UnityEngine;

	[CustomEditor(typeof(AIBrain))]
	public class AIBrainEditor : Editor 
	{
		private AIBrain m_AIBrain;


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
				
			string goalName = "None";
			if(m_AIBrain.CurrentGoal != null)
			{
				goalName = m_AIBrain.CurrentGoal.name;
				goalName = goalName.Remove(goalName.IndexOf("(Clone)"));
			}

			GUILayout.Label("Current Goal: " + goalName);

			GUILayout.Label("Action Queue:");

			if(m_AIBrain.ActionQueue == null)
				return;

			for(int i = 0;i < m_AIBrain.ActionQueue.Count;i ++)
			{
				var actionName = (i + 1) + "." + m_AIBrain.ActionQueue.ToArray()[i].name;
				actionName = actionName.Remove(actionName.IndexOf("(Clone)"));

				Rect rect = EditorGUILayout.GetControlRect();
				rect.x += 16f;

				GUI.Label(rect, actionName);
			}
		}

		private void OnEnable()
		{
			m_AIBrain = serializedObject.targetObject as AIBrain;
		}
	}
}
