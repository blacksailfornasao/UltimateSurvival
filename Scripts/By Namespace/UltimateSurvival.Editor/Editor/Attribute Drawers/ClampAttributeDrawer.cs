using UnityEngine;
using UnityEditor;

namespace UltimateSurvival.Editor
{
	[CustomPropertyDrawer(typeof(ClampAttribute))]
	public class ClampAttributeDrawer : PropertyDrawer 
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.PropertyField(position, property);

			Vector2 clampLimits = ((ClampAttribute)attribute).ClampLimits;

			if(property.propertyType == SerializedPropertyType.Float)
				property.floatValue = Mathf.Clamp(property.floatValue, clampLimits.x, clampLimits.y);
			else if(property.propertyType == SerializedPropertyType.Integer)
				property.intValue = Mathf.Clamp(property.intValue, (int)clampLimits.x, (int)clampLimits.y);
		}
	}
}
