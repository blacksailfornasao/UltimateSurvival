using System;
using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class RayImpact
	{
		[Range(0f, 1000f)]
		[SerializeField]
		[Tooltip("The damage at close range.")]
		private float m_MaxDamage = 15f;

		[Range(0f, 1000f)]
		[SerializeField]
		[Tooltip("The impact impulse that will be transfered to the rigidbodies at contact.")]
		private float m_MaxImpulse = 15f;

		[SerializeField]
		[Tooltip("How damage and impulse lowers over distance.")]
		private AnimationCurve m_DistanceCurve = new AnimationCurve(
			new Keyframe(0f, 1f), 
			new Keyframe(0.8f, 0.5f), 
			new Keyframe(1f, 0f));


		/// <summary>
		/// 
		/// </summary>
		/// <param name="distance"></param>
		/// <param name="maxDistance"></param>
		public float GetDamageAtDistance(float distance, float maxDistance)
		{
			return ApplyCurveToValue(m_MaxDamage, distance, maxDistance);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>The impulse at distance.</returns>
		/// <param name="distance">Distance.</param>
		/// <param name="maxDistance">Max distance.</param>
		public float GetImpulseAtDistance(float distance, float maxDistance)
		{
			return ApplyCurveToValue(m_MaxImpulse, distance, maxDistance);
		}

		private float ApplyCurveToValue(float value, float distance, float maxDistance)
		{
			float maxDistanceAbsolute = Mathf.Abs(maxDistance);
			float distanceClamped = Mathf.Clamp(distance, 0f, maxDistanceAbsolute);

			return value * m_DistanceCurve.Evaluate(distanceClamped / maxDistanceAbsolute);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public abstract class FPWeaponBase : FPObject
	{
		/// <summary></summary>
		public Message Attack { get { return m_Attack; } }

		/// <summary>Can this weapon be used while too close to other objects? (e.g. a wall)</summary>
		public bool UseWhileNearObjects { get { return m_UseWhileNearObjects; } }

		/// <summary></summary>
		public bool CanBeUsed { get; set; }

		private Message m_Attack = new Message();

		[SerializeField]
		[Tooltip("Can this weapon be used while too close to other objects? (e.g. a wall)")]
		private bool m_UseWhileNearObjects = true;

		//private ItemProperty.Value m_AmmoProperty;


		public override void On_Draw(SavableItem correspondingItem)
		{
			base.On_Draw(correspondingItem);
			//m_AmmoProperty = correspondingItem.GetPropertyValue("Ammo Type");
		}

		public virtual bool TryAttackOnce(Camera camera) { return false; }

		public virtual bool TryAttackContinuously(Camera camera) { return false; }
	}
}
