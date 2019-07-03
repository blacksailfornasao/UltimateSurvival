using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UltimateSurvival;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class EndNameEdit : EndNameEditAction
{
	#region implemented abstract members of EndNameEditAction
	public override void Action (int instanceId, string pathName, string resourceFile)
	{
		AssetDatabase.CreateAsset(EditorUtility.InstanceIDToObject(instanceId), AssetDatabase.GenerateUniqueAssetPath(pathName));
	}

	#endregion
}

/// <summary>
/// Scriptable object window.
/// </summary>
public class FileCreatorWindow : EditorWindow
{
    private string[] m_ScriptableHideList = new string[] { "EndNameEdit", "FileCreatorWindow", "ScriptableSingleton`1", "Action", "Goal", "ActionTemplate", "GoalTemplate" };

	private int m_SelectedIndex;

    private ET.FileCreatorMode m_Mode;

    private bool m_CloseAfterCreation = false;
	
	private Type[] m_ScriptableObjectTypes;
    private Type[] m_TemplateObjectTypes;
	
	public Type[] ScriptableObjectTypes
	{ 
		get { return m_ScriptableObjectTypes; }
        set { m_ScriptableObjectTypes = value; }
	}

    public Type[] TemplateObjectTypes 
    {
        get { return m_TemplateObjectTypes; }
        set { m_TemplateObjectTypes = value; }
    }
	
	public void OnGUI()
	{
        m_Mode = (ET.FileCreatorMode)EditorGUILayout.EnumPopup("File Type", m_Mode);

        Type[] selectedTypes = (m_Mode == ET.FileCreatorMode.ScriptableObject) ? m_ScriptableObjectTypes: m_TemplateObjectTypes;
        string[] fileNames = (selectedTypes == m_ScriptableObjectTypes) ? GetFileNamesFiltered(m_ScriptableObjectTypes) : GetFileNames(m_TemplateObjectTypes);

        FilterType(ref m_ScriptableObjectTypes);

        GUILayout.Label((m_Mode == ET.FileCreatorMode.ScriptableObject) ? "Scriptable Object Type" : "Script File Type");
		m_SelectedIndex = EditorGUILayout.Popup(m_SelectedIndex, fileNames);

		if (GUILayout.Button("Create " + m_Mode.ToString()))
		{
            if (m_Mode == ET.FileCreatorMode.ScriptableObject)
                CreateScriptableOfType(m_ScriptableObjectTypes[m_SelectedIndex], fileNames[m_SelectedIndex]);
            else if (m_Mode == ET.FileCreatorMode.ScriptFile)
                CreateScriptFileOfType(m_TemplateObjectTypes[m_SelectedIndex]);
            else if (m_Mode == ET.FileCreatorMode.Both)
            {
                CreateScriptableOfType(m_TemplateObjectTypes[m_SelectedIndex], fileNames[m_SelectedIndex]);

                CreateScriptFileOfType(m_TemplateObjectTypes[m_SelectedIndex]);
            }

            if (m_CloseAfterCreation)
                Close();
		}
	}

    private void FilterType(ref Type[] typeS)
    {
        List<Type> typeSNew = new List<Type>();

        for (int i = 0; i < typeS.Length; i++)
        {
            bool skip = false;

            for (int x = 0; x < m_ScriptableHideList.Length; x++)
            {
                if (m_ScriptableHideList[x] == typeS[i].Name)
                    skip = true;
            }

            if (!skip)
                typeSNew.Add(typeS[i]);
        }

        typeS = typeSNew.ToArray();
    }

    private string[] GetFileNames(Type[] types)
    {
        List<string> names = new List<string>();

        for (int i = 0; i < types.Length; i++)
            names.Add(types[i].Name);

        return names.ToArray();
    }

    private string[] GetFileNamesFiltered(Type[] types)
    {
        List<string> names = new List<string>();
        for (int i = 0; i < types.Length; i++)
        {
            bool skip = false;

            for (int x = 0; x < m_ScriptableHideList.Length; x++)
            {
                if (m_ScriptableHideList[x] == types[i].Name)
                    skip = true;
            }

            if (!skip)
                names.Add(types[i].Name);
        }

        return names.ToArray();
    }

    public static void CreateScriptableOfType(Type type, string name)
    {
        ScriptableObject asset = ScriptableObject.CreateInstance(type);
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
            asset.GetInstanceID(),
            ScriptableObject.CreateInstance<EndNameEdit>(),
            string.Format("{0}.asset", name),
            AssetPreview.GetMiniThumbnail(asset),
            null);
    }

    private void CreateScriptFileOfType(Type type)
    {
        string path = GetSelectedPath() + "/" + type.Name + ".cs";
        
        string[] s = AssetDatabase.FindAssets(type.Name);
        string templatePath = AssetDatabase.GUIDToAssetPath(s[0]);
        UnityEngine.Object[] scrpt = AssetDatabase.LoadAllAssetsAtPath(templatePath);
        object template = null;

        for (int i = 0; i < scrpt.Length; i++)
        {
            if (scrpt[i].name == type.Name)
                template = scrpt[i].ToString();
        }

        StreamWriter sw = new StreamWriter(path);
        sw.Write(template.ToString());
        sw.Close();

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    private string GetSelectedPath()
    {
        string path = "Assets";

        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }
}