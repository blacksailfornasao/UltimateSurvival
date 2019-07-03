using System.Collections;
using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	[RequireComponent (typeof (CharacterController))]
	public class CCDrivenController : PlayerBehaviour
	{
		public enum JumpedFrom { Ground, Ladder }

		[Header("General")]

		[SerializeField] 
		[Tooltip("How fast the player will change direction / accelerate.")]
		private float m_Acceleration = 5f;

		[SerializeField] 
		[Tooltip("How fast the player will stop if no input is given (applies only when grounded).")]
		private float m_Damping = 8f;

		[SerializeField] 
		[Range(0f, 1f)]
		[Tooltip("How well the player can control direction while in air.")]
		private float m_AirControl = 0.15f;

		[SerializeField] 
		private float m_ForwardSpeed = 4f;

		[SerializeField] 
		private float m_SidewaysSpeed = 3.5f;

		[SerializeField] 
		private float m_BackwardSpeed = 3f;

		[SerializeField]
		[Tooltip("Curve for multiplying speed based on slope.")]
		private AnimationCurve m_SlopeMultiplier = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

		[SerializeField]
		[Tooltip("A small number will make the player bump when descending slopes, a larger one will make it stick to the surface.")]
		private float m_AntiBumpFactor = 1f;

		[Header("Running")]

		[SerializeField]
		[Tooltip("Can the player run?")]
		private bool m_EnableRunning = true;

		[SerializeField] 
		[Tooltip("The current movement speed will be multiplied by this value, when sprinting.")]
		private float m_RunSpeedMultiplier = 1.8f;

		[Header("Jumping")]

		[SerializeField]
		[Tooltip("Can the player jump?")]
		private bool m_EnableJumping = true;

		[SerializeField] 
		[Tooltip("How high do we jump when pressing jump and letting go immediately.")]
		private float m_JumpHeight = 1f;

		[SerializeField]
		private float m_JumpSpeedFromLadder = 5f;

		[Header("Crouching")]

		[SerializeField]
		[Tooltip("Can the player crouch?")]
		private bool m_EnableCrouching = true;

		[SerializeField] 
		[Tooltip("The current movement speed will be multiplied by this value, when moving crouched.")]
		private float m_CrouchSpeedMultiplier = 0.7f;

		[SerializeField] 
		[Tooltip("The CharacterController's height when fully-crouched.")]
		private float m_CrouchHeight = 1f;

		[SerializeField] 
		[Tooltip("How much time it takes to go in and out of crouch-mode.")]
		private float m_CrouchDuration = 0.3f;

		[Header("Ladder Climbing")]

		[SerializeField]
		[Tooltip("How fast the character moves on ladder.")]
		private float m_SpeedOnLadder = 1f;

		[Header("Sliding")]

		[SerializeField]
		[Tooltip("Will the player slide on steep surfaces?")]
		private bool m_EnableSliding = true;

		[SerializeField]
		private float m_SlideLimit = 32f;

		[Tooltip("How fast does the character slide on steep surfaces?")]
		[SerializeField] 
		private float m_SlidingSpeed = 15f;

		[Header("Physics")]

		[SerializeField]
		private float m_PushForce = 60f;

		[SerializeField] 
		[Tooltip("How fast we accelerate into falling.")]
		private float m_Gravity = 20f;

		private CharacterController m_Controller;
		private float m_DesiredSpeed;
		private Vector3 m_CurrentVelocity;
		private Vector3 m_SlideVelocity;
		private Vector3 m_DesiredVelocity;
		private Vector3 m_LastSurfaceNormal;
		private CollisionFlags m_LastCollisionFlags;
		private float m_UncrouchedHeight;
		private bool m_PreviouslyGrounded;
		private float m_LastTimeToggledCrouching;
		private JumpedFrom m_JumpedFrom;


		private void Start()
		{
			m_Controller = GetComponent<CharacterController>();
			m_UncrouchedHeight = m_Controller.height;

			Player.Jump.AddStartTryer(TryStart_Jump);
			Player.Run.AddStartTryer(TryStart_Run);
			Player.Crouch.AddStartTryer(TryStart_Crouch);
			Player.Crouch.AddStopTryer(TryStop_Crouch);
			Player.Sleep.AddStopListener(()=> m_CurrentVelocity = Vector3.zero);
		}

		private void Update()
		{
			// Move the controller.
			m_LastCollisionFlags = m_Controller.Move(m_CurrentVelocity * Time.deltaTime);

			if((m_LastCollisionFlags & CollisionFlags.Below) == CollisionFlags.Below && !m_PreviouslyGrounded)
			{
				if(Player.Jump.Active)
					Player.Jump.ForceStop();

				Player.Land.Send(Mathf.Abs(Player.Velocity.Get().y));
			}

			Player.IsGrounded.Set(m_Controller.isGrounded);
			Player.Velocity.Set(m_Controller.velocity);

			// If the character is on a ladder, move it accordingly.
			if(Player.NearLadders.Count > 0)
			{
				if(Player.Walk.Active)
					Player.Walk.ForceStop();

				if(Player.Run.Active)
					Player.Run.ForceStop();

				if(Player.Crouch.Active)
					Player.Crouch.TryStop();

				if(Player.Jump.Active && m_JumpedFrom == JumpedFrom.Ground)
					Player.Jump.ForceStop();

				UpdateLadderMovement();
			}
			else 
			{
				if(!m_Controller.isGrounded)
				{
					if(Player.Walk.Active)
						Player.Walk.ForceStop();

					if(Player.Run.Active)
						Player.Run.ForceStop();

					UpdateFalling();
				}
				else if(!Player.Jump.Active)
					UpdateMovement();
			}

			m_PreviouslyGrounded = m_Controller.isGrounded;
		}

		private void UpdateLadderMovement()
		{
			// Clamp the input vector's magnitude to 1; If we don't do this, when moving diagonally, the speed will be ~1.4x higher.
			Vector2 movementInputClamped = Vector2.ClampMagnitude(Player.MovementInput.Get(), 1f);
	
			if(InventoryController.Instance.IsClosed)
			{
				var currentLadder = Player.NearLadders.Peek();
				var lookDirection = Player.LookDirection.Get();

				bool facingAwayFromLadder = Vector3.Dot(currentLadder.forward, lookDirection) < 0f;

				bool wantsToGetOffTheLadder = (movementInputClamped.y < 0f && !facingAwayFromLadder) || (movementInputClamped.y > 0f && facingAwayFromLadder);

				if(Player.IsGrounded.Get() && wantsToGetOffTheLadder)
					m_DesiredVelocity = transform.forward * movementInputClamped.y * 3f;
				else
				{
					Vector3 leftRightVelocity = currentLadder.right * movementInputClamped.x * m_SpeedOnLadder / 2f;
					if(facingAwayFromLadder)
						leftRightVelocity *= -1f;

					Vector3 directionalVelocity = lookDirection * movementInputClamped.y * m_SpeedOnLadder;

					// Calculate the desired velocity on the ladder's plane.
					Vector3 velocity = leftRightVelocity + directionalVelocity;

					Vector3 fromDirection = Vector3.ProjectOnPlane(velocity, currentLadder.right);
					Vector3 toDirection = currentLadder.up * Mathf.Sign(velocity.y);

					// Rotate the velocity vector to either point straight up, or straight down, depending on the direction the player is looking.
					velocity = Quaternion.FromToRotation(fromDirection, toDirection) * velocity;

					// And add a little bit of push towards the ladder.
					if(movementInputClamped.sqrMagnitude > 0f)
						velocity += currentLadder.forward;

					m_DesiredVelocity = velocity;
				}
			}
			else
				m_DesiredVelocity = Vector3.zero;

			// Calculate the rate at which the current speed should increase / decrease. 
			// If the player doesn't press any movement button, use the "m_Damping" value, otherwise use "m_Acceleration".
			float targetAccel = m_DesiredVelocity.sqrMagnitude > 0f ? m_Acceleration : m_Damping;

			m_CurrentVelocity = Vector3.Lerp(m_CurrentVelocity, m_DesiredVelocity, targetAccel * Time.deltaTime); 
		}
			
		private void UpdateMovement()
		{
			CalculateDesiredVelocity();

			bool inventoryIsClosed = InventoryController.Instance.IsClosed;

			// If the inventory is open, the target velocity should be 0, so the character stops.
			Vector3 targetVelocity = inventoryIsClosed ? m_DesiredVelocity : Vector3.zero;

			// Make sure to lower the speed when moving on steep surfaces.
			float surfaceAngle = Vector3.Angle(Vector3.up, m_LastSurfaceNormal);
			targetVelocity *= m_SlopeMultiplier.Evaluate(surfaceAngle / m_Controller.slopeLimit);

			// Calculate the rate at which the current speed should increase / decrease. 
			// If the player doesn't press any movement button, use the "m_Damping" value, otherwise use "m_Acceleration".
			float targetAccel = targetVelocity.sqrMagnitude > 0f ? m_Acceleration : m_Damping;

			m_CurrentVelocity = Vector3.Lerp(m_CurrentVelocity, targetVelocity, targetAccel * Time.deltaTime); 

			// If we're moving and not running, start the "Walk" activity.
			if(!Player.Walk.Active && targetVelocity.sqrMagnitude > 0.1f && !Player.Run.Active)
				Player.Walk.ForceStart();
			// If we're sprinting, or not moving, stop the "Walk" activity.
			else if(Player.Walk.Active && (targetVelocity.sqrMagnitude < 0.1f || Player.Run.Active))
				Player.Walk.ForceStop();

			if(Player.Run.Active)
			{
				if(Player.Stamina.Is(0f))
					Player.Run.ForceStop();
				else
				{
					bool wantsToMoveBackwards = Player.MovementInput.Get().y < 0f;
					bool runShouldStop = wantsToMoveBackwards  || m_DesiredSpeed == 0f;
					if(runShouldStop)
						Player.Run.ForceStop();
				}
			}

			if(m_EnableSliding)
			{
				// Sliding...
				if(surfaceAngle > m_SlideLimit)
				{
					Vector3 slideDirection = (m_LastSurfaceNormal + Vector3.down);
					m_SlideVelocity += slideDirection * m_SlidingSpeed * Time.deltaTime;
				}
				else
					m_SlideVelocity = Vector3.Lerp(m_SlideVelocity, Vector3.zero, Time.deltaTime * 10f);

				m_CurrentVelocity += m_SlideVelocity;
			}
				
			// Apply a "force" downwards, so we stick to the surface below.
			if(!Player.Jump.Active)
				m_CurrentVelocity.y = -m_AntiBumpFactor;
		}

		private void UpdateFalling()
		{
			if(m_PreviouslyGrounded && !Player.Jump.Active)
				m_CurrentVelocity.y = 0f;

			// Modify the current velocity by taking into account how well we can change direction when not grounded (see "m_AirControl" tooltip).
			m_CurrentVelocity += m_DesiredVelocity * m_Acceleration * m_AirControl * Time.deltaTime;

			// Apply gravity.
			m_CurrentVelocity.y -= m_Gravity * Time.deltaTime;
		}

		private void CalculateDesiredVelocity()
		{
			// Clamp the input vector's magnitude to 1; If we don't do this, when moving diagonally, the speed will be ~1.4x higher.
			Vector2 movementInputClamped = Vector2.ClampMagnitude(Player.MovementInput.Get(), 1f);

			// Has the player pressed any movement button?
			bool wantsToMove = movementInputClamped.sqrMagnitude > 0f;

			// Calculate the direction (relative to the us), in which the player wants to move.
			Vector3 targetDirection = (wantsToMove ? transform.TransformDirection(new Vector3(movementInputClamped.x, 0f, movementInputClamped.y)) : m_Controller.velocity.normalized);

			m_DesiredSpeed = 0f;

			if(wantsToMove)
			{
				// Set the default speed.
				m_DesiredSpeed = m_ForwardSpeed * Player.MovementSpeedFactor.Get();

				// If the player wants to move sideways...
				if(Mathf.Abs(movementInputClamped.x) > 0f)
					m_DesiredSpeed = m_SidewaysSpeed;

				// If the player wants to move backwards...
				if(movementInputClamped.y < 0f)
					m_DesiredSpeed = m_BackwardSpeed;

				// If we're currently running...
				if(Player.Run.Active)
				{
					// If the player wants to move forward or sideways, apply the run speed multiplier.
					if(m_DesiredSpeed == m_ForwardSpeed || m_DesiredSpeed == m_SidewaysSpeed)
						m_DesiredSpeed *= m_RunSpeedMultiplier;
				}

				// If we're crouching...
				if(Player.Crouch.Active)
					m_DesiredSpeed *= m_CrouchSpeedMultiplier;
			}

			m_DesiredVelocity = targetDirection * m_DesiredSpeed;
		}

		private bool TryStart_Run()
		{
			if(!m_EnableRunning)
				return false;

			bool wantsToMoveForward = Player.MovementInput.Get().y > 0f;
			bool inventoryClosed = InventoryController.Instance.IsClosed;
			bool hasStamina = Player.Stamina.Get() > 0f;

			return Player.IsGrounded.Get() && hasStamina && wantsToMoveForward && inventoryClosed && !Player.Crouch.Active && !Player.Aim.Active;
		}

		private bool TryStart_Jump()
		{
			bool canJump = m_EnableJumping && 
				Player.Stamina.Get() > 5f &&
				(Player.IsGrounded.Get() || Player.NearLadders.Count > 0) &&
				(!Player.Crouch.Active || Player.Crouch.TryStop());
			
			if(canJump)
			{
				Vector3 jumpDirection = Vector3.up;
				//float surfaceAngle = Vector3.Angle(Vector3.up, m_LastSurfaceNormal);

				if(Player.NearLadders.Count > 0)
					jumpDirection = -Player.NearLadders.Peek().forward;
				// Jump more perpendicular to the surface, on steep surfaces.
				//else if(surfaceAngle > 30f)
				//	jumpDirection = Vector3.Lerp(Vector3.up, m_LastSurfaceNormal, surfaceAngle / 60f).normalized;

				m_CurrentVelocity.y = 0f;

				if(Player.NearLadders.Count > 0)
				{
					m_CurrentVelocity += jumpDirection * m_JumpSpeedFromLadder;
					m_JumpedFrom = JumpedFrom.Ladder;
				}
				else
				{
					m_CurrentVelocity += jumpDirection * CalculateJumpSpeed(m_JumpHeight);
					m_JumpedFrom = JumpedFrom.Ground;
				}

				return true;
			}
		
			return false;
		}

		private float CalculateJumpSpeed(float heightToReach)
		{
			return Mathf.Sqrt(2f * m_Gravity * heightToReach);
		}

		private bool TryStart_Crouch()
		{
			bool canCrouch = 
				m_EnableCrouching &&
				Player.NearLadders.Count == 0 &&
				(Time.time > m_LastTimeToggledCrouching + m_CrouchDuration) && 
				Player.IsGrounded.Get() && 
				!Player.Run.Active;
			
			if(canCrouch)
			{
				StartCoroutine(C_SetHeight(m_CrouchHeight));
				m_LastTimeToggledCrouching = Time.time;
			}

			return canCrouch;
		}

		private bool TryStop_Crouch()
		{
			bool passedEnoughTime = (Time.time > m_LastTimeToggledCrouching + m_CrouchDuration);
			bool obstacleAbove = CheckForObstacles(true, Mathf.Abs(m_CrouchHeight - m_UncrouchedHeight));
			bool canStopCrouching = passedEnoughTime && !obstacleAbove;

			if(canStopCrouching)
			{
				StartCoroutine(C_SetHeight(m_UncrouchedHeight));
				m_LastTimeToggledCrouching = Time.time;
			}

			return canStopCrouching;
		}

		private IEnumerator C_SetHeight(float targetHeight)
		{
			float speed = Mathf.Abs(targetHeight - m_Controller.height) / m_CrouchDuration;

			while(Mathf.Abs(targetHeight - m_Controller.height) > Mathf.Epsilon)
			{
				m_Controller.height = Mathf.MoveTowards(m_Controller.height, targetHeight, Time.deltaTime * speed);
				m_Controller.center = Vector3.up * m_Controller.height / 2f;

				yield return null;
			}
		}

		private void OnControllerColliderHit(ControllerColliderHit hit)
		{
			m_LastSurfaceNormal = hit.normal;

			if(hit.rigidbody)
			{
				float forceMagnitude = m_PushForce * m_Controller.velocity.magnitude;
				Vector3 impactForce = (hit.moveDirection + Vector3.up * 0.35f) * forceMagnitude;
				hit.rigidbody.AddForceAtPosition(impactForce, hit.point);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if(other.CompareTag(Tags.LADDER))
			{
				Player.NearLadders.Enqueue(other.transform);
				//Player.CurrentLadder.Set(other.transform);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if(other.CompareTag(Tags.LADDER))
			{
				Player.NearLadders.Dequeue();
				//Player.CurrentLadder.Set(null);
			}
		}
			
		private bool CheckForObstacles(bool checkAbove, float maxDistance, out RaycastHit hitInfo)
		{
			Vector3 rayOrigin = transform.position + (checkAbove ? Vector3.up * m_Controller.height : Vector3.zero);
			Vector3 rayDirection = checkAbove ? Vector3.up : Vector3.down;

			return Physics.SphereCast(new Ray(rayOrigin, rayDirection), m_Controller.radius, out hitInfo, maxDistance, ~0, QueryTriggerInteraction.Ignore);
		}

		private bool CheckForObstacles(bool checkAbove, float maxDistance)
		{
			Vector3 rayOrigin = transform.position + (checkAbove ? Vector3.up * m_Controller.height : Vector3.zero);
			Vector3 rayDirection = checkAbove ? Vector3.up : Vector3.down;

			return Physics.SphereCast(new Ray(rayOrigin, rayDirection), m_Controller.radius, maxDistance, ~0, QueryTriggerInteraction.Ignore);
		}
	}
}