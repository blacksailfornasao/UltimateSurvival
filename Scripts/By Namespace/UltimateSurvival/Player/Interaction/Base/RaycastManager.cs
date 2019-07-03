using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// Sends a ray from the center of the camera, in the game world.
	/// It gathers data about what is in front of the player, and stores it in a variable.
	/// </summary>
	public class RaycastManager : PlayerBehaviour
	{
		[SerializeField] 
		private Camera m_WorldCamera;

		[SerializeField] 
		[Tooltip("The maximum distance at which you can interact with objects.")]
		private float m_RayLength = 1.5f;

		[SerializeField]
		[Tooltip("The distance at which an object is considered 'too close'.")]
		private float m_TooCloseThreeshold = 1f;

		[SerializeField]
		private LayerMask m_LayerMask;


		private void Start()
		{
			Player.InteractOnce.SetTryer(Try_InteractOnce);
			Player.Sleep.AddStartListener(()=> Player.RaycastData.Set(null));
		}

		private bool Try_InteractOnce()
		{
			var raycastData = Player.RaycastData.Get();
			if(raycastData)
			{
				if(raycastData.ObjectIsInteractable)
					raycastData.InteractableObject.OnInteract(Player);
			}

			return true;
		}

		private void Update()
		{
			var ray = m_WorldCamera.ViewportPointToRay(Vector2.one * 0.5f);
			RaycastHit hitInfo;
			var lastRaycastData = Player.RaycastData.Get();

			if(Physics.Raycast(ray, out hitInfo, m_RayLength, m_LayerMask, QueryTriggerInteraction.Ignore))
			{
				var raycastData = new RaycastData(hitInfo);
				Player.RaycastData.Set(raycastData);

				bool startedRaycastingOnObject =
					lastRaycastData && 
					raycastData.ObjectIsInteractable && 
					raycastData.InteractableObject != lastRaycastData.InteractableObject;
				
				if(startedRaycastingOnObject)
					raycastData.InteractableObject.OnRaycastStart(Player);
				else if(raycastData.ObjectIsInteractable)
					raycastData.InteractableObject.OnRaycastUpdate(Player);
			}
			else
			{
				// Let the object know the ray it's not on it anymore.
				if(lastRaycastData && lastRaycastData.ObjectIsInteractable)
					lastRaycastData.InteractableObject.OnRaycastEnd(Player);

				if(lastRaycastData != null)
					Player.RaycastData.Set(null);
			}

			bool isNearObject = (Player.RaycastData.Get() && Player.RaycastData.Get().HitInfo.distance < m_TooCloseThreeshold);
			Player.IsCloseToAnObject.Set(isNearObject);
		}
	}
}
