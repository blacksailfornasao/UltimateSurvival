using UnityEngine;
using UltimateSurvival.AI;
using UnityEngine.AI;

namespace UltimateSurvival.Editor
{
	using UnityEditor;

	public class MenuUtilities
	{
		[MenuItem("Tools/Ultimate Survival/Add/Base Scene Objects", false, -10)]
		public static void CreateSimpleScene()
		{
			if(Object.FindObjectOfType<GameController>())
				Debug.LogError("Some (or all) base objects already exist in the scene! Will not create anything.");
			else
			{
				var allPrefabs = Resources.LoadAll<GameObject>("_Scene Base");
				if(allPrefabs.Length == 0)
					Debug.LogWarning("No objects found at path Resources/_Scene Base");

				foreach(var prefab in allPrefabs)
				{
					var obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

					obj.transform.position = Vector3.zero;
					obj.transform.rotation = Quaternion.identity;
					obj.name = obj.name.Replace("(Clone)", "");

					Undo.RegisterCreatedObjectUndo(obj, "Create Simple Scene");

					Debug.Log("Added " + obj.name);
				}
			}
		}
	}
}