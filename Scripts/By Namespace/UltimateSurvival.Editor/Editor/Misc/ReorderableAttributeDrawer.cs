using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace UltimateSurvival.Editor
{
	[CustomPropertyDrawer(typeof(ReorderableAttribute))]
	public class ReorderableAttributeDrawer : PropertyDrawer
	{
		private ReorderableList m_List;
		private string m_PropertyName;


		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			if(m_List == null)
				InitializeList(property);

			Rect rect = new Rect(position);
			m_List.DoList(rect);
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			if(m_List == null)
				InitializeList(property);
			
			return m_List.GetHeight();
		}

		private void DrawHeader(Rect rect)
		{
			GUI.Label(rect, m_PropertyName);
		}

		private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			if(m_List.count <= index)
				return;

			EditorGUI.PropertyField(rect, m_List.serializedProperty.GetArrayElementAtIndex(index), GUIContent.none);
			ItemManagementUtility.DoListElementBehaviours(m_List, m_List.index, isFocused);
		}

		private void InitializeList(SerializedProperty source)
		{
			m_List = new ReorderableList(source.serializedObject, source.FindPropertyRelative("m_List"), true, true, true, true);
			m_List.elementHeight = 16f;
			m_List.drawHeaderCallback += DrawHeader;
			m_List.drawElementCallback += DrawElement;

			m_PropertyName = source.displayName;
		}
	}
}
