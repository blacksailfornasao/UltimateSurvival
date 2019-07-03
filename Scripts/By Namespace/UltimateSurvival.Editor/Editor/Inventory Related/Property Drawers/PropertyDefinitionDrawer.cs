using UnityEngine;
using UnityEditor;

namespace UltimateSurvival.Editor
{
	[CustomPropertyDrawer(typeof(ItemProperty.Definition))]
	public class PropertyDefinitionDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			position.width /= 2f;

			// Name.
			EditorGUI.PropertyField(position, property.FindPropertyRelative("m_Name"), GUIContent.none);
		

			// Type.
			position.x += 144f;
			EditorGUI.PropertyField(position, property.FindPropertyRelative("m_Type"), GUIContent.none);
		}
	}
}
