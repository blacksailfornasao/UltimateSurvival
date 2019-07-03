using UnityEngine;
using UnityEditor;

public class ProjectExporter : EditorWindow 
{

	[MenuItem("Window/Project Exporter")]
	static void InitializeWindow() 
	{
		GetWindow<ProjectExporter>(true, "Project Exporter");
	}

	void OnGUI() 
	{
		if(GUILayout.Button("Export All Assets!")) 
		{
			string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
			AssetDatabase.ExportPackage(allAssetPaths, "MyUnityPackage.unitypackage", ExportPackageOptions.Recurse | ExportPackageOptions.Interactive | ExportPackageOptions.IncludeLibraryAssets);
		}
	}
}
