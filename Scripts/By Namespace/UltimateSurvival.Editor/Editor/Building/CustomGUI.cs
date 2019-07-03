using UnityEditor;
using UnityEngine;

public static class CustomGUI
{
	public static readonly GUIStyle splitter;

	private static readonly Color m_Color = EditorGUIUtility.isProSkin ? new Color(0.157f, 0.157f, 0.157f) : new Color(0.5f, 0.5f, 0.5f);

	static CustomGUI() 
	{
		GUISkin skin = GUI.skin;

		splitter = new GUIStyle();
		splitter.normal.background = EditorGUIUtility.whiteTexture;
		splitter.stretchWidth = true;
		splitter.margin = new RectOffset(0, 0, 7, 7);
	}

	public static void DoHorizontalLine(Color rgb, float thickness = 1)
	{
		Rect position = GUILayoutUtility.GetRect(GUIContent.none, splitter, GUILayout.Height(thickness));

		if (Event.current.type == EventType.Repaint) 
		{
			Color restoreColor = GUI.color;
			GUI.color = rgb;
			splitter.Draw(position, false, false, false, false);
			GUI.color = restoreColor;
		}
	}

	public static void DoHorizontalLine(float thickness, GUIStyle splitterStyle) 
	{
		Rect position = GUILayoutUtility.GetRect(GUIContent.none, splitterStyle, GUILayout.Height(thickness));

		if (Event.current.type == EventType.Repaint)
		{
			Color restoreColor = GUI.color;
			GUI.color = m_Color;
			splitterStyle.Draw(position, false, false, false, false);
			GUI.color = restoreColor;
		}
	}

	public static void DoHorizontalLine(float thickness = 1) 
	{
		DoHorizontalLine(thickness, splitter);
	}
		
	public static void DoHorizontalLine(Rect position) 
	{
		if (Event.current.type == EventType.Repaint)
		{
			Color restoreColor = GUI.color;
			GUI.color = m_Color;
			splitter.Draw(position, false, false, false, false);
			GUI.color = restoreColor;
		}
	}
}