using UnityEngine;

namespace UltimateSurvival
{
	public class ClampAttribute : PropertyAttribute
	{
		public readonly Vector2 ClampLimits;


		public ClampAttribute(float min, float max)
		{
			ClampLimits = new Vector2(min, max);
		}
	}
}