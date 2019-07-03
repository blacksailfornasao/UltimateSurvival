using UnityEngine;
using System.Collections;
using System;

namespace UltimateSurvival.Editor
{
	using UnityEditor;

	public class BoneData
	{
		public string Name;

		public Transform Anchor;
		public CharacterJoint Joint;
		public BoneData Parent;

		public float MinLimit;
		public float MaxLimit;
		public float SwingLimit;

		public Vector3 Axis;
		public Vector3 NormalAxis;

		public float RadiusScale;
		public Type ColliderType;

		public ArrayList Children = new ArrayList();
		public float Density;
		public float TotalMass;
	}

	public class RagdollCreator : ScriptableWizard
	{
		[SerializeField]
		private Animator m_Animator;

		[Header("Bones")]

		[SerializeField]
		private Transform m_Root;

		[SerializeField]
		private Transform m_LeftHips;

		[SerializeField]
		private Transform m_LeftKnee;

		[SerializeField]
		private Transform m_LeftFoot;

		[SerializeField]
		private Transform m_RightHips;

		[SerializeField]
		private Transform m_RightKnee;

		[SerializeField]
		private Transform m_RightFoot;

		[SerializeField]
		private Transform m_LeftArm;

		[SerializeField]
		private Transform m_LeftElbow;

		[SerializeField]
		private Transform m_RightArm;

		[SerializeField]
		private Transform m_RightElbow;

		[SerializeField]
		private Transform m_MiddleSpine;

		[SerializeField]
		private Transform m_Head;

		[Header("Joint Settings")]

		[SerializeField]
		private bool m_EnableProjection = true;

		[Header("Ragdoll Settings")]

		[SerializeField]
		private float m_TotalMass = 60f;

		[SerializeField]
		private bool m_FlipForward; 

		private Vector3 m_Right = Vector3.right;
		private Vector3 m_Up = Vector3.up;
		private Vector3 m_Forward = Vector3.forward;
		
		private Vector3 m_WorldRight = Vector3.right;
		private Vector3 m_WorldUp = Vector3.up;
		private Vector3 m_WorldForward = Vector3.forward;   

		private ArrayList m_Bones;
		private BoneData m_RootBone;


		private string CheckConsistency()
		{
			PrepareBones();
			Hashtable map = new Hashtable();

			foreach (BoneData bone in m_Bones)
			{
				if (bone.Anchor)
				{
					if (map[bone.Anchor] != null)
					{
						BoneData oldBone = (BoneData)map[bone.Anchor];
						return String.Format("{0} and {1} may not be assigned to the same bone.", bone.Name, oldBone.Name);
					}

					map[bone.Anchor] = bone;
				}
			}
			
			foreach (BoneData bone in m_Bones)
			{
				if (bone.Anchor == null)
					return String.Format("{0} has not been assigned yet.\n", bone.Name);
			}
			
			return "";
		}

		[MenuItem("Tools/Ultimate Survival/Add/Humanoid Ragdoll")]
		private static void CreateWizard()
		{
			ScriptableWizard.DisplayWizard ("Ragdoll Creator", typeof(RagdollCreator));
		}

		private void OnEnable()
		{
			Selection.selectionChanged += OnSelectionChanged;
		}

		private void OnSelectionChanged()
		{
			m_RootBone = null;
			m_Bones = null;

			if(Selection.activeGameObject == null)
			{
				m_Animator = null;

				m_Root = null;

				m_LeftHips = null;
				m_LeftKnee = null;
				m_LeftFoot = null;

				m_RightHips = null;
				m_RightKnee = null;
				m_RightFoot = null;

				m_LeftArm = null;
				m_LeftElbow = null;

				m_RightArm = null;
				m_RightElbow = null;

				m_MiddleSpine = null;

				m_Head = null;
			}

			Repaint();
		}

		private void OnDestroy()
		{
			Selection.selectionChanged -= OnSelectionChanged;
		}
		
		private void DecomposeVector(out Vector3 normal, out Vector3 tangent, Vector3 outwardDir, Vector3 outwardNormal)
		{
			outwardNormal = outwardNormal.normalized;
			normal = outwardNormal * Vector3.Dot(outwardDir, outwardNormal);
			tangent = outwardDir - normal;
		}
		
