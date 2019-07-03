using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UltimateSurvival.GUISystem;

namespace UltimateSurvival.Editor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(PlayerVitals))]
	public class PlayerStatusEditor : UnityEditor.Editor 
	{
		public override void OnInspectorGUI()
		{
			if(Application.isPlaying)
			{
				EditorGUILayout.Space();

				float healthValue = (serializedObject.targetObject as PlayerVitals).Player.Health.Get();
				float staminaValue = (serializedObject.targetObject as PlayerVitals).Player.Stamina.Get();
				float thirstValue = (serializedObject.targetObject as PlayerVitals).Player.Thirst.Get();
				float hungerValue = (serializedObject.targetObject as PlayerVitals).Player.Hunger.Get();

				string info = string.Format(
					"Health: {0}\n" +
					"Stamina: {1}\n" +
					"Thirst: {2}\n" +
					"Hunger: {3}",
					healthValue, staminaValue, thirstValue, hungerValue
				);

				EditorGUILayout.HelpBox(info, MessageType.Info);

				Repaint();
			}

			base.OnInspectorGUI();
		}
	}
}
