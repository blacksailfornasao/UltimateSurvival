using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// Displays an object's pivot as a solid sphere.
	/// </summary>
	public class PivotDisplayer : MonoBehaviour
	{
		[SerializeField] 
		private Color m_Color = Color.red;

		[SerializeField] 
		private float m_Radius = 0.06f;
		
		
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = m_Color;
			Gizmos.DrawSphere(transform.position, m_Radius);
		}
	}
}
