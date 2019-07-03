using UnityEngine;
using System.Collections.Generic;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class ProbabilityUtils : MonoBehaviour 
	{
		/// <summary>
		/// Return an index randomly chosen following the distribution specified by a list of probabilities
		/// </summary>
		/// <returns>
		/// An index in range [0, probabilities.Length) following the distribution specified in probabilites.
		/// </returns>
		/// <param name="probabilities">
		/// A list of probabilities from which to choose an index. All values must be >= 0!
		/// </param>
		public static int RandomChoiceFollowingDistribution(List<float> probabilities) 
		{
			// Sum to create CDF:
			float[] cdf = new float[probabilities.Count];
			float sum = 0;

			for (int i = 0; i < probabilities.Count; ++i) 
			{
				cdf[i] = sum + probabilities[i];
				sum = cdf[i];
			}

			// Choose from CDF:
			float cdf_value = Random.Range(0f, cdf[probabilities.Count - 1]);
			int index = System.Array.BinarySearch(cdf, cdf_value);

			if (index < 0)
				// If not found (probably won't be) BinarySearch returns bitwise complement of next-highest index.
				index = ~index;	

			return index;
		}
	}
}
