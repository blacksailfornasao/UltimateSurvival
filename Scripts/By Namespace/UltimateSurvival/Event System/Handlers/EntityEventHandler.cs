using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class EntityEventHandler : MonoBehaviour
	{
		/// <summary></summary>
		public Value<float> Health = new Value<float>(100f);

		/// <summary> </summary>
		public Attempt<HealthEventData> ChangeHealth = new Attempt<HealthEventData>();

		/// <summary> </summary>
		public Value<bool> IsGrounded = new Value<bool>(true);

		/// <summary> </summary>
		public Value<Vector3> Velocity = new Value<Vector3>(Vector3.zero);

		/// <summary> </summary>
		public Message<float> Land = new Message<float>();

		/// <summary></summary>
		public Message Death = new Message();

		/// <summary></summary>
		public Message Respawn = new Message();
	}
}
