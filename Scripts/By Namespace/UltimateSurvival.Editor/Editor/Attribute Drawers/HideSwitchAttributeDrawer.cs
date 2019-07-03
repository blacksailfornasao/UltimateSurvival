using UnityEngine;
using UnityEditor;

namespace UltimateSurvival.Editor
{
    [CustomPropertyDrawer(typeof(HideSwitchAttribute))]
    public class HideSwitchAttributeDrawer : PropertyDrawer
    {
        private Rect m_Position;
        private SerializedProperty m_Property;
        private float m_IndentAmount;
        private bool m_Showing;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            m_Position = position;
            m_Property = property;

            SerializedProperty basedOnValue = m_Property.serializedObject.FindProperty(((HideSwitchAttribute)attribute).m_BasedOnValue);

            bool showOnBool = ((HideSwitchAttribute)attribute).m_ShowOnBool;
            string showOnString = ((HideSwitchAttribute)attribute).m_ShowOnString;
            int showOnInt = ((HideSwitchAttribute)attribute).m_ShowOnInt;
            float showOnFloat = ((HideSwitchAttribute)attribute).m_ShowOnFloat;
            Vector3 showOnVector3 = ((HideSwitchAttribute)attribute).m_ShowOnVector3;

            m_IndentAmount = ((HideSwitchAttribute)attribute).m_IndentAmount;

            if (basedOnValue.boolValue == showOnBool)
                Draw();
            else if (basedOnValue.propertyType == SerializedPropertyType.Integer && basedOnValue.intValue == showOnInt)
                Draw();
            else if (basedOnValue.propertyType == SerializedPropertyType.String && basedOnValue.stringValue == showOnString)
                Draw();
            else if (basedOnValue.propertyType == SerializedPropertyType.Vector3 && basedOnValue.vector3Value == showOnVector3)
                Draw();
            else if (basedOnValue.propertyType == SerializedPropertyType.Float && basedOnValue.floatValue == showOnFloat)
                Draw();
            else
                m_Showing = false;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (m_Showing == false)
                return 0;
            else
                return base.GetPropertyHeight(property, label);
        }

        private void Draw()
        {
            m_Position.x += m_IndentAmount;

            if (m_Property.propertyType == SerializedPropertyType.Boolean)
                m_Property.boolValue = EditorGUI.Toggle(m_Position, m_Property.displayName, m_Property.boolValue);
            else if (m_Property.propertyType == SerializedPropertyType.LayerMask)
                m_Property.intValue = EditorGUI.MaskField(m_Position, m_Property.displayName, m_Property.intValue, UnityEditorInternal.InternalEditorUtility.layers);
            else if (m_Property.propertyType == SerializedPropertyType.Integer)
                m_Property.intValue = EditorGUI.IntField(m_Position, m_Property.displayName, m_Property.intValue);
            else if (m_Property.propertyType == SerializedPropertyType.Float)
                m_Property.floatValue = EditorGUI.FloatField(m_Position, m_Property.displayName, m_Property.floatValue);
            else if (m_Property.propertyType == SerializedPropertyType.String)
                m_Property.stringValue = EditorGUI.TextField(m_Position, m_Property.displayName, m_Property.stringValue);
            else if (m_Property.propertyType == SerializedPropertyType.Vector3)
                m_Property.vector3Value = EditorGUI.Vector3Field(m_Position, m_Property.displayName, m_Property.vector3Value);

            m_Showing = true;
        }
    }
}