using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Helper class for instantiating ScriptableObjects or Templates.
/// </summary>
public class FileFactory
{
	[MenuItem("Assets/File Creator")]
	public static void Create()
	{
		var assembly = GetAssembly ();

		// Get all classes derived from ScriptableObject
		var allScriptableObjects = (from t in assembly.GetTypes()
									where t.IsSubclassOf(typeof(ScriptableObject))
		                            select t).ToArray();

        List<Type> allTemplateObjects = new List<Type>();

        for (int i = 0; i < assembly.GetTypes().Length; i++)
        {
            Type type = assembly.GetTypes()[i];
            Type[] intrfs = type.GetInterfaces();

            for (int x = 0; x < intrfs.Length; x++)
            {
                if (intrfs[x].Name == "Template")
                    allTemplateObjects.Add(type);
            }
        }

        // Show the selection window.
        var window = EditorWindow.GetWindow<FileCreatorWindow>(true, "File Creator", true);
		window.ShowPopup();

		window.ScriptableObjectTypes = allScriptableObjects;
        window.TemplateObjectTypes = allTemplateObjects.ToArray();
	}

    /// <summary>
    /// Returns the assembly that contains the script code for this project (currently hard coded)
    /// </summary>
    public static Assembly GetAssembly() { return Assembly.Load(new AssemblyName("Assembly-CSharp")); }
}