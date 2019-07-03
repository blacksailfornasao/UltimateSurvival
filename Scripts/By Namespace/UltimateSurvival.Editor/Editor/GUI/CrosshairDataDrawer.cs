using UnityEngine;
using UltimateSurvival.GUISystem;

namespace UltimateSurvival.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(CrosshairData))]
	public class CrosshairDataDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			position.height = 16f;

			property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, property.displayName);

			if(property.isExpanded)
			{
				EditorGUI.indentLevel ++;

				// Name.
				position.y = position.yMax + 4f;
				EditorGUI.PropertyField(position, property.FindPropertyRelative("m_ItemName"));

				// Hide when aiming.
				position.y = position.yMax + 4f;
				EditorGUI.PropertyField(position, property.FindPropertyRelative("m_HideWhenAiming"));

				// Normal color.
				position.y = position.yMax + 4f;
				EditorGUI.PropertyField(position, property.FindPropertyRelative("m_NormalColor"));

				// On entity color.
				position.y = position.yMax + 4f;
				EditorGUI.PropertyField(position, property.FindPropertyRelative("m_OnEntityColor"));

				// Type.
				position.y = position.yMax + 4f;
				EditorGUI.PropertyField(position, property.FindPropertyRelative("m_Type"));

				EditorGUI.indentLevel ++;

				if(((CrosshairType)property.FindPropertyRelative("m_Type").enumValueIndex) == CrosshairType.Dynamic)
				{
					// Dynamic crosshair.
					position.y = position.yMax + 4f;
					EditorGUI.PropertyField(position, property.FindPropertyRelative("m_Crosshair"));

					// Idle distance.
					position.y = position.yMax + 4f;
					EditorGUI.PropertyField(position, property.FindPropertyRelative("m_IdleDistance"));

					// Crouch distance.
					position.y = position.yMax + 4f;
					EditorGUI.PropertyField(position, property.FindPropertyRelative("m_CrouchDistance"));

					// Walk distance.
					position.y = position.yMax + 4f;
					EditorGUI.PropertyField(position, property.FindPropertyRelative("m_WalkDistance"));

					// Run distance.
					position.y = position.yMax + 4f;
					EditorGUI.PropertyField(position, property.FindPropertyRelative("m_RunDistance"));

					// Jump distance.
					position.y = position.yMax + 4f;
					EditorGUI.PropertyField(position, property.FindPropertyRelative("m_JumpDistance"));
				}
				else if(((CrosshairType)property.FindPropertyRelative("m_Type").enumValueIndex) == CrosshairType.Simple)
				{
					// Image.
					position.y = position.yMax + 4f;
					EditorGUI.PropertyField(position, property.FindPropertyRelative("m_Image"));

					// Sprite.
					position.y = position.yMax + 4f;
					EditorGUI.PropertyField(position, property.FindPropertyRelative("m_Sprite"));

					// Size.
					position.y = position.yMax + 4f;
					EditorGUI.PropertyField(position, property.FindPropertyRelative("m_Size"));
				}

				EditorGUI.indentLevel --;

				EditorGUI.indentLevel --;
			}
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			float height = 16f;
			if(property.isExpanded)
			{
				height += 20f * 6;

				if(((CrosshairType)property.FindPropertyRelative("m_Type").enumValueIndex) == CrosshairType.Dynamic)
					height += 20f * 6;
				else if(((CrosshairType)property.FindPropertyRelative("m_Type").enumValueIndex) == CrosshairType.Simple)
					height += 20f * 3;
			}

			return height;
		}
	}
}
