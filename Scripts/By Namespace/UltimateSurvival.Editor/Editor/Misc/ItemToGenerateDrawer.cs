using UnityEngine;

namespace UltimateSurvival.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(ItemToGenerate))]
	public class ItemToGenerateDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var random = property.FindPropertyRelative("m_Random");
			var customName = property.FindPropertyRelative("m_CustomName");
			var stackSize = property.FindPropertyRelative("m_StackSize");

			//position.y += 4f;
			position.height = 16f;
			EditorGUI.PropertyField(position, random);

			position.y = position.yMax;

			GUI.enabled = !random.boolValue;

			if(!random.boolValue)
			{
				EditorGUI.PropertyField(position, customName);

				position.y = position.yMax;
				EditorGUI.PropertyField(position, stackSize);
			}
			else
			{
				EditorGUI.LabelField(position, "Custom Name: ???");

				position.y = position.yMax;
				EditorGUI.LabelField(position, "Stack Size: ???");
			}

			GUI.enabled = true;

			position.y = position.yMax + 4f;
			position.height = 1f;
			CustomGUI.DoHorizontalLine(position);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return 54f;
		}
	}
}
