using UnityEngine;
using UnityEditor;
using UltimateSurvival.Debugging;

namespace UltimateSurvival.Editor
{
	[CustomPropertyDrawer(typeof(StartupItems.ItemToAdd))]
	public class ItemToAddDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Name.
			position.width = 42f;
			GUI.Label(position, "Name: ");
			position.x = position.xMax + 4f;
			position.width = 80f;
			EditorGUI.PropertyField(position, property.FindPropertyRelative("m_Name"), GUIContent.none);

			// Count.
			position.x = position.xMax + 4f;
			GUI.Label(position, "Count: ");
			position.width = 42f;
			position.x = position.xMax;
			position.width = 80f;
			property.FindPropertyRelative("m_Count").intValue = EditorGUI.IntField(position, property.FindPropertyRelative("m_Count").intValue);
		}
	}
}
