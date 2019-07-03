using UnityEngine;

namespace UltimateSurvival
{
	public class HelpboxAttribute : PropertyAttribute
	{
		public readonly string Message;


		public HelpboxAttribute(string message)
		{
			Message = message;
		}
	}
}
