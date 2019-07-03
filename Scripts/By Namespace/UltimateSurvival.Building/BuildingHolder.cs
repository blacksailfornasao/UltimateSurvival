using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival.Building
{
	public class BuildingHolder : MonoBehaviour 
	{
		private List<BuildingPiece> m_Pieces = new List<BuildingPiece>();


		public bool HasCollider(Collider col)
		{
			for(int i = 0;i < m_Pieces.Count;i ++)
				if(m_Pieces[i].HasCollider(col))
					return true;

			return false;
		}

		public void AddPiece(BuildingPiece piece)
		{
			if(!m_Pieces.Contains(piece))
			{
				m_Pieces.Add(piece); 
				piece.GetComponent<EntityEventHandler>().Death.AddListener(()=> OnPieceDeath(piece));
			}
		}

		private void OnPieceDeath(BuildingPiece piece)
		{
			m_Pieces.Remove(piece);

			for(int p = 0;p < m_Pieces.Count;p ++)
			{
				for(int s = 0;s < m_Pieces[p].Sockets.Length;s ++)
					m_Pieces[p].Sockets[s].OnPieceDeath(piece);
			}
		}
	}
}
