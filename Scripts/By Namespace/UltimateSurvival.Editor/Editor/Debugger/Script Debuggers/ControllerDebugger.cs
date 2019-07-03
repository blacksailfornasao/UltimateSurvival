using UnityEditor;
using UnityEngine;
using UltimateSurvival;

public class ControllerDebugger
{
    private CCDrivenController m_CC;

    public ControllerDebugger(CCDrivenController cc) { m_CC = cc; }

    public void DebugInformation()
    {
        EditorGUILayout.LabelField("Information:");

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Information Only Debuggable At Runtime", MessageType.Info);

            return;
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);

        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField("Near Ladders: " + m_CC.Player.NearLadders.Count.ToString());

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }
}