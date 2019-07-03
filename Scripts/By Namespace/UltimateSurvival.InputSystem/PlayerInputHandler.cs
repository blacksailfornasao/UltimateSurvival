using System;
using UnityEngine;
using UltimateSurvival.InputSystem;

namespace UltimateSurvival
{
	/// <summary>
	/// Handles the player input, and feeds it to the other components.
	/// </summary>
	public class PlayerInputHandler : PlayerBehaviour
	{
		[SerializeField]
		private bool m_ShowControls;

		private InputManager m_Input;


		private void Awake()
		{
			if(GameController.InputManager)
				m_Input = GameController.InputManager;
			else
				enabled = false;

			Player.PlaceObject.AddListener(OnSucceded_PlaceObject);
		}

		private void OnGUI()
		{
			if(!m_ShowControls)
				return;
			
			Rect rect = new Rect(Screen.width - 8f - 150f, 32f, 150f, 20f);

			GUI.Label(rect, "Unlock cursor - Escape");

			rect.y = rect.yMax + 4f;
			GUI.Label(rect, "Inventory - TAB");

			rect.y = rect.yMax + 4f;
			GUI.Label(rect, "Move - WASD");

			rect.y = rect.yMax + 4f;
			GUI.Label(rect, "Attack - Left Click");

			rect.y = rect.yMax + 4f;
			GUI.Label(rect, "Aim - Right Click");

			rect.y = rect.yMax + 4f;
			GUI.Label(rect, "Crouch - C");

			rect.y = rect.yMax + 4f;
			GUI.Label(rect, "Run - Left Shift");

			rect.y = rect.yMax + 4f;
			GUI.Label(rect, "Interact - E");

			rect.y = rect.yMax + 4f;
			GUI.Label(rect, "Rotate - Arrows");

			rect.y = rect.yMax + 4f;
			GUI.Label(rect, "Wheel - Right Click");

			rect.y = rect.yMax + 4f;
			if(GUI.Button(rect, "Quit!"))
				Application.Quit();
		}
			
		private void Update()
		{
			// Inventory.
			if(!InventoryController.Instance.IsClosed && m_Input.GetButtonDown("Close Inventory"))
				InventoryController.Instance.SetState.Try(ET.InventoryState.Closed);
			if(InventoryController.Instance.IsClosed && m_Input.GetButtonDown("Open Inventory"))
				InventoryController.Instance.SetState.Try(ET.InventoryState.Normal);

			var rotateObjectAxis = m_Input.GetAxisRaw("Rotate Object");
			if(Mathf.Abs(rotateObjectAxis) > 0f)
				Player.RotateObject.Try(rotateObjectAxis);

			if(m_Input.GetButtonDown("Select Buildable"))
			{
				if(!Player.SelectBuildable.Active)
					Player.SelectBuildable.TryStart();
				else
					Player.SelectBuildable.TryStop();
			}

			if(InventoryController.Instance.IsClosed && Player.ViewLocked.Is(false))
			{
				// Movement.
				Vector2 moveInput = new Vector2(m_Input.GetAxis("Horizontal"), m_Input.GetAxis("Vertical"));
				Player.MovementInput.Set(moveInput);

				// Look.
				Player.LookInput.Set(new Vector2(m_Input.GetAxisRaw("Mouse X"), m_Input.GetAxisRaw("Mouse Y")));

				// Interact once.
				if(m_Input.GetButtonDown("Interact"))
					Player.InteractOnce.Try();

				// Interact continuously.
				Player.InteractContinuously.Set(m_Input.GetButton("Interact"));

				// Jump.
				if(m_Input.GetButtonDown("Jump"))
					Player.Jump.TryStart();

				// Run.
				bool sprintButtonHeld = m_Input.GetButton("Run");
				bool canStartSprinting = Player.IsGrounded.Get() && Player.MovementInput.Get().y > 0f;

				if(!Player.Run.Active && sprintButtonHeld && canStartSprinting)
					Player.Run.TryStart();
				
				if(Player.Run.Active && !sprintButtonHeld)
					Player.Run.ForceStop();
				
				if(m_Input.GetButtonDown("Crouch"))
				{
					if(!Player.Crouch.Active)
						Player.Crouch.TryStart();
					else
						Player.Crouch.TryStop();
				}

				// Attack.
				if(m_Input.GetButton("Attack"))
					Player.AttackContinuously.Try();
				
				if(m_Input.GetButtonDown("Attack"))
					Player.AttackOnce.Try();

				// Aim.
				if(m_Input.GetButtonDown("Aim"))
					Player.Aim.TryStart();
				else if(m_Input.GetButtonUp("Aim"))
					Player.Aim.ForceStop();
					
				if(m_Input.GetButtonDown("Place Object"))
				{
					if(Player.CanShowObjectPreview.Is(true))
						Player.PlaceObject.Try();
				}
				else
					Player.CanShowObjectPreview.Set(true);
			}
			else
			{
				// Movement.
				Player.MovementInput.Set(Vector2.zero);

				// Look.
				Player.LookInput.Set(Vector2.zero);
			}

			var scrollWheelValue = m_Input.GetAxis("Mouse ScrollWheel");
			Player.ScrollValue.Set(scrollWheelValue);
		}

		private void OnSucceded_PlaceObject()
		{
			Player.CanShowObjectPreview.Set(false);
		}
	}
}
