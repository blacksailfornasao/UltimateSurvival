using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class RaycastData
	{
		/// <summary> </summary>
		public bool ObjectIsInteractable { get; private set; }

		/// <summary> </summary>
		public GameObject GameObject { get; private set; }

		/// <summary> </summary>
		public InteractableObject InteractableObject { get; private set; }

		/// <summary> </summary>
		public RaycastHit HitInfo { get; private set; }


		public static implicit operator bool(RaycastData raycastData)
		{
			return !ReferenceEquals(raycastData, null);
		}

		/// <summary>
		/// 
		/// </summary>
		public RaycastData(RaycastHit hitInfo)
		{
			GameObject = hitInfo.collider.gameObject;
			InteractableObject = hitInfo.collider.GetComponent<InteractableObject>();
			if(InteractableObject && !InteractableObject.enabled)
				InteractableObject = null;

			ObjectIsInteractable = (InteractableObject != null);
			HitInfo = hitInfo;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class InteractableObject : MonoBehaviour
	{
		/// <summary>
		/// 
		/// </summary>
		public virtual void OnRaycastStart(PlayerEventHandler player) { }

		/// <summary>
		/// 
		/// </summary>
		public virtual void OnRaycastUpdate(PlayerEventHandler player) { }

		/// <summary>
		/// 
		/// </summary>
		public virtual void OnRaycastEnd(PlayerEventHandler player) { }

		/// <summary>
		/// 
		/// </summary>
		public virtual void OnInteract(PlayerEventHandler player) { }

		/// <summary>
		/// 
		/// </summary>
        public virtual void OnInteractHold(PlayerEventHandler player) { }
	}
}