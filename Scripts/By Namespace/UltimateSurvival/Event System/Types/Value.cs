using System;
using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// This allows to have a callback when the value changes (An example would be updating the GUI when the player health changes).
	/// </summary>
	public class Value<T>
	{
		public delegate T Filter(T lastValue, T newValue);

		private Action m_Set;
		private Filter m_Filter;
		private T m_CurrentValue;
		private T m_LastValue;


		/// <summary>
		/// 
		/// </summary>
		public Value(T initialValue)
		{
			m_CurrentValue = initialValue;
			m_LastValue = m_CurrentValue;
		}

		public bool Is(T value)
		{
			return m_CurrentValue != null && m_CurrentValue.Equals(value);
		}

		/// <summary>
		/// When this value will change, the callback method will be called.
		/// </summary>
		public void AddChangeListener(Action callback)
		{
			m_Set += callback;
		}

		/// <summary>
		/// 
		/// </summary>
		public void RemoveChangeListener(Action callback)
		{
			m_Set -= callback;
		}

		/// <summary>
		/// A "filter" will be called before the regular callbacks, useful for clamping values (like the player health, etc).
		/// </summary>
		public void SetFilter(Filter filter)
		{
			m_Filter = filter;
		}

		/// <summary>
		/// 
		/// </summary>
		public T Get()
		{
			return m_CurrentValue;
		}

		/// <summary>
		/// 
		/// </summary>
		public T GetLastValue()
		{
			return m_LastValue;
		}

		/// <summary>
		/// 
		/// </summary>
		public void Set(T value)
		{
			m_LastValue = m_CurrentValue;
			m_CurrentValue = value;

			if(m_Filter != null)
				m_CurrentValue = m_Filter(m_LastValue, m_CurrentValue);
			
			if(m_Set != null && (m_LastValue == null || !m_LastValue.Equals(m_CurrentValue)))
				m_Set();
		}

		/// <summary>
		/// 
		/// </summary>
		public void SetAndForceUpdate(T value)
		{
			m_LastValue = m_CurrentValue;
			m_CurrentValue = value;

			if(m_Filter != null)
				m_CurrentValue = m_Filter(m_LastValue, m_CurrentValue);

			if(m_Set != null)
				m_Set();
		}

		public void SetAndDontUpdate(T value)
		{
			m_LastValue = m_CurrentValue;
			m_CurrentValue = value;

			if(m_Filter != null)
				m_CurrentValue = m_Filter(m_LastValue, m_CurrentValue);
		}
	}
}