using UnityEngine;
using UnityEditor;

namespace UltimateSurvival.Editor
{
	/*[CustomPropertyDrawer(typeof(ItemProperty.Value))]
	public class PropertyValueDrawer : PropertyDrawer
	{
		private ItemProperty.Type m_Type;


		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			base.OnGUI(position, property, label);
			return;
			GUIStyle boldStyle = new GUIStyle() { fontStyle = FontStyle.Bold };

			var name = property.FindPropertyRelative("m_Name").stringValue;
			m_Type = (ItemProperty.Type)property.FindPropertyRelative("m_Type").enumValueIndex;

			// Type (Test)
			//position.height = 16f;
			//EditorGUI.PropertyField(position, property.FindPropertyRelative("m_Type"));

			// Name
			//position.y += 16f;
			position.height = 16f;
			EditorGUI.LabelField(position, "Definition: ");

			position.y += 15f;

			if(m_Type == ItemProperty.Type.Bool)
				DrawBool(position, property.FindPropertyRelative("m_Bool"), label);
			else if(m_Type == ItemProperty.Type.Int)
				DrawInt(position, property.FindPropertyRelative("m_Int"), label);
			else if(m_Type == ItemProperty.Type.IntRange)
				DrawIntRange(position, property.FindPropertyRelative("m_IntRange"), label);
			else if(m_Type == ItemProperty.Type.RandomInt)
				DrawRandomInt(position, property.FindPropertyRelative("m_RandomInt"), label);
			else if(m_Type == ItemProperty.Type.Float)
				DrawFloat(position, property.FindPropertyRelative("m_Float"), label);
			else if(m_Type == ItemProperty.Type.FloatRange)
				DrawFloatRange(position, property.FindPropertyRelative("m_FloatRange"), label);
			else if(m_Type == ItemProperty.Type.RandomFloat)
				DrawRandomFloat(position, property.FindPropertyRelative("m_RandomFloat"), label);
			else if(m_Type == ItemProperty.Type.String)
				DrawString(position, property.FindPropertyRelative("m_String"), label);
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			float height = 48f;

			return height;
		}

		private void DrawBool(Rect position, SerializedProperty property, GUIContent label)
		{
			property.boolValue = EditorGUI.Toggle(position, property.boolValue);
		}

		private void DrawInt(Rect position, SerializedProperty property, GUIContent label)
		{
			property.intValue = EditorGUI.IntField(position, property.intValue);
		}

		private void DrawIntRange(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty min = property.FindPropertyRelative("m_Min");
			SerializedProperty value = property.FindPropertyRelative("m_Value");
			SerializedProperty max = property.FindPropertyRelative("m_Max");

			float labelWidth = 64f;
			float fieldWidth = 80f;

			// Value label
			position.width = labelWidth;
			EditorGUI.PrefixLabel(position, new GUIContent("Val:"));

			// Value field
			position.width = fieldWidth;
			position.x = position.xMax - labelWidth * 0.75f;
			value.intValue = EditorGUI.IntField(position, Mathf.Clamp(value.intValue, min.intValue, max.intValue));

			// Min label
			position.width = labelWidth;
			position.x = position.xMax - fieldWidth * 0.25f;
			EditorGUI.PrefixLabel(position, new GUIContent("Min:"));

			// Min field
			position.width = fieldWidth;
			position.x = position.xMax - labelWidth * 0.75f;
			min.intValue = EditorGUI.IntField(position, min.intValue);

			// Max label
			position.width = labelWidth;
			position.x = position.xMax - fieldWidth * 0.25f;
			EditorGUI.PrefixLabel(position, new GUIContent("Max:"));

			// Max field
			position.width = fieldWidth;
			position.x = position.xMax - labelWidth * 0.75f;
			max.intValue = EditorGUI.IntField(position, max.intValue);
		}

		private void DrawRandomInt(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty min = property.FindPropertyRelative("m_Min");
			SerializedProperty max = property.FindPropertyRelative("m_Max");

			float labelWidth = 64f;
			float fieldWidth = 80f;

			// Min label
			position.width = labelWidth;
			EditorGUI.PrefixLabel(position, new GUIContent("Min:"));

			// Min field
			position.width = fieldWidth;
			position.x = position.xMax - labelWidth * 0.75f;
			min.intValue = EditorGUI.IntField(position, min.intValue);

			// Max label
			position.width = labelWidth;
			position.x = position.xMax - fieldWidth * 0.25f;
			EditorGUI.PrefixLabel(position, new GUIContent("Max:"));

			// Max field
			position.width = fieldWidth;
			position.x = position.xMax - labelWidth * 0.75f;
			max.intValue = EditorGUI.IntField(position, max.intValue);
		}

		private void DrawFloat(Rect position, SerializedProperty property, GUIContent label)
		{
			property.floatValue = EditorGUI.FloatField(position, property.floatValue);
		}

		private void DrawFloatRange(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty min = property.FindPropertyRelative("m_Min");
			SerializedProperty value = property.FindPropertyRelative("m_Value");
			SerializedProperty max = property.FindPropertyRelative("m_Max");

			float labelWidth = 64f;
			float fieldWidth = 80f;

			// Value label
			position.width = labelWidth;
			EditorGUI.PrefixLabel(position, new GUIContent("Val:"));

			// Value field
			position.width = fieldWidth;
			position.x = position.xMax - labelWidth * 0.75f;
			value.floatValue = EditorGUI.FloatField(position, Mathf.Clamp(value.floatValue, min.floatValue, max.floatValue));

			// Min label
			position.width = labelWidth;
			position.x = position.xMax - fieldWidth * 0.25f;
			EditorGUI.PrefixLabel(position, new GUIContent("Min:"));

			// Min field
			position.width = fieldWidth;
			position.x = position.xMax - labelWidth * 0.75f;
			value.floatValue = EditorGUI.FloatField(position, min.floatValue);

			// Max label
			position.width = labelWidth;
			position.x = position.xMax - fieldWidth * 0.25f;
			EditorGUI.PrefixLabel(position, new GUIContent("Max:"));

			// Max field
			position.width = fieldWidth;
			position.x = position.xMax - labelWidth * 0.75f;
			max.floatValue = EditorGUI.FloatField(position, max.floatValue);
		}

		private void DrawRandomFloat(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty min = property.FindPropertyRelative("m_Min");
			SerializedProperty max = property.FindPropertyRelative("m_Max");

			float labelWidth = 64f;
			float fieldWidth = 80f;

			// Min label
			position.width = labelWidth;
			EditorGUI.PrefixLabel(position, new GUIContent("Min:"));
	
			// Min field
			position.width = fieldWidth;
			position.x = position.xMax - labelWidth * 0.75f;
			min.floatValue = EditorGUI.FloatField(position, min.floatValue);

			// Max label
			position.width = labelWidth;
			position.x = position.xMax - fieldWidth * 0.25f;
			EditorGUI.PrefixLabel(position, new GUIContent("Max:"));

			// Max field
			position.width = fieldWidth;
			position.x = position.xMax - labelWidth * 0.75f;
			max.floatValue = EditorGUI.FloatField(position, max.floatValue);
		}

		private void DrawString(Rect position, SerializedProperty property, GUIContent label)
		{
			property.stringValue = EditorGUI.TextField(position, property.stringValue);
		}
	}*/
}
