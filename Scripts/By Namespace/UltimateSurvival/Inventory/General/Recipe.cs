using System;
using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class RequiredItem
	{
		public string Name { get { return m_Name; } }

		public int Amount { get { return m_Amount; } }

		[SerializeField]
		private string m_Name;

		[SerializeField]
		private int m_Amount;
	}

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class Recipe
	{
		public int Duration { get { return m_Duration; } }

		public RequiredItem[] RequiredItems { get { return m_RequiredItems; } }

		[SerializeField]
		[Range(1, 999)]
		private int	m_Duration = 1;

		[SerializeField]
		private RequiredItem[] m_RequiredItems;


		public static implicit operator bool(Recipe recipe)
		{
			return recipe != null;
		}
	}
}
