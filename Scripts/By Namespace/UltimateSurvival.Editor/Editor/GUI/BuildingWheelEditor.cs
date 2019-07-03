using UnityEngine;
using UltimateSurvival.GUISystem;

namespace UltimateSurvival.Editor
{
	using UnityEditor;

	[CustomEditor(typeof(BuildingWheel))]
	public class BuildingWheelEditor : Editor 
	{
		public static void ApplyForWheel(BuildingWheel wheel)
		{
			var categories = wheel.transform.GetComponentsInChildren<BuildingCategory>();
			for(int i = 0;i < categories.Length;i ++)
			{
				categories[i].transform.localPosition = GetPositionForElement(wheel, i) + (Vector3)categories[i].DesiredOffset;
				BuildingCategoryEditor.ApplyForCategory(categories[i]);
			}

			if(wheel.SelectionHighlight != null)
			{
				wheel.SelectionHighlight.localPosition = GetPositionForElement(wheel, 0);
				wheel.SelectionHighlight.localRotation = Quaternion.identity;
			}
		}

		private static Vector3 GetPositionForElement(BuildingWheel wheel, int index)
		{
			float angle = wheel.Offset + wheel.Spacing * index;

			return (Quaternion.Euler(Vector3.back * angle) * Vector3.up) * wheel.Distance;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if(GUI.changed && !Application.isPlaying)
				Apply();
		}

		private void OnEnable()
		{
			Apply();
			Undo.undoRedoPerformed += Apply;
		}

		private void OnDestroy()
		{
			Undo.undoRedoPerformed -= Apply;
		}

		private void Apply()
		{
			ApplyForWheel(serializedObject.targetObject as BuildingWheel);
		}
	}
}
