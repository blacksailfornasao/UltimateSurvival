using UnityEngine;
using UnityEngine.UI;
using UltimateSurvival.GUISystem;

namespace UltimateSurvival.Editor
{
	using UnityEditor;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(FPMotion))]
	public class FPMotionDrawer : Editor
	{
		public enum Preset { Tool, TwoHandedGun, Handgun }


		private Preset m_SelectedPreset;

		private SerializedProperty m_MovementSway;
		private SerializedProperty m_RotationSway;

		private SerializedProperty m_WalkBob;
		private SerializedProperty m_AimBob;
		private SerializedProperty m_RunBob;
		//private SerializedProperty m_LandBob;

		private SerializedProperty m_IdleOffset;
		private SerializedProperty m_RunOffset;
		private SerializedProperty m_AimOffset;
		private SerializedProperty m_OnLadderOffset;
		private SerializedProperty m_JumpOffset;
		private SerializedProperty m_TooCloseOffset;


		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			CustomGUI.DoHorizontalLine();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			var rect = EditorGUILayout.GetControlRect();
			rect.width *= 0.5f;

			serializedObject.Update();

			if(GUI.Button(rect, "Apply Presets"))
				if(EditorUtility.DisplayDialog("Apply Presets", "Applying a preset will overwrite the current values, are you sure?", "Yes", "Cancel"))
					ApplyPresets();

			serializedObject.ApplyModifiedProperties();

			rect.x = rect.xMax;
			m_SelectedPreset = (Preset)EditorGUI.EnumPopup(rect, m_SelectedPreset);
		}

		private void OnEnable()
		{
			m_MovementSway = serializedObject.FindProperty("m_MovementSway");
			m_RotationSway = serializedObject.FindProperty("m_RotationSway");

			m_WalkBob = serializedObject.FindProperty("m_WalkBob");
			m_AimBob = serializedObject.FindProperty("m_AimBob");
			m_RunBob = serializedObject.FindProperty("m_RunBob");
			//m_LandBob = serializedObject.FindProperty("m_LandBob");

			m_IdleOffset = serializedObject.FindProperty("m_IdleOffset");
			m_RunOffset = serializedObject.FindProperty("m_RunOffset");
			m_AimOffset = serializedObject.FindProperty("m_AimOffset");
			m_OnLadderOffset = serializedObject.FindProperty("m_OnLadderOffset");
			m_JumpOffset = serializedObject.FindProperty("m_JumpOffset");
			m_TooCloseOffset = serializedObject.FindProperty("m_TooCloseOffset");
		}

		private void ApplyPresets()
		{
			if(m_SelectedPreset == Preset.Tool)
			{
				m_MovementSway.FindPropertyRelative("Enabled").boolValue = false;

				m_RotationSway.FindPropertyRelative("Magnitude").vector2Value = new Vector2(5f, 3.5f);
				m_RotationSway.FindPropertyRelative("LerpSpeed").floatValue = 4f;


				m_WalkBob.FindPropertyRelative("m_Speed").floatValue = 0.26f;
				m_WalkBob.FindPropertyRelative("m_CooldownSpeed").floatValue = 3f;
				m_WalkBob.FindPropertyRelative("m_AmountX").floatValue = 0.008f;
				m_WalkBob.FindPropertyRelative("m_AmountY").floatValue = 0.008f;
				m_WalkBob.FindPropertyRelative("m_Curve").animationCurveValue = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

				m_AimBob.FindPropertyRelative("m_Speed").floatValue = 0.26f;
				m_AimBob.FindPropertyRelative("m_CooldownSpeed").floatValue = 3f;
				m_AimBob.FindPropertyRelative("m_AmountX").floatValue = 0.01f;
				m_AimBob.FindPropertyRelative("m_AmountY").floatValue = 0.01f;
				m_AimBob.FindPropertyRelative("m_Curve").animationCurveValue = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

				m_RunBob.FindPropertyRelative("m_Speed").floatValue = 0.26f;
				m_RunBob.FindPropertyRelative("m_CooldownSpeed").floatValue = 3f;
				m_RunBob.FindPropertyRelative("m_AmountX").floatValue = 0.015f;
				m_RunBob.FindPropertyRelative("m_AmountY").floatValue = 0.015f;
				m_RunBob.FindPropertyRelative("m_Curve").animationCurveValue = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));


				m_IdleOffset.FindPropertyRelative("m_LerpSpeed").floatValue = 7f;
				m_IdleOffset.FindPropertyRelative("m_Position").vector3Value = new Vector3(0f, 0f, 0f);
				m_IdleOffset.FindPropertyRelative("m_Rotation").vector3Value = new Vector3(0f, 0f, 0f);

				m_RunOffset.FindPropertyRelative("m_LerpSpeed").floatValue = 4f;
				m_RunOffset.FindPropertyRelative("m_Position").vector3Value = new Vector3(0.05f, -0.2f, 0f);
				m_RunOffset.FindPropertyRelative("m_Rotation").vector3Value = new Vector3(0f, 4f, 0f);

				m_AimOffset.FindPropertyRelative("m_LerpSpeed").floatValue = 5f;
				m_AimOffset.FindPropertyRelative("m_Position").vector3Value = new Vector3(0f, 0f, 0f);
				m_AimOffset.FindPropertyRelative("m_Rotation").vector3Value = new Vector3(0f, 0f, 0f);

				m_OnLadderOffset.FindPropertyRelative("m_LerpSpeed").floatValue = 4f;
				m_OnLadderOffset.FindPropertyRelative("m_Position").vector3Value = new Vector3(0f, 0f, 0f);
				m_OnLadderOffset.FindPropertyRelative("m_Rotation").vector3Value = new Vector3(0f, 0f, 0f);

				m_JumpOffset.FindPropertyRelative("m_LerpSpeed").floatValue = 4f;
				m_JumpOffset.FindPropertyRelative("m_Position").vector3Value = new Vector3(0f, -0.03f, -0.03f);
				m_JumpOffset.FindPropertyRelative("m_Rotation").vector3Value = new Vector3(-8f, 0f, 0f);

				m_TooCloseOffset.FindPropertyRelative("m_LerpSpeed").floatValue = 5f;
				m_TooCloseOffset.FindPropertyRelative("m_Position").vector3Value = new Vector3(0f, 0f, 0f);
				m_TooCloseOffset.FindPropertyRelative("m_Rotation").vector3Value = new Vector3(0f, 0f, 0f);
			}
			else if(m_SelectedPreset == Preset.TwoHandedGun)
			{
				m_MovementSway.FindPropertyRelative("Enabled").boolValue = false;

				m_RotationSway.FindPropertyRelative("Magnitude").vector2Value = new Vector2(1.5f, 1.5f);
				m_RotationSway.FindPropertyRelative("LerpSpeed").floatValue = 4f;


				m_WalkBob.FindPropertyRelative("m_Speed").floatValue = 0.28f;
				m_WalkBob.FindPropertyRelative("m_CooldownSpeed").floatValue = 3f;
				m_WalkBob.FindPropertyRelative("m_AmountX").floatValue = 0.003f;
				m_WalkBob.FindPropertyRelative("m_AmountY").floatValue = 0.003f;
				m_WalkBob.FindPropertyRelative("m_Curve").animationCurveValue = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

				m_AimBob.FindPropertyRelative("m_Speed").floatValue = 0.26f;
				m_AimBob.FindPropertyRelative("m_CooldownSpeed").floatValue = 3f;
				m_AimBob.FindPropertyRelative("m_AmountX").floatValue = 0.0007f;
				m_AimBob.FindPropertyRelative("m_AmountY").floatValue = 0.0007f;
				m_AimBob.FindPropertyRelative("m_Curve").animationCurveValue = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

				m_RunBob.FindPropertyRelative("m_Speed").floatValue = 0.22f;
				m_RunBob.FindPropertyRelative("m_CooldownSpeed").floatValue = 3f;
				m_RunBob.FindPropertyRelative("m_AmountX").floatValue = 0.0085f;
				m_RunBob.FindPropertyRelative("m_AmountY").floatValue = 0.0085f;
				m_RunBob.FindPropertyRelative("m_Curve").animationCurveValue = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));


				m_IdleOffset.FindPropertyRelative("m_LerpSpeed").floatValue = 7f;
				m_IdleOffset.FindPropertyRelative("m_Position").vector3Value = new Vector3(0f, 0f, 0f);
				m_IdleOffset.FindPropertyRelative("m_Rotation").vector3Value = new Vector3(0f, 0f, 0f);

				m_RunOffset.FindPropertyRelative("m_LerpSpeed").floatValue = 3f;
				m_RunOffset.FindPropertyRelative("m_Position").vector3Value = new Vector3(-0.085f, -0.05f, 0f);
				m_RunOffset.FindPropertyRelative("m_Rotation").vector3Value = new Vector3(10.76f, -19.46f, 10.67f);

				m_AimOffset.FindPropertyRelative("m_LerpSpeed").floatValue = 12f;
				m_AimOffset.FindPropertyRelative("m_Position").vector3Value = new Vector3(-0.068f, 0.045f, -0.05f);
				m_AimOffset.FindPropertyRelative("m_Rotation").vector3Value = new Vector3(0f, 0.06f, 0f);

				m_OnLadderOffset.FindPropertyRelative("m_LerpSpeed").floatValue = 4f;
				m_OnLadderOffset.FindPropertyRelative("m_Position").vector3Value = new Vector3(.1f, -0.5f, 0f);
				m_OnLadderOffset.FindPropertyRelative("m_Rotation").vector3Value = new Vector3(-15f, -45f, 0f);

				m_JumpOffset.FindPropertyRelative("m_LerpSpeed").floatValue = 8f;
				m_JumpOffset.FindPropertyRelative("m_Position").vector3Value = new Vector3(0f, 0f, 0f);
				m_JumpOffset.FindPropertyRelative("m_Rotation").vector3Value = new Vector3(-5f, 0f, 0f);

				m_TooCloseOffset.FindPropertyRelative("m_LerpSpeed").floatValue = 7f;
				m_TooCloseOffset.FindPropertyRelative("m_Position").vector3Value = new Vector3(-0.05f, 0f, 0f);
				m_TooCloseOffset.FindPropertyRelative("m_Rotation").vector3Value = new Vector3(-79.4f, 0f, 29f);
			}
			else if(m_SelectedPreset == Preset.Handgun)
			{
				m_MovementSway.FindPropertyRelative("Enabled").boolValue = false;

				m_RotationSway.FindPropertyRelative("Magnitude").vector2Value = new Vector2(3f, 3f);
				m_RotationSway.FindPropertyRelative("LerpSpeed").floatValue = 4f;


				m_WalkBob.FindPropertyRelative("m_Speed").floatValue = 0.26f;
				m_WalkBob.FindPropertyRelative("m_CooldownSpeed").floatValue = 3f;
				m_WalkBob.FindPropertyRelative("m_AmountX").floatValue = 0.004f;
				m_WalkBob.FindPropertyRelative("m_AmountY").floatValue = 0.004f;
				m_WalkBob.FindPropertyRelative("m_Curve").animationCurveValue = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

				m_AimBob.FindPropertyRelative("m_Speed").floatValue = 0.26f;
				m_AimBob.FindPropertyRelative("m_CooldownSpeed").floatValue = 3f;
				m_AimBob.FindPropertyRelative("m_AmountX").floatValue = 0.002f;
				m_AimBob.FindPropertyRelative("m_AmountY").floatValue = 0.002f;
				m_AimBob.FindPropertyRelative("m_Curve").animationCurveValue = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

				m_RunBob.FindPropertyRelative("m_Speed").floatValue = 0.22f;
				m_RunBob.FindPropertyRelative("m_CooldownSpeed").floatValue = 3f;
				m_RunBob.FindPropertyRelative("m_AmountX").floatValue = 0.01f;
				m_RunBob.FindPropertyRelative("m_AmountY").floatValue = 0.01f;
				m_RunBob.FindPropertyRelative("m_Curve").animationCurveValue = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));


				m_IdleOffset.FindPropertyRelative("m_LerpSpeed").floatValue = 7f;
				m_IdleOffset.FindPropertyRelative("m_Position").vector3Value = new Vector3(0f, 0f, 0f);
				m_IdleOffset.FindPropertyRelative("m_Rotation").vector3Value = new Vector3(0f, 0f, 0f);

				m_RunOffset.FindPropertyRelative("m_LerpSpeed").floatValue = 2f;
				m_RunOffset.FindPropertyRelative("m_Position").vector3Value = new Vector3(0f, -0.05f, 0f);
				m_RunOffset.FindPropertyRelative("m_Rotation").vector3Value = new Vector3(16f, 0f, 0f);

				m_AimOffset.FindPropertyRelative("m_LerpSpeed").floatValue = 12f;
				m_AimOffset.FindPropertyRelative("m_Position").vector3Value = new Vector3(-0.071f, 0.028f, -0.1f);
				m_AimOffset.FindPropertyRelative("m_Rotation").vector3Value = new Vector3(0f, 0f, 0f);

				m_OnLadderOffset.FindPropertyRelative("m_LerpSpeed").floatValue = 2.2f;
				m_OnLadderOffset.FindPropertyRelative("m_Position").vector3Value = new Vector3(0.1f, -0.3f, 0f);
				m_OnLadderOffset.FindPropertyRelative("m_Rotation").vector3Value = new Vector3(66f, 0f, 0f);

				m_JumpOffset.FindPropertyRelative("m_LerpSpeed").floatValue = 5f;
				m_JumpOffset.FindPropertyRelative("m_Position").vector3Value = new Vector3(0f, -0.02f, 0f);
				m_JumpOffset.FindPropertyRelative("m_Rotation").vector3Value = new Vector3(-5f, 0f, 0f);

				m_TooCloseOffset.FindPropertyRelative("m_LerpSpeed").floatValue = 9f;
				m_TooCloseOffset.FindPropertyRelative("m_Position").vector3Value = new Vector3(-0.05f, 0f, 0f);
				m_TooCloseOffset.FindPropertyRelative("m_Rotation").vector3Value = new Vector3(-79.6f, 0f, 36.96f);
			}
		}
	}
}
