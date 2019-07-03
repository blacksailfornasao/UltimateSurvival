using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UltimateSurvival.GUISystem;

namespace UltimateSurvival.Editor
{
	[CustomPropertyDrawer(typeof(DurabilityBar))]
	public class DurabilityBarDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			position.height = 16f;
			property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, "Durability Bar");

			if(property.isExpanded)
			{
				float spacing = 4f;
	
				EditorGUI.indentLevel ++;

				position.y = position.yMax + spacing;
				EditorGUI.PropertyField(position, property.FindPropertyRelative("m_Background"));

				position.y = position.yMax + spacing;
				EditorGUI.PropertyField(position, property.FindPropertyRelative("m_Bar"));

				position.y = position.yMax + spacing;
				EditorGUI.PropertyField(position, property.FindPropertyRelative("m_ColorGradient"));

				position.y = position.yMax + spacing;
				EditorGUI.PropertyField(position, property.FindPropertyRelative("m_Durability"));

				EditorGUI.indentLevel --;

				if(!Application.isPlaying)
				{
					var durability = property.FindPropertyRelative("m_Durability").floatValue;
					var durabilityBar = (DurabilityBar)property.serializedObject.targetObject.GetType().GetField("m_DurabilityBar", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(property.serializedObject.targetObject);

					Gradient gradient = null;
		
					if(durabilityBar != null)
						gradient = (Gradient)durabilityBar.GetType().GetField("m_ColorGradient", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(durabilityBar);

					var bar = (Image)property.FindPropertyRelative("m_Bar").objectReferenceValue;

					if(gradient != null && bar != null)
					{
						bar.fillAmount = durability;
						bar.color = gradient.Evaluate(durability);
					}
				}
			}
		}
			
		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			return 16f + (property.isExpanded ? 16f * 5f : 0f);
		}
	}
}
