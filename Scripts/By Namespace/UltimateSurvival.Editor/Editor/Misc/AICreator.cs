using UnityEngine;
using UnityEngine.AI;
using UltimateSurvival.AI;

namespace UltimateSurvival.Editor
{
	using UnityEditor;

	public class AICreator : ScriptableWizard 
	{
		[SerializeField]
		private GameObject m_Agent;

		[Header("Animator")]

		[SerializeField]
		private RuntimeAnimatorController m_AnimatorController;


		[MenuItem("Tools/Ultimate Survival/Add/Humanoid AI Creator")]
		private static void CreateWizard()
		{
			ScriptableWizard.DisplayWizard("Humanoid AI Creator", typeof(AICreator));
		}

		private void OnWizardCreate()
		{
			TryAddComponents();
		}
			
		private void TryAddComponents()
		{
			if(!m_Agent)
				return;	

			if(m_Agent.GetComponent<AIEventHandler>())
				Debug.LogError("Some (or all) of the components are already attached! Will not add anything.");
			else
			{
				Undo.RecordObject(m_Agent, "");
				m_Agent.layer = LayerMask.NameToLayer("Entity");

				// Animator
				var animator = m_Agent.GetComponent<Animator>();
				if(m_AnimatorController != null)
					m_Agent.GetComponent<Animator>().runtimeAnimatorController = m_AnimatorController;
				
				m_Agent.GetComponent<Animator>().applyRootMotion = false;
					
				// Component creation
				Undo.AddComponent<CapsuleCollider>(m_Agent);
				Undo.AddComponent<NavMeshAgent>(m_Agent);
				Undo.AddComponent<AudioSource>(m_Agent);

				Undo.AddComponent<AIEventHandler>(m_Agent);
				Undo.AddComponent<AISettings>(m_Agent);
				Undo.AddComponent<AIBrain>(m_Agent);
				Undo.AddComponent<EntityVitals>(m_Agent);
				Undo.AddComponent<EntityDeathHandler>(m_Agent);

				// Capsule height adjustment
				if(animator != null)
				{
					float capsuleHeight = Vector3.Distance(animator.GetBoneTransform(HumanBodyBones.Hips).position, animator.GetBoneTransform(HumanBodyBones.LeftFoot).position) * 2f;

					var capsule = m_Agent.GetComponent<CapsuleCollider>();
					capsule.height = capsuleHeight;
					capsule.center = Vector3.up * capsuleHeight / 2;
					capsule.radius = 0.3f;
				}

				// Agent
				var agent = m_Agent.GetComponent<NavMeshAgent>();
				agent.height = 1.85f;
				agent.angularSpeed = 360f;
				agent.acceleration = 22f;
				agent.stoppingDistance = 2f;
				agent.autoBraking = true;
				agent.obstacleAvoidanceType = ObstacleAvoidanceType.GoodQualityObstacleAvoidance;
				agent.avoidancePriority = 45;

				m_Agent.GetComponent<AudioSource>().spatialBlend = 1f;

				// AI Settings
				var aiSettings = m_Agent.GetComponent<AISettings>();
				var aiSettingsSer = new SerializedObject(aiSettings);

				aiSettingsSer.Update();


				aiSettingsSer.FindProperty("m_Movement").FindPropertyRelative("m_WalkSpeed").floatValue = 0.815f;
				aiSettingsSer.FindProperty("m_Movement").FindPropertyRelative("m_RunSpeed").floatValue = 4f;

				var eyes = new GameObject("Eyes");
				eyes.transform.SetParent(animator.GetBoneTransform(HumanBodyBones.Head));
				eyes.transform.position = animator.GetBoneTransform(HumanBodyBones.Head).position + Vector3.up * 0.1f;
				eyes.transform.forward = m_Agent.transform.forward;

				Undo.RegisterCreatedObjectUndo(eyes, "");

				aiSettingsSer.FindProperty("m_Detection").FindPropertyRelative("m_TargetSearchDelay").floatValue = 0.1f;
				aiSettingsSer.FindProperty("m_Detection").FindPropertyRelative("m_Eyes").objectReferenceValue = eyes;
				aiSettingsSer.FindProperty("m_Detection").FindPropertyRelative("m_ViewAngle").intValue = 135;
				aiSettingsSer.FindProperty("m_Detection").FindPropertyRelative("m_ViewRadius").intValue = 14;
				aiSettingsSer.FindProperty("m_Detection").FindPropertyRelative("m_HearRange").intValue = 30;
				aiSettingsSer.FindProperty("m_Detection").FindPropertyRelative("m_SpotMask").intValue = LayerMask.GetMask("Entity");
				aiSettingsSer.FindProperty("m_Detection").FindPropertyRelative("m_ObstacleMask").intValue = LayerMask.GetMask("Default", "Building Piece");

				aiSettingsSer.FindProperty("m_HitDamage").floatValue = 25f;
				aiSettingsSer.FindProperty("m_MaxAttackDistance").floatValue = 3f;

				aiSettingsSer.FindProperty("m_AudioSource").objectReferenceValue = m_Agent.GetComponent<AudioSource>();


				aiSettingsSer.ApplyModifiedProperties();

				// AI Brain
				var aiBrain = m_Agent.GetComponent<AIBrain>();
				var aiBrainSer = new SerializedObject(aiBrain);

				aiBrainSer.Update();


				aiBrainSer.FindProperty("m_AvailableActions").arraySize = 2;
		
				aiBrainSer.FindProperty("m_AvailableActions").GetArrayElementAtIndex(0).objectReferenceValue = Resources.Load<ScriptableObject>("Custom Data/AI/Actions/Attack (Cannibal)");
				aiBrainSer.FindProperty("m_AvailableActions").GetArrayElementAtIndex(1).objectReferenceValue = Resources.Load<ScriptableObject>("Custom Data/AI/Actions/Chase (Cannibal)");

				aiBrainSer.FindProperty("m_AvailableGoals").arraySize = 1;
				aiBrainSer.FindProperty("m_AvailableGoals").GetArrayElementAtIndex(0).objectReferenceValue = Resources.Load<ScriptableObject>("Custom Data/AI/Goals/Kill Player");

				aiBrainSer.FindProperty("m_Fallback").objectReferenceValue = Resources.Load<AI.Actions.Action>("Custom Data/AI/Actions/Random Patrol (Cannibal)");
				aiBrainSer.FindProperty("m_MinPlanInterval").floatValue = 1;
				aiBrainSer.FindProperty("m_MinGoalPriorityCheckInterval").floatValue = 1;


				aiBrainSer.ApplyModifiedProperties();

				// Entity Vitals
				var entityVitals = m_Agent.GetComponent<EntityVitals>();
				var entityVitalsSer = new SerializedObject(entityVitals);

				entityVitalsSer.Update();

	
				entityVitalsSer.FindProperty("m_AudioSource").objectReferenceValue = m_Agent.GetComponent<AudioSource>();
				entityVitalsSer.FindProperty("m_Animator").objectReferenceValue = animator;


				entityVitalsSer.ApplyModifiedProperties();

				// Entity Death Handler
				var deathHandler = m_Agent.GetComponent<EntityDeathHandler>();
				var deathHandlerSer = new SerializedObject(deathHandler);

				deathHandlerSer.Update();


				deathHandlerSer.FindProperty("m_AudioSource").objectReferenceValue = m_Agent.GetComponent<AudioSource>();

				var behToDisable = deathHandlerSer.FindProperty("m_BehavioursToDisable");
				behToDisable.arraySize = 4;
				behToDisable.GetArrayElementAtIndex(0).objectReferenceValue = animator;
				behToDisable.GetArrayElementAtIndex(1).objectReferenceValue = agent;
				behToDisable.GetArrayElementAtIndex(2).objectReferenceValue = aiBrain;
				behToDisable.GetArrayElementAtIndex(3).objectReferenceValue = entityVitals;

				deathHandlerSer.FindProperty("m_CollidersToDisable").arraySize = 1;
				deathHandlerSer.FindProperty("m_CollidersToDisable").GetArrayElementAtIndex(0).objectReferenceValue = m_Agent.GetComponent<CapsuleCollider>();

				deathHandlerSer.FindProperty("m_DestroyTimer").floatValue = 30f;


				deathHandlerSer.ApplyModifiedProperties();

				// Loot
				var hips = animator.GetBoneTransform(HumanBodyBones.Hips);
				var loot = hips.gameObject.AddComponent<LootObject>();
				loot.enabled = false;
				var lootOnDeath = hips.gameObject.AddComponent<LootOnDeath>();
				lootOnDeath.LootContainer = loot;
			}
		}
	}
}
