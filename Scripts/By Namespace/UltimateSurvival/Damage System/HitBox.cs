using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// Will register damage events from outside and pass them to the parent entity.
	/// </summary>
	[RequireComponent(typeof(Collider))]
	[RequireComponent(typeof(Rigidbody))]
	public class HitBox : MonoBehaviour, IDamageable 
	{
		public Collider Collider { get { return m_Collider; } }

		[SerializeField]
		[Clamp(0f, Mathf.Infinity)]
		private float m_DamageMultiplier = 1f;

		private Collider m_Collider;
		private Rigidbody m_Rigidbody;
		private EntityEventHandler m_ParentEntity;


		public void ReceiveDamage(HealthEventData damageData)
		{
			if(enabled)
			{
				if(m_ParentEntity.Health.Get() > 0f)
				{
					damageData.Delta *= m_DamageMultiplier;
					m_ParentEntity.ChangeHealth.Try(damageData);
				}
				
				if(m_ParentEntity.Health.Get() == 0f)
					m_Rigidbody.AddForceAtPosition(damageData.HitDirection * damageData.HitImpulse, damageData.HitPoint, ForceMode.Impulse);
			}
		}

		private void Awake()
		{
			m_ParentEntity = GetComponentInParent<EntityEventHandler>();
			if(!m_ParentEntity)
			{
				Debug.LogErrorFormat(this, "[This HitBox is not under an entity, like a player, animal, etc, it has no purpose.", name);
				enabled = false;
				return;
			}

			m_Collider = GetComponent<Collider>();
			m_Rigidbody = GetComponent<Rigidbody>();
		}
	}
}
