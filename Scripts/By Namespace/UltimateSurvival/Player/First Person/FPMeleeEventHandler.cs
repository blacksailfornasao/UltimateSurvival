using System;
using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class FPMeleeEventHandler : MonoBehaviour
	{
		/// <summary>Raised when the "On_Hit" callback is called by Mecanim (visually, it should correspond with a tool / melee weapon hitting an object).</summary>
		public Message Hit = new Message();

		/// <summary>Raised when the "On_Miss" callback is called by Mecanim.</summary>
		public Message Woosh = new Message();


		/// <summary>
		/// Called by Mecanim as a callback for an animation event.
		/// </summary>
		public void On_Hit()
		{
			Hit.Send();
		}

		/// <summary>
		/// Called by Mecanim as a callback for an animation event.
		/// </summary>
		public void On_Woosh()
		{
			Woosh.Send();
		}
	}
}
