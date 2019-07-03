using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class Ragdoll : MonoBehaviour 
	{
		[System.Serializable]
		public class BoneToReparent
		{
			[SerializeField]
			private Transform m_Bone;

			[SerializeField]
			private Transform m_NewParent;


			public void Reparent()
			{
				m_Bone.SetParent(m_NewParent, true);
			}
		}

		[SerializeField]
		private Rigidbody m_Pelvis;

		[SerializeField]
		private string m_NormalLayer = "Hitbox";

		[SerializeField]
		private string m_RagdollLayer = "Default";

		[Header("Helpers")]

		[SerializeField]
		private BoneToReparent[] m_BonesToReparent;

		[SerializeField]
		private Texture m_SurfaceTexture;

		[SerializeField]
		private bool m_AutoAssignHitboxes = true;

		private List<Rigidbody> m_Bones = new List<Rigidbody>();


		public void Enable()
		{
			foreach(var bone in m_Bones)
			{
				bone.isKinematic = false;
				bone.gameObject.layer = LayerMask.NameToLayer(m_RagdollLayer);
			}

			foreach(var bone in m_BonesToReparent)
				bone.Reparent();
		}

		public void Disable()
		{
			foreach(var bone in m_Bones)
			{
				bone.isKinematic = true;
				bone.gameObject.layer = LayerMask.NameToLayer(m_NormalLayer);
			}
		}

		private void Awake()
		{
			m_Bones = GetComponentsInChildren<CharacterJoint>().Select(joint=> joint.GetComponent<Rigidbody>()).ToList();
			m_Bones.Add(m_Pelvis);
			Disable();

			foreach(var bone in m_Bones)
			{
				if(m_AutoAssignHitboxes && bone.gameObject.GetComponent<HitBox>() == null)
					bone.gameObject.AddComponent<HitBox>();

				if(m_SurfaceTexture && bone.gameObject.GetComponent<SurfaceIdentity>() == null)
				{
					var si = bone.gameObject.AddComponent<SurfaceIdentity>();
					si.Texture = m_SurfaceTexture;
				}
			}
		}
	}
}
