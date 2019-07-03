using System;
using UnityEngine;

namespace UltimateSurvival.Building
{
	[Serializable]
	public class StabilityPoint
	{
		[SerializeField]
		private string m_Name;

		[SerializeField]
		private Vector3 m_Position;

		[SerializeField]
		private Vector3 m_Direction = Vector3.down;

		[SerializeField]
		[ClampAttribute(0f, 10f)]
		private float m_Distance = 0.2f;


		public StabilityPoint GetClone()
		{
			return (StabilityPoint)MemberwiseClone();
		}

		/// <summary>
		/// Is this point stable?
		/// </summary>
		public bool IsStable(BuildingPiece piece, LayerMask mask)
		{
			var hits = Physics.RaycastAll(piece.transform.position + piece.transform.TransformVector(m_Position), piece.transform.TransformDirection(m_Direction), m_Distance, mask, QueryTriggerInteraction.Ignore);
			for(int i = 0;i < hits.Length;i ++)
			{
				if(!piece.HasCollider(hits[i].collider))
					return true;
			}

			return false;
		}

		public void OnDrawGizmosSelected(BuildingPiece piece)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawRay(piece.transform.position + piece.transform.TransformVector(m_Position), piece.transform.TransformDirection(m_Direction).normalized * m_Distance);
		}
	}
}