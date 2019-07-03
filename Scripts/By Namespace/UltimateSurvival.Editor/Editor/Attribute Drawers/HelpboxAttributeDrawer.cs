using UnityEngine;
using UnityEditor;

namespace UltimateSurvival.Editor
{
	/*[CustomPropertyDrawer(typeof(HelpboxAttribute))]
	public class HelpboxAttributeDrawer : PropertyDrawer 
	{
		private float m_PropertyHeight = 32f;

		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.HelpBox(position, ((HelpboxAttribute)attribute).Message, MessageType.Info);

			position.y = position.yMax;
			position.height = 16f;
			EditorGUI.PropertyField(position, property);
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			return m_PropertyHeight;
		}
	}*/
}
