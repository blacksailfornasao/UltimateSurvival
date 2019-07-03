using System;
using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class FPObject : PlayerBehaviour
	{
		/// <summary> </summary>
		public Message Draw = new Message();

		/// <summary> </summary>
		public Message Holster = new Message();

		/// <summary> </summary>
		public bool	IsEnabled { get; private set; }

		/// <summary> </summary>
		public string ObjectName { get { return m_ObjectName; } }

		/// <summary> </summary>
		public SavableItem CorrespondingItem { get; private set; }

		/// <summary> </summary>
		public int TargetFOV { get { return m_TargetFOV; } }

		/// <summary> </summary>
		public float LastDrawTime { get; private set; }

		[Header("General")]

		[SerializeField] 
		private string m_ObjectName = "Unnamed";

		[SerializeField] 
		[Range(15, 100)]
		private int m_TargetFOV = 45;

		protected ItemProperty.Value m_Durability;


		protected virtual void Awake()
		{
			gameObject.SetActive(true);
			gameObject.SetActive(false);
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void On_Draw(SavableItem correspondingItem)
		{
			IsEnabled = true;
			CorrespondingItem = correspondingItem;
			LastDrawTime = Time.time;
			m_Durability = correspondingItem.GetPropertyValue("Durability");

			Draw.Send();
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void On_Holster()
		{
			IsEnabled = false;
			CorrespondingItem = null;

			Holster.Send();
		}
	}
}
