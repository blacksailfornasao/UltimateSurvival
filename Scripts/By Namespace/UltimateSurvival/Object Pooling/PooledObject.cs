using System.Collections;
using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
    public class PooledObject : MonoBehaviour
    {
		/// <summary>This event is raised when the object has been released.</summary>
		public Message<PooledObject> Released = new Message<PooledObject>();

		[SerializeField]
		private bool m_ReleaseOnTimer = true;
		
		[SerializeField]
		private float m_ReleaseTimer = 20f;

		[SerializeField]
		private ParticleSystem[] m_ToResetParticles;

		//private Transform m_Container;
		private WaitForSeconds m_WaitInterval;


		/// <summary>
		/// 
		/// </summary>
		public virtual void OnUse(Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion), Transform parent = null)
		{
			try
			{
				gameObject.SetActive(true);

				transform.position = position;
				transform.rotation = rotation;

				if(transform.parent)
					transform.SetParent(parent);

				for(int i = 0;i < m_ToResetParticles.Length;i ++)
					m_ToResetParticles[i].Play(true);

				if(m_ReleaseOnTimer)
				{
					StopAllCoroutines();
					StartCoroutine(ReleaseWithDelay());
				}
			}
			// HACK: Bug when a pooled object (eg a bullet hole decal gets deleted while being a child of an object).
			catch{}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void OnRelease()
		{
			gameObject.SetActive(false);
			//transform.SetParent(m_Container);

			Released.Send(this);
		}

		private void Awake()
		{
			// TODO Bug with WaitForSecondsRealtime.
			if(m_ReleaseOnTimer)
				m_WaitInterval = new WaitForSeconds(m_ReleaseTimer);
		}

		private IEnumerator ReleaseWithDelay()
		{
			yield return m_WaitInterval;
			OnRelease();
		}
    }
}