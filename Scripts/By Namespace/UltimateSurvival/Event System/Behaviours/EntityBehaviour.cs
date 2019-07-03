using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class EntityBehaviour : MonoBehaviour 
	{
		public EntityEventHandler Entity
		{
			get 
			{
				if(!m_Entity)
					m_Entity = GetComponent<EntityEventHandler>();
				if(!m_Entity)
					m_Entity = GetComponentInParent<EntityEventHandler>();
				
				return m_Entity;
			}
		}

		private EntityEventHandler m_Entity;
	}
}
