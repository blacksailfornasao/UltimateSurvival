using System;
using System.Collections.Generic;
using UltimateSurvival;
using UltimateSurvival.AI;
using UnityEditor;
using UnityEngine;

public class AIDebugger
{
   /* public bool Subscribed;

    private bool m_DebugAgentView;
    private bool m_DebugCurrentDestination;
    private bool m_DebugFindFoodPoints;
    private bool m_DebugPatrolPoints;
    private bool m_DebugVisibleTargets;
    private bool m_DebugStillInRangeTargets;

    private AIBrain m_EBrain;
	private AISettings settings;

	public AIDebugger(AIBrain br, AISettings settings)
    {
        m_EBrain = br;

        settings = settings;
    }

    public void DebugInformation()
    {
        EditorGUILayout.LabelField("Information:");

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Information only debuggable at runtime", MessageType.Info);

            return;
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);

        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField(GetCurrentAction().ToString());

        EditorGUILayout.LabelField(GetNextAction());

        EditorGUILayout.LabelField(GetCurrentAction());

		EditorGUILayout.LabelField(settings.Movement.MovementState.ToString());

        EditorGUILayout.LabelField(settings.Vitals.IsHungry.ToString());

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    public void DrawOptions()
    {
        EditorGUILayout.LabelField("Debug Options:");

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);

        EditorGUILayout.BeginVertical();

        m_DebugAgentView = EditorGUILayout.Toggle("Agent View", m_DebugAgentView);

        m_DebugCurrentDestination = EditorGUILayout.Toggle("Current Destination", m_DebugCurrentDestination);

        m_DebugFindFoodPoints = EditorGUILayout.Toggle("Find Food Points", m_DebugFindFoodPoints);

        m_DebugPatrolPoints = EditorGUILayout.Toggle("Patrol Points", m_DebugPatrolPoints);

        m_DebugVisibleTargets = EditorGUILayout.Toggle("Visible Targets", m_DebugVisibleTargets);

        m_DebugStillInRangeTargets = EditorGUILayout.Toggle("Still In Range Targets", m_DebugStillInRangeTargets);

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    public void DrawOptionsResult(SceneView view)
    {
        view.Repaint();

        if (m_DebugAgentView)
            DrawView();

        if (m_DebugCurrentDestination)
            DrawDestination();

        DrawTargets();

        DrawSpecificActions();
    }

    private void DrawSpecificActions()
    {
       if (m_DebugFindFoodPoints)
		{
            int index = FindActionOfType(typeof(FindFoodAction));
            FindFoodAction ffA = null;

            if (m_EBrain.AvailableActions.IndexIsValid(index))
                ffA = m_EBrain.AvailableActions[index] as FindFoodAction;

            if (ffA == null)
            {
                ffA = m_EBrain.Fallback as FindFoodAction;

                if (ffA == null)
                    return;
            }

            List<Vector3> posS = new List<Vector3>();
            posS = posS.CopyOther(ffA.PointPositions);

            DrawPoints(posS);
        }

        if (m_DebugPatrolPoints)
        {
            int index = FindActionOfType(typeof(PatrolAction));
            PatrolAction pA = null;

            if (m_EBrain.AvailableActions.IndexIsValid(index))
                pA = m_EBrain.AvailableActions[index] as PatrolAction;

            if (pA == null)
            {
                pA = m_EBrain.Fallback as PatrolAction;

                if (pA == null)
                    return;
            }

            List<Vector3> posS = new List<Vector3>();
            posS = posS.CopyOther(pA.PointPositions);

            DrawPoints(posS);
        }
    }

    private int FindActionOfType(Type type)
    {
        int toReturn = -1;

        List<UltimateSurvival.AI.Action> available = m_EBrain.AvailableActions;

        for (int i = 0; i < available.Count; i++)
        {
            if (available[i].GetType().Name == type.Name)
                toReturn = i;
        }

        return toReturn;
    }

    private void DrawTargets()
    {
        if (m_DebugVisibleTargets)
            DrawTargetListDebugs(settings.Detection.VisibleTargets, Color.green, 1.5f, 1);

        if (m_DebugStillInRangeTargets)
            DrawTargetListDebugs(settings.Detection.StillInRangeTargets, Color.blue, 3, 0.5f);
    }

    private static void DrawTargetListDebugs(List<GameObject> list, Color color, float plus, float size)
    {
        bool targetsExist = (list != null);
        bool targetsCountIsNotZero = (list.Count > 0);

        if (targetsExist && targetsCountIsNotZero)
        {
            Handles.color = color;
            List<GameObject> targets = list;

            for (int i = 0; i < list.Count; i++)
            {
                Vector3 position = targets[i].transform.position;
                position.y += plus;

                Handles.CubeCap(i, position, Quaternion.identity, size);
            }
        }
    }

    private void DrawPoints(List<Vector3> points)
    {
        if (points == null || points.Count == 0)
            return;

        Handles.color = Color.green;

        if (settings.Movement.Agent)
        {
            float agentStopDistance = settings.Movement.Agent.stoppingDistance;
            float stopDistance = (agentStopDistance < 1) ? 1 : agentStopDistance;

            for (int i = 0; i < points.Count; i++)
                Handles.DrawWireArc(points[i], Vector3.up, Vector3.forward, 360, stopDistance);
        }
    }

    private void DrawDestination()
    {
        Handles.color = Color.red;

        Vector3 destination = settings.Movement.CurrentDestination;

        if (destination != Vector3.zero)
            Handles.DrawLine(settings.transform.position, destination);
    }

    private void DrawView()
    {
        Handles.color = Color.black;

        int vAngle = settings.Detection.ViewAngle;
        int vRadius = settings.Detection.ViewRadius;

        Vector3 dPos = settings.transform.position;

        if (vRadius > 0)
        {
            Handles.DrawWireArc(dPos, Vector3.up, m_EBrain.transform.forward, 360, vRadius);

            if (vAngle > 0)
            {
                Vector3 viewAngleA = DirFromAngle(-vAngle / 2, settings.transform);
                Vector3 viewAngleB = DirFromAngle(vAngle / 2, settings.transform);

                Handles.DrawLine(dPos, dPos + viewAngleA * settings.Detection.ViewRadius);
                Handles.DrawLine(dPos, dPos + viewAngleB * settings.Detection.ViewRadius);
            }
        }
    }

    private Vector3 DirFromAngle(float angleInDeg, Transform brTr)
    {
        angleInDeg += brTr.transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(angleInDeg * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDeg * Mathf.Deg2Rad));
    }

    private string GetCurrentAction()
    {
        if (m_EBrain.ActionQueue == null || m_EBrain.ActionQueue.Count == 0)
            return string.Empty;

        return m_EBrain.ActionQueue.Peek().ToString();
    }

    private string GetNextAction()
    {
        if (m_EBrain.ActionQueue == null || m_EBrain.ActionQueue.Count < 2)
            return string.Empty;

        return m_EBrain.ActionQueue.ToArray()[1].ToString();
    }*/
}