		private void CalculateAxes()
		{
			if (m_Head != null && m_Root != null)
				m_Up = CalculateDirectionAxis(m_Root.InverseTransformPoint(m_Head.position));
			if (m_RightElbow != null && m_Root != null)
			{
				Vector3 removed, temp;
				DecomposeVector(out temp, out removed, m_Root.InverseTransformPoint(m_RightElbow.position), m_Up);
				m_Right = CalculateDirectionAxis(removed);
			}
			
			m_Forward = Vector3.Cross(m_Right, m_Up);
			if (m_FlipForward)
				m_Forward = -m_Forward;	
		}

		private void Update()
		{        
			errorString = CheckConsistency();
			CalculateAxes();		

			if (errorString.Length != 0)
				helpString = "Drag all bones from the hierarchy into their slots.\nMake sure your character is in T-Stand.\n";
			
			isValid = errorString.Length == 0;
		}

		private void PrepareBones()
		{
			if(Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Animator>() != null)
				m_Animator = Selection.activeGameObject.GetComponent<Animator>();
			if(m_Animator != null) 
			{			
				try {
					m_Root = m_Animator.GetBoneTransform (HumanBodyBones.Hips);
					
					m_LeftHips = m_Animator.GetBoneTransform (HumanBodyBones.LeftUpperLeg);
					m_LeftKnee = m_Animator.GetBoneTransform (HumanBodyBones.LeftLowerLeg);
					m_LeftFoot = m_Animator.GetBoneTransform (HumanBodyBones.LeftFoot);
					
					m_RightHips = m_Animator.GetBoneTransform (HumanBodyBones.RightUpperLeg);
					m_RightKnee = m_Animator.GetBoneTransform (HumanBodyBones.RightLowerLeg);
					m_RightFoot = m_Animator.GetBoneTransform (HumanBodyBones.RightFoot);
					
					m_LeftArm = m_Animator.GetBoneTransform (HumanBodyBones.LeftUpperArm);
					m_LeftElbow = m_Animator.GetBoneTransform (HumanBodyBones.LeftLowerArm);
					
					m_RightArm = m_Animator.GetBoneTransform (HumanBodyBones.RightUpperArm);
					m_RightElbow = m_Animator.GetBoneTransform (HumanBodyBones.RightLowerArm);
					
					m_MiddleSpine = m_Animator.GetBoneTransform (HumanBodyBones.Chest);

					m_Head = m_Animator.GetBoneTransform (HumanBodyBones.Head);

					EditorUtility.SetDirty(this);
				} catch {
				}
			}

			if (m_Root)
			{
				m_WorldRight = m_Root.TransformDirection(m_Right);
				m_WorldUp = m_Root.TransformDirection(m_Up);
				m_WorldForward = m_Root.TransformDirection(m_Forward);
			}
			
			m_Bones = new ArrayList();
			
			m_RootBone = new BoneData();
			m_RootBone.Name = "Root";
			m_RootBone.Anchor = m_Root;
			m_RootBone.Parent = null;
			m_RootBone.Density = 2.5F;
			m_Bones.Add (m_RootBone);
			
			AddMirroredJoint ("Hips", m_LeftHips, m_RightHips, "Root", m_WorldRight, m_WorldForward, -20, 70, 30, typeof(CapsuleCollider), 0.3F, 1.5f);
			AddMirroredJoint ("Knee", m_LeftKnee, m_RightKnee, "Hips", m_WorldRight, m_WorldForward, -80, 0, 0, typeof(CapsuleCollider), 0.25F, 1.5f);
			
			AddJoint ("Middle Spine", m_MiddleSpine, "Root", m_WorldRight, m_WorldForward, -20, 20, 10, null, 1f, 2.5f);
			
			AddMirroredJoint ("Arm", m_LeftArm, m_RightArm, "Middle Spine", m_WorldUp, m_WorldForward, -70f, 10f, 50f, typeof(CapsuleCollider), 0.25f, 1f);
			AddMirroredJoint ("Elbow", m_LeftElbow, m_RightElbow, "Arm", m_WorldForward, m_WorldUp, -90f, 0f, 0f, typeof(CapsuleCollider), 0.2f, 1f);
			
			AddJoint ("Head", m_Head, "Middle Spine", m_WorldRight, m_WorldForward, -40, 25, 25, null, 1, 1.0f);
		}

