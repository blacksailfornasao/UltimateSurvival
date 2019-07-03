using UnityEngine;
using UltimateSurvival.GUISystem;

namespace UltimateSurvival.Editor
{
	using UnityEditor;

	[CustomEditor(typeof(BuildingCategory))]
	[CanEditMultipleObjects]
	public class BuildingCategoryEditor : Editor 
	{
		public static void ApplyForCategory(BuildingCategory category)
		{
			Vector3 categoryPos = category.transform.localPosition;
			var wheel = category.GetComponentInParent<BuildingWheel>();

			if(!wheel)
				return;

			var pieces = category.transform.GetComponentsInChildren<BuildingPiece>();
			for(int i = 0;i < pieces.Length;i ++)
				pieces[i].transform.position = wheel.transform.TransformPoint(GetPositionForElement(category, i)) + (Vector3)pieces[i].DesiredOffset;
		}

		private static Vector3 GetPositionForElement(BuildingCategory category, int index)
		{
			Vector3 categoryPos = category.transform.localPosition;

			float angle = category.Offset + category.Spacing * index;

			//(Quaternion.Euler(Vector3.back * angle) * Vector3.up) * wheel.Distance

			return Quaternion.Euler(Vector3.back * angle) * (categoryPos + categoryPos.normalized * category.Distance);
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if(GUI.changed && !Application.isPlaying)
				Apply();
		}

		private void OnEnable()
		{
			//Debug.Log("on enable");
			Apply();
			//EditorApplication.hierarchyWindowChanged += OnHierarchyChange;
		}

		private void OnHierarchyChange()
		{
			if(!Application.isPlaying)
				Apply();
		}

		private void OnDestroy()
		{
			//EditorApplication.hierarchyWindowChanged -= OnHierarchyChange;
		}

		private void Apply()
		{
			ApplyForCategory(serializedObject.targetObject as BuildingCategory);

			var wheel = (serializedObject.targetObject as BuildingCategory).GetComponentInParent<BuildingWheel>();
			if(wheel != null)
				BuildingWheelEditor.ApplyForWheel(wheel);
		}
	}
}
