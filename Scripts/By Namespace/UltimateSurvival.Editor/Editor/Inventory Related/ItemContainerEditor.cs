using UnityEngine;
using UnityEditor;
using UltimateSurvival.GUISystem;

namespace UltimateSurvival.Editor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(ItemContainer))]
	public class ItemContainerEditor : UnityEditor.Editor 
	{
		private SerializedProperty _Name;
		private SerializedProperty m_Window;
		private SerializedProperty m_SlotTemplate;
		private SerializedProperty m_SlotsParent;
		private SerializedProperty m_PreviewSize;
		private SerializedProperty m_RequiredCategories;
		private SerializedProperty m_RequiredProperties;


		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.Space();

			if(!m_SlotTemplate.objectReferenceValue || !m_SlotsParent.objectReferenceValue)
				EditorGUILayout.HelpBox("Make sure a template and a parent are provided, otherwise the collection can't be created at runtime or be previewed!", MessageType.Error);

			EditorGUILayout.PropertyField(_Name);
			EditorGUILayout.PropertyField(m_Window);

			EditorGUILayout.PropertyField(m_SlotTemplate);
			EditorGUILayout.PropertyField(m_SlotsParent);

			EditorGUILayout.PropertyField(m_PreviewSize);

			EditorGUILayout.PropertyField(m_RequiredCategories);
			EditorGUILayout.PropertyField(m_RequiredProperties);

			if(!Application.isPlaying)
			{
				EditorGUILayout.HelpBox("This button will apply the settings above to all the child slots.", MessageType.Info);

				if(!serializedObject.isEditingMultipleObjects && GUILayout.Button("Apply"))
					(serializedObject.targetObject as ItemContainer).ApplyAll();
			}

			serializedObject.ApplyModifiedProperties();
		}

		private void OnEnable()
		{
			_Name = serializedObject.FindProperty("_Name");
			m_Window = serializedObject.FindProperty("m_Window");

			m_SlotTemplate = serializedObject.FindProperty("m_SlotTemplate");
			m_SlotsParent = serializedObject.FindProperty("m_SlotsParent");
			m_PreviewSize = serializedObject.FindProperty("m_PreviewSize");
			m_RequiredCategories = serializedObject.FindProperty("m_RequiredCategories");
			m_RequiredProperties = serializedObject.FindProperty("m_RequiredProperties");
		}
	}
}