		private void OnWizardCreate()
		{		
			Cleanup();
			BuildCapsules();	
			AddBreastColliders();
			AddHeadCollider();
			
			BuildBodies ();
			BuildJoints ();
			CalculateMass();
		}

		private BoneData FindBone (string name)
		{
			foreach (BoneData bone in m_Bones)
			{
				if (bone.Name == name)
					return bone;
			}

			return null;
		}
		
		private void AddMirroredJoint (string name, Transform leftAnchor, Transform rightAnchor, string parent, Vector3 worldTwistAxis, Vector3 worldSwingAxis, float minLimit, float maxLimit, float swingLimit, Type colliderType, float radiusScale, float density)
		{
			AddJoint ("Left " + name, leftAnchor, parent, worldTwistAxis, worldSwingAxis, minLimit, maxLimit, swingLimit, colliderType, radiusScale, density);
			AddJoint ("Right " + name, rightAnchor, parent, worldTwistAxis, worldSwingAxis, minLimit, maxLimit, swingLimit, colliderType, radiusScale, density);
		}	
		
		private void AddJoint(string name, Transform anchor, string parent, Vector3 worldTwistAxis, Vector3 worldSwingAxis, float minLimit, float maxLimit, float swingLimit, Type colliderType, float radiusScale, float density)
		{
			BoneData bone = new BoneData();
			bone.Name = name;
			bone.Anchor = anchor;
			bone.Axis = worldTwistAxis;
			bone.NormalAxis = worldSwingAxis;
			bone.MinLimit = minLimit;
			bone.MaxLimit = maxLimit;
			bone.SwingLimit = swingLimit;
			bone.Density = density;
			bone.ColliderType = colliderType;
			bone.RadiusScale = radiusScale;
			
			if (FindBone (parent) != null)
				bone.Parent = FindBone (parent);
			else if (name.StartsWith ("Left"))
				bone.Parent = FindBone ("Left " + parent);
			else if (name.StartsWith ("Right"))
				bone.Parent = FindBone ("Right "+ parent);		
			
			bone.Parent.Children.Add(bone);
			m_Bones.Add (bone);
		}
		
		private void BuildCapsules()
		{
			foreach (BoneData bone in m_Bones)
			{
				if (bone.ColliderType != typeof (CapsuleCollider))
					continue;
				
				int direction;
				float distance;
				if (bone.Children.Count == 1)
				{
					BoneData childBone = (BoneData)bone.Children[0];
					Vector3 endPoint = childBone.Anchor.position;
					CalculateDirection (bone.Anchor.InverseTransformPoint(endPoint), out direction, out distance);
				}
				else
				{
					Vector3 endPoint = (bone.Anchor.position - bone.Parent.Anchor.position) + bone.Anchor.position;
					CalculateDirection (bone.Anchor.InverseTransformPoint(endPoint), out direction, out distance);
					
					if (bone.Anchor.GetComponentsInChildren(typeof(Transform)).Length > 1)
					{
						Bounds bounds = new Bounds();
						foreach (Transform child in bone.Anchor.GetComponentsInChildren(typeof(Transform)))
							bounds.Encapsulate(bone.Anchor.InverseTransformPoint(child.position));
						
						if (distance > 0)
							distance = bounds.max[direction];
						else
							distance = bounds.min[direction];
					}
				}
				
				CapsuleCollider collider = Undo.AddComponent<CapsuleCollider>(bone.Anchor.gameObject);
				collider.direction = direction;
				
				Vector3 center = Vector3.zero;
				center[direction] = distance * 0.5F;
				collider.center = center;
				collider.height = Mathf.Abs (distance);
				collider.radius = Mathf.Abs (distance * bone.RadiusScale);
			}
		}
		
