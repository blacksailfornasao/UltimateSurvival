using UnityEngine;

namespace UltimateSurvival
{
	public class LootOnDeath : EntityBehaviour 
	{
		public LootObject LootContainer { get { return m_LootContainer; } set { m_LootContainer = value; } }

		[SerializeField]
		private LootObject m_LootContainer;


		private void Start()
		{
			Entity.Death.AddListener(On_Death);
			m_LootContainer.enabled = false;
		}

		private void On_Death()
		{
			m_LootContainer.enabled = true;
		}
	}
}
