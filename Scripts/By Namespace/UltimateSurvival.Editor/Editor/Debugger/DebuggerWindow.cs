using UltimateSurvival;
using UltimateSurvival.AI;
using UnityEditor;
using UnityEngine;

public class DebuggerWindow : EditorWindow
{
   /* private AIDebugger m_AIDebugger;
    private ControllerDebugger m_ControllerDebugger;


    [MenuItem("Ultimate Survival/Debugger")]
    public static void OpenWindow() 
	{
		DebuggerWindow win = GetWindow<DebuggerWindow>("Debugger"); 
	}

    private void OnFocus() 
	{ 
		SubscribeScripts(); 
	}

    public void OnGUI()
    {
        SubscribeScripts();

        GameObject sO = Selection.activeGameObject;

        if (!sO)
        {
            EditorGUILayout.HelpBox("No object selected", MessageType.Warning);
            return;
        }

        CheckContents(sO);
    }

    private void CheckContents(GameObject sO)
    {
        if (sO.GetComponent<AIBrain>())
			DisplayAIDebugger(sO.GetComponent<AIBrain>(), sO.GetComponent<AISettings>());

        if (sO.GetComponent<CCDrivenController>())
            DisplayCharacterDebugger(sO.GetComponent<CCDrivenController>());

        else
			EditorGUILayout.HelpBox("No debuggable script one the selected object!", MessageType.Warning);
    }

    private void DisplayAIDebugger(AIBrain br, AISettings settings)
    {
        if (m_AIDebugger == null)
			m_AIDebugger = new AIDebugger(br, settings);

        m_AIDebugger.DrawOptions();
        m_AIDebugger.DebugInformation();
    }

    private void DisplayCharacterDebugger(CCDrivenController cc)
    {
        if (m_ControllerDebugger == null)
            m_ControllerDebugger = new ControllerDebugger(cc);

        m_ControllerDebugger.DebugInformation();
    }

    private void SubscribeScripts()
    {
        if (m_AIDebugger != null && !m_AIDebugger.Subscribed)
        {
            SceneView.onSceneGUIDelegate += m_AIDebugger.DrawOptionsResult;
            m_AIDebugger.Subscribed = true;
        }
    }

    private void UnsubscribeAll()
    {
        if (m_AIDebugger != null && m_AIDebugger.Subscribed)
        {
            SceneView.onSceneGUIDelegate -= m_AIDebugger.DrawOptionsResult;

            m_AIDebugger.Subscribed = false;
        }
    }

	private void OnEnable()
	{
		Selection.selectionChanged += Repaint;
	}

    private void OnDestroy() 
	{ 
		UnsubscribeAll(); 
		Selection.selectionChanged -= Repaint;
	}*/
}