		private void Cleanup()
		{
			foreach (BoneData bone in m_Bones)
			{
				if (!bone.Anchor)
					continue;
				
				Component[] joints = bone.Anchor.GetComponentsInChildren(typeof(Joint));
				foreach (Joint joint in joints)
					Undo.DestroyObjectImmediate(joint);
				
				Component[] bodies = bone.Anchor.GetComponentsInChildren(typeof(Rigidbody));
				foreach (Rigidbody body in bodies)
					Undo.DestroyObjectImmediate(body);
				
				Component[] colliders = bone.Anchor.GetComponentsInChildren(typeof(Collider));
				foreach (Collider collider in colliders)
				{
					if(collider.transform != m_LeftFoot.transform && collider.transform != m_RightFoot)
						Undo.DestroyObjectImmediate(collider);
				}				
			}
		}
		
		private void BuildBodies()
		{
			foreach(BoneData bone in m_Bones)
			{
				Undo.AddComponent<Rigidbody>(bone.Anchor.gameObject);
				bone.Anchor.GetComponent<Rigidbody>().mass = bone.Density;
			}
		}
		
		private void BuildJoints()
		{
			foreach(BoneData bone in m_Bones)
			{
				if(bone.Parent == null)
					continue;
				
				CharacterJoint joint = Undo.AddComponent<CharacterJoint>(bone.Anchor.gameObject);
				bone.Joint = joint;

				joint.axis = CalculateDirectionAxis (bone.Anchor.InverseTransformDirection(bone.Axis));
				joint.swingAxis = CalculateDirectionAxis (bone.Anchor.InverseTransformDirection(bone.NormalAxis));
				joint.anchor = Vector3.zero;
				joint.connectedBody = bone.Parent.Anchor.GetComponent<Rigidbody>();
					
				SoftJointLimit limit = new SoftJointLimit ();
				
				limit.limit = bone.MinLimit;
				joint.lowTwistLimit = limit;
				
				limit.limit = bone.MaxLimit;
				joint.highTwistLimit = limit;
				
				limit.limit = bone.SwingLimit;
				joint.swing1Limit = limit;
				
				limit.limit = 0;
				joint.swing2Limit = limit;
	            joint.enableProjection = m_EnableProjection;
			}
		}
		
		private	void CalculateMassRecursively(BoneData bone)
		{
			float mass = bone.Anchor.GetComponent<Rigidbody>().mass;
			foreach (BoneData child in bone.Children)
			{
				CalculateMassRecursively (child);
				mass += child.TotalMass;
			}

			bone.TotalMass = mass;
		}
		
		private void CalculateMass()
		{
			CalculateMassRecursively (m_RootBone);

			float massScale = m_TotalMass / m_RootBone.TotalMass;
	        foreach (BoneData bone in m_Bones)
				bone.Anchor.GetComponent<Rigidbody>().mass *= massScale;
			
			CalculateMassRecursively(m_RootBone);
		}

		private static void CalculateDirection (Vector3 point, out int direction, out float distance)
		{
			direction = 0;
			if (Mathf.Abs(point[1]) > Mathf.Abs(point[0]))
				direction = 1;
			if (Mathf.Abs(point[2]) >Mathf.Abs(point[direction]))
				direction = 2;

			distance = point[direction];
		}
		
		private static Vector3 CalculateDirectionAxis(Vector3 point)
		{
			int direction = 0;
			float distance;
			CalculateDirection (point, out direction, out distance);
			Vector3 axis = Vector3.zero;
			if (distance > 0)
				axis[direction] = 1f;
			else
				axis[direction] = -1f;
			
			return axis;
		}
		
		private static int SmallestComponent(Vector3 point)
		{
			int direction = 0;
			if (Mathf.Abs(point[1]) < Mathf.Abs(point[0]))
				direction = 1;
			if (Mathf.Abs(point[2]) < Mathf.Abs(point[direction]))
				direction = 2;
			return direction;
		}
		
		private static int LargestComponent(Vector3 point)
		{
			int direction = 0;
			if (Mathf.Abs(point[1]) > Mathf.Abs(point[0]))
				direction = 1;
			if (Mathf.Abs(point[2]) > Mathf.Abs(point[direction]))
				direction = 2;
			return direction;
		}
		
