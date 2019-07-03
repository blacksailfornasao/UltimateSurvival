using UnityEngine;

namespace UltimateSurvival
{
	public class DamageArea : MonoBehaviour 
	{
		public bool Active { get; set; }

		[SerializeField]
		private Vector2 m_DamagePerSecond = new Vector2(3f, 5f);


		private void OnTriggerStay(Collider other)
		{
			if(!Active)
				return;

			var entity = other.GetComponent<EntityEventHandler>();

			if(entity)
			{
				var healthEventData = new HealthEventData(-Random.Range(m_DamagePerSecond.x, m_DamagePerSecond.y) * Time.deltaTime);
				entity.ChangeHealth.Try(healthEventData);
			}
		}
	}
}
