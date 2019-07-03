using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival
{
	public class DetailSpawnData : ScriptableObject 
	{
		public DetailSpawn[] m_TreeList;

		public DetailSpawn[] m_RockList;


		public DetailSpawn GetTreePrefab()
		{
			int choice = ProbabilityUtils.RandomChoiceFollowingDistribution(GetSpawnProbabilities(m_TreeList));
			return m_TreeList[choice];
		}

		public DetailSpawn GetRockPrefab()
		{
			int choice = ProbabilityUtils.RandomChoiceFollowingDistribution(GetSpawnProbabilities(m_RockList));
			return m_RockList[choice];
		}

		private List<float> GetSpawnProbabilities(DetailSpawn[] array)
		{
			List<float> probabilities = new List<float>();
			for(int i = 0;i < array.Length;i ++)
				probabilities.Add(array[i].SpawnProbability);

			return probabilities;
		}

		[System.Serializable]
		public class DetailSpawn
		{
			public GameObject Object;

			[Range(0, 100)]
			public int SpawnProbability;

			public Vector3 PositionOffset;

			public Vector3 RandomRotation;
		}
	}
}
