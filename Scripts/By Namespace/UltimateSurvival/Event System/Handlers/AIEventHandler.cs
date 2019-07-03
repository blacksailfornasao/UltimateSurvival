using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class AIEventHandler : EntityEventHandler 
	{
		/// <summary></summary>
		public Value<bool> IsHungry = new Value<bool>(false); 

		/// <summary></summary>
		public Value<float> LastFedTime = new Value<float>(0f);

		/// <summary></summary>
		public Activity Patrol = new Activity();

		/// <summary></summary>
		public Activity Chase = new Activity();

		/// <summary></summary>
		public Activity Attack = new Activity();

		/// <summary></summary>
		public Activity RunAway = new Activity();
	}
}
