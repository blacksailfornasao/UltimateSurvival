using System;
using UnityEngine;
using UnityEditor;

namespace UltimateSurvival.Editor
{
	public static class AnimationCurveCopier
	{
		public static void Copy(AnimationCurve source, AnimationCurve destination)
		{
			destination.keys = source.keys;
			destination.preWrapMode = source.preWrapMode;
			destination.postWrapMode = source.postWrapMode;
		}

		public static AnimationCurve CreateCopy(AnimationCurve source)
		{
			AnimationCurve newCurve = new AnimationCurve();
			Copy(source, newCurve);
			return newCurve;
		}
	}

	[CustomPropertyDrawer(typeof(AnimationCurve))]
	public class AnimationCurveFieldDrawer : PropertyDrawer 
	{
		private static AnimationCurve m_ClipboardAnimationCurve = new AnimationCurve();

		// Menu command context isn't working, so we'll just stash it here...	
		private static SerializedProperty m_PopupTargetAnimationCurveProperty = null;

		[MenuItem("CONTEXT/AnimationCurve/Copy Curve")]
		private static void Copy(MenuCommand command) 
		{
			if(m_PopupTargetAnimationCurveProperty != null)
				m_ClipboardAnimationCurve = AnimationCurveCopier.CreateCopy(m_PopupTargetAnimationCurveProperty.animationCurveValue);
	    }
		
		[MenuItem("CONTEXT/AnimationCurve/Paste Curve")]
		private static void Paste(MenuCommand command) 
		{
			if(m_PopupTargetAnimationCurveProperty != null)
			{
				m_PopupTargetAnimationCurveProperty.serializedObject.Update();
				m_PopupTargetAnimationCurveProperty.animationCurveValue = AnimationCurveCopier.CreateCopy(m_ClipboardAnimationCurve);
				m_PopupTargetAnimationCurveProperty.serializedObject.ApplyModifiedProperties();
			}
	    }

	   	public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) 
		{
			var current = Event.current;
			if(current.type == EventType.ContextClick) 
			{
	        	var mousePos = current.mousePosition;
				if(rect.Contains(mousePos))
				{
					m_PopupTargetAnimationCurveProperty = property.Copy();
					property.serializedObject.Update();
					EditorUtility.DisplayPopupMenu(new Rect(mousePos.x,mousePos.y, 0, 0), "CONTEXT/AnimationCurve/", null);
				}
			}
			else
			{
				label = EditorGUI.BeginProperty(rect, label, property);
				EditorGUI.BeginChangeCheck();
				AnimationCurve newCurve = EditorGUI.CurveField(rect, label, property.animationCurveValue);
				
				if(EditorGUI.EndChangeCheck())
					property.animationCurveValue = newCurve;
				
				EditorGUI.EndProperty();
			}
	    }
	}
}
