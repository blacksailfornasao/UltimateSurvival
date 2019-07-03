using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UltimateSurvival.InputSystem;
using UnityEditorInternal;

namespace UltimateSurvival.Editor
{
	public class InputManagerWindow : EditorWindow
	{
		private static bool m_ButtonListFoldout = true;

		private static bool m_AxesListFoldout = true;

		private static Vector2 m_ScrollPosition;

		//private static List<bool> m_AxesFoldouts = new List<bool>();

		private SerializedObject m_SerializedInput;
		private SerializedObject m_SerializedInputData;

        private ReorderableList m_ButtonReorderables;
        private ReorderableList m_AxesReorderables;

		private InputManager m_Input;


		[MenuItem("Tools/Ultimate Survival/Input Manager", false)]
	    public static void OpenInputManager()
	    {
	        EditorWindow window = GetWindow<InputManagerWindow>(true,"Input Manager");

	        window.maxSize = new Vector2(1000, 750);
	        window.minSize = window.maxSize;
	    }

        private void OnEnable()
        {
            Undo.undoRedoPerformed += Repaint;

            LookForData();

            SerializedProperty buttonsProperty = m_SerializedInputData.FindProperty("m_Buttons");

            m_ButtonReorderables = new ReorderableList(m_SerializedInputData, buttonsProperty);

            m_ButtonReorderables.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight), buttonsProperty.GetArrayElementAtIndex(index).FindPropertyRelative("m_ButtonName"));