		private static int SecondLargestComponent (Vector3 point)
		{
			int smallest = SmallestComponent (point);
			int largest = LargestComponent (point);
			if (smallest < largest)
			{
				int temp = largest;
				largest = smallest;
				smallest = temp;
			}
			
			if (smallest == 0 && largest == 1)
				return 2;
			else if (smallest == 0 && largest == 2)
				return 1;
			else
				return 0;
		}
		
		private Bounds Clip(Bounds bounds, Transform relativeTo, Transform clipTransform, bool below)
		{
			int axis = LargestComponent(bounds.size);
			
			if (Vector3.Dot (m_WorldUp, relativeTo.TransformPoint(bounds.max)) > Vector3.Dot (m_WorldUp, relativeTo.TransformPoint(bounds.min)) == below)
			{
				Vector3 min = bounds.min;
				min[axis] = relativeTo.InverseTransformPoint(clipTransform.position)[axis];
				bounds.min = min;
			}
			else
			{
				Vector3 max = bounds.max;
				max[axis] = relativeTo.InverseTransformPoint(clipTransform.position)[axis];
				bounds.max = max;
			}

			return bounds;
		}
		
		private Bounds GetBreastBounds (Transform relativeTo)
		{
			Bounds bounds = new Bounds ();
			bounds.Encapsulate (relativeTo.InverseTransformPoint (m_LeftHips.position));
			bounds.Encapsulate (relativeTo.InverseTransformPoint (m_RightHips.position));
			bounds.Encapsulate (relativeTo.InverseTransformPoint (m_LeftArm.position));
			bounds.Encapsulate (relativeTo.InverseTransformPoint (m_RightArm.position));
			Vector3 size = bounds.size;
			size[SmallestComponent (bounds.size)] = size[LargestComponent (bounds.size)] / 2.0F;
			bounds.size = size;

			return bounds;		
		}
		
		private void AddBreastColliders()
		{
			if (m_MiddleSpine != null && m_Root != null)
			{
				Bounds bounds;
				BoxCollider box;

				bounds = Clip (GetBreastBounds (m_Root), m_Root, m_MiddleSpine, false);
				box = Undo.AddComponent<BoxCollider>(m_Root.gameObject);
				box.center = bounds.center;
				box.size = bounds.size;
				
				bounds = Clip (GetBreastBounds (m_MiddleSpine), m_MiddleSpine, m_MiddleSpine, true);
				box = Undo.AddComponent<BoxCollider>(m_MiddleSpine.gameObject);
				box.center = bounds.center;
				box.size = bounds.size;
			}
			else
			{
				Bounds bounds = new Bounds ();
				bounds.Encapsulate (m_Root.InverseTransformPoint (m_LeftHips.position));
				bounds.Encapsulate (m_Root.InverseTransformPoint (m_RightHips.position));
				bounds.Encapsulate (m_Root.InverseTransformPoint (m_LeftArm.position));
				bounds.Encapsulate (m_Root.InverseTransformPoint (m_RightArm.position));
				
				Vector3 size = bounds.size;
				size[SmallestComponent (bounds.size)] = size[LargestComponent (bounds.size)] / 2.0F;

				BoxCollider box = Undo.AddComponent<BoxCollider>(m_Root.gameObject);
				box.center = bounds.center;
				box.size = size;
			}
		}
		
		private void AddHeadCollider()
		{
			if (m_Head.GetComponent<Collider>())
				Destroy (m_Head.GetComponent<Collider>());
			
			float radius = Vector3.Distance(m_RightArm.transform.position ,m_LeftArm.transform.position);
			radius /= 4f;

			SphereCollider sphere = Undo.AddComponent<SphereCollider>(m_Head.gameObject);
			sphere.radius = radius;
			Vector3 center = Vector3.zero;
			
			int direction;
			float distance;
			CalculateDirection (m_Head.InverseTransformPoint(m_Root.position), out direction, out distance);

			if (distance > 0)
				center[direction] = -radius;
			else
				center[direction] = radius;
			
			sphere.center = center;
		}	
	}
}