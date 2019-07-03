using System;
using UnityEngine;

namespace UltimateSurvival
{
	public delegate bool TryerDelegate();

	/// <summary>
	/// 
	/// </summary>
	public class Attempt
	{
		private TryerDelegate m_Tryer;
		private Action m_Listeners;


		/// <summary>
		/// Registers a method that will try to execute this action.
		/// NOTE: Only 1 tryer is allowed!
		/// </summary>
		public void SetTryer(TryerDelegate tryer)
		{
			m_Tryer = tryer;
		}

		/// <summary>
		/// 
		/// </summary>
		public void AddListener(Action listener)
		{
			m_Listeners += listener;
		}

		/// <summary>
		/// 
		/// </summary>
		public void RemoveListener(Action listener)
		{
			m_Listeners -= listener;
		}

		/// <summary>
		/// 
		/// </summary>
		public bool Try()
		{
			bool wasSuccessful = (m_Tryer == null || m_Tryer());
			if(wasSuccessful)
			{
				if(m_Listeners != null)
					m_Listeners();
				return true;
			}

			return false;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class Attempt<T>
	{
		public delegate bool GenericTryerDelegate(T arg);

		GenericTryerDelegate m_Tryer;
		Action<T> m_Listeners;


		/// <summary>
		/// Registers a method that will try to execute this action.
		/// NOTE: Only 1 tryer is allowed!
		/// </summary>
		public void SetTryer(GenericTryerDelegate tryer)
		{
			m_Tryer = tryer;
		}

		/// <summary>
		/// 
		/// </summary>
		public void AddListener(Action<T> listener)
		{
			m_Listeners += listener;
		}

		/// <summary>
		/// 
		/// </summary>
		public void RemoveListener(Action<T> listener)
		{
			m_Listeners -= listener;
		}

		/// <summary>
		/// 
		/// </summary>
		public bool Try(T arg)
		{
			bool succeeded = m_Tryer != null && m_Tryer(arg);
			if(succeeded)
			{
				if(m_Listeners != null)
					m_Listeners(arg);
				return true;
			}

			return false;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class Attempt<T, V>
	{
		public delegate bool GenericTryerDelegate(T arg1, V arg2);

		private GenericTryerDelegate m_Tryer;
		private Action<T, V> m_Listeners;


		/// <summary>
		/// Registers a method that will try to execute this action.
		/// NOTE: Only 1 tryer is allowed!
		/// </summary>
		public void SetTryer(GenericTryerDelegate tryer)
		{
			m_Tryer = tryer;
		}

		/// <summary>
		/// 
		/// </summary>
		public void AddListener(Action<T, V> listener)
		{
			m_Listeners += listener;
		}

		/// <summary>
		/// 
		/// </summary>
		public void RemoveListener(Action<T, V> listener)
		{
			m_Listeners -= listener;
		}

		/// <summary>
		/// 
		/// </summary>
		public bool Try(T arg1, V arg2)
		{
			bool succeeded = m_Tryer != null && m_Tryer(arg1, arg2);
			if(succeeded)
			{
				if(m_Listeners != null)
					m_Listeners(arg1, arg2);
				return true;
			}

			return false;
		}
	}
}