                EditorGUI.PropertyField(new Rect(rect.x + 250, rect.y, 200, EditorGUIUtility.singleLineHeight), buttonsProperty.GetArrayElementAtIndex(index).FindPropertyRelative("m_Key"));
            };

            SerializedProperty axesProperty = m_SerializedInputData.FindProperty("m_Axes");

            m_AxesReorderables = new ReorderableList(m_SerializedInputData, axesProperty);

            m_AxesReorderables.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight), axesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("m_AxisName"));

                ET.StandaloneAxisType axisType = (ET.StandaloneAxisType)axesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("m_AxisType").enumValueIndex;

                EditorGUI.PropertyField(new Rect(rect.x + 250, rect.y, 200, EditorGUIUtility.singleLineHeight), axesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("m_AxisType"));

                if (axisType == ET.StandaloneAxisType.Custom)
                {
                    EditorGUI.PropertyField(new Rect(rect.x + 500, rect.y, 200, EditorGUIUtility.singleLineHeight), axesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("m_Normalize"));

                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight + 5), 200, EditorGUIUtility.singleLineHeight), axesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("m_PositiveKey"));
                    EditorGUI.PropertyField(new Rect(rect.x + 250, rect.y + (EditorGUIUtility.singleLineHeight + 5), 200, EditorGUIUtility.singleLineHeight), axesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("m_NegativeKey"));
                }
                else if (axisType == ET.StandaloneAxisType.Unity)
                    EditorGUI.PropertyField(new Rect(rect.x + 500, rect.y, 200, EditorGUIUtility.singleLineHeight), axesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("m_UnityAxisName"));
                else
                {
                    EditorGUI.PropertyField(new Rect(rect.x + 500, rect.y, 200, EditorGUIUtility.singleLineHeight), axesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("m_UnityAxisName"));
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight + 5), 200, EditorGUIUtility.singleLineHeight), axesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("m_Normalize"));

                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + ((EditorGUIUtility.singleLineHeight * 2) + 10), 200, EditorGUIUtility.singleLineHeight), axesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("m_PositiveKey"));
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + ((EditorGUIUtility.singleLineHeight * 2) + 10), 200, EditorGUIUtility.singleLineHeight), axesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("m_NegativeKey"));
                }
            };

            m_AxesReorderables.elementHeight = 40f;
        }

        private void OnGUI()
        {
            if (!m_Input)
            {
                EditorGUILayout.HelpBox("No Input Manager On The Scene", MessageType.Warning);
                return;
            }

            BuildGUI();
        }

        private void LookForData()
        {
            if (!m_Input)
                m_Input = FindObjectOfType<InputManager>();

			if(m_SerializedInput == null)
           		m_SerializedInput = new SerializedObject(m_Input);
			if(m_SerializedInputData == null)
           		m_SerializedInputData = new SerializedObject(m_Input.InputData);
        }

        private void BuildGUI()
        {
            LookForData();

            if (m_Input)
            {
                if (m_Input.InputData == null)
                    EditorGUILayout.HelpBox("No InputData found on the InputManager!", MessageType.Warning);
                else
                    DrawInputManagerWindow();
            }
        }

	    private void DrawInputManagerWindow()
	    {
	      	m_SerializedInput.Update();
	        m_SerializedInputData.Update();

	        EditorGUIUtility.labelWidth = 100;

	        m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);

	        EditorGUILayout.BeginHorizontal();
	        GUILayout.Space(20);

	        EditorGUILayout.BeginVertical();
	        GUILayout.Space(20);

	        EditorGUILayout.LabelField("Input Manager", MainStyle());

	        EditorGUILayout.EndVertical();
	        EditorGUILayout.EndHorizontal();

	        DrawLists();

	        EditorGUILayout.BeginVertical();
	        GUILayout.Space(20);

	        EditorGUILayout.BeginHorizontal();

	        if (GUILayout.Button("Delete All"))
	            GameController.InputManager.ClearAll();

	        if (GUILayout.Button("Reset All Defaults"))
	        {
				GameController.InputManager.ClearAll();

				GameController.InputManager.SetupDefaults((ET.InputType)m_SerializedInputData.FindProperty("m_InputType").enumValueIndex, ET.InputMode.Buttons);

				GameController.InputManager.SetupDefaults((ET.InputType)m_SerializedInputData.FindProperty("m_InputType").enumValueIndex, ET.InputMode.Axes);
	        }

	        EditorGUILayout.EndHorizontal();
	        EditorGUILayout.EndVertical();

	        EditorGUILayout.EndScrollView();

	        m_SerializedInput.ApplyModifiedProperties();

	        m_SerializedInputData.ApplyModifiedProperties();
	    }

	    private void DrawLists()
	    {
	        EditorGUILayout.BeginHorizontal();
	        GUILayout.Space(40);

	        //SerializedProperty inputTypeProperty = _serializedInput.FindProperty("_inputData").FindPropertyRelative("_inputType").enumValueIndex;  //THIS ACTIVATES MOBILE USE.
	        //EditorGUILayout.PropertyField(inputTypeProperty);

	        EditorGUILayout.EndHorizontal();

	        EditorGUILayout.BeginHorizontal();
	        GUILayout.Space(40);

	        EditorGUILayout.BeginVertical();

	        m_ButtonListFoldout = EditorGUILayout.Foldout(m_ButtonListFoldout, "Button List");
            if (m_ButtonListFoldout)
                DrawReorderableButtonList();

	        m_AxesListFoldout = EditorGUILayout.Foldout(m_AxesListFoldout, "Axes List");
            if (m_AxesListFoldout)
                DrawReorderableAxesList();

	        EditorGUILayout.EndVertical();
	        EditorGUILayout.EndHorizontal();
	    }

        private void DrawReorderableButtonList()
        {
            if (m_Input.InputData.Buttons.Count == 0 || m_ButtonReorderables == null)
                return;

            m_ButtonReorderables.DoLayoutList();
        }

        private void DrawReorderableAxesList()
        {
            if (m_Input.InputData.Axes.Count == 0 || m_AxesReorderables == null)
                return;

            m_AxesReorderables.DoLayoutList();
        }

	    private GUIStyle MainStyle()
	    {
	        GUIStyle style = new GUIStyle();
	        style.fontSize = 16;

	        return style;
	    }
	}
}