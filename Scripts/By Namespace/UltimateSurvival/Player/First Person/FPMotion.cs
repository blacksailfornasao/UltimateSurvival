using System;
using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class TransformOffset
	{
		public Vector3 CurrentPosition { get { return m_CurrentPosition; } }
		public Vector3 CurrentRotation { get { return m_CurrentRotation; } }

		[SerializeField]
		private float m_LerpSpeed = 5f;

		[SerializeField]
		private Vector3 m_Position;

		[SerializeField]
		private Vector3 m_Rotation;

		private Vector3 m_CurrentPosition;
		private Vector3 m_CurrentRotation;


		public TransformOffset GetClone()
		{
			return (TransformOffset)MemberwiseClone();
		}

		public void Reset()
		{
			m_CurrentPosition = m_CurrentRotation = Vector3.zero;
		}

		public void ContinueFrom(Vector3 position, Vector3 rotation)
		{
			m_CurrentPosition = position;
			m_CurrentRotation = rotation;
		}

		public void ContinueFrom(TransformOffset state)
		{
			m_CurrentPosition = state.CurrentPosition;
			m_CurrentRotation = state.CurrentRotation;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetValue"> 0 = no influence, 1 = full influence. </param>
		/// <param name="lerpSpeed">Lerp speed.</param>
		public void Update(float deltaTime, out Vector3 position, out Quaternion rotation)
		{	
			m_CurrentPosition = Vector3.Lerp(m_CurrentPosition, m_Position, deltaTime * m_LerpSpeed);
			m_CurrentRotation = new Vector3(
				Mathf.LerpAngle(m_CurrentRotation.x, m_Rotation.x, deltaTime * m_LerpSpeed),
				Mathf.LerpAngle(m_CurrentRotation.y, m_Rotation.y, deltaTime * m_LerpSpeed),
				Mathf.LerpAngle(m_CurrentRotation.z, m_Rotation.z, deltaTime * m_LerpSpeed)
			);

			position = m_CurrentPosition;
			rotation = Quaternion.Euler(m_CurrentRotation);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	[RequireComponent(typeof(FPObject))]
	public class FPMotion : PlayerBehaviour
	{
		[Header("Setup")]

		[SerializeField] 
		private Transform m_Model;

		[SerializeField] 
		private Transform m_Pivot;

		[Header("Sway")]

		[SerializeField] 
		private Sway m_MovementSway;

		[SerializeField] 
		private Sway m_RotationSway;

		[Header("Bob")]

		[SerializeField] 
		private TrigonometricBob m_WalkBob;

		[SerializeField]
		private TrigonometricBob m_AimBob;

		[SerializeField] 
		private TrigonometricBob m_RunBob;

		[SerializeField]
		private LerpControlledBob m_LandBob;

		[SerializeField]
		private float m_MaxLandSpeed = 12f;

		[Header("Offset")]

		[SerializeField]
		private TransformOffset m_IdleOffset;

		[SerializeField]
		private TransformOffset m_RunOffset;

		[SerializeField]
		private TransformOffset m_AimOffset;

		[SerializeField]
		private TransformOffset m_OnLadderOffset;

		[SerializeField]
		private TransformOffset m_JumpOffset;

		[SerializeField]
		[Tooltip("The object position and rotation offset, when the character is too close to an object. " +
			"NOTE: Will not be taken into consideration if the object can be used when near other objects (see the 'CanUseWhileNearObjects' setting).")]
		private TransformOffset m_TooCloseOffset;

		private Transform m_Root;
		private FPObject m_Object;
		private FPWeaponBase m_Weapon;
		private TransformOffset m_CurrentOffset;
		private bool m_HolsterActive;


		private void Awake()
		{
			m_Object = GetComponent<FPObject>();
			m_Weapon = m_Object as FPWeaponBase;
			m_Object.Draw.AddListener(On_Draw);
			m_Object.Holster.AddListener(On_Holster);

			SetupTransforms();

			Player.Land.AddListener(On_Land);
			m_CurrentOffset = m_IdleOffset;
		}

		private void On_Draw()
		{
			m_IdleOffset.Reset();
			m_CurrentOffset = m_IdleOffset;
			m_HolsterActive = false;
		}

		private void On_Holster()
		{
			m_HolsterActive = true;
		}

		private void On_Land(float landSpeed)
		{
			if(m_Object.IsEnabled && gameObject.activeInHierarchy)
				StartCoroutine(m_LandBob.DoBobCycle(landSpeed / m_MaxLandSpeed));
		}

		private void SetupTransforms()
		{
			var m_Root = new GameObject("Root").transform;
			m_Root.transform.SetParent(transform);

			m_Root.position = m_Pivot.position;
			m_Root.rotation = m_Pivot.rotation;

			m_Pivot.SetParent(m_Root, true);
			m_Model.SetParent(m_Pivot, true);
		}

		private void Update()
		{
			Vector2 input;
			if(InventoryController.Instance.IsClosed)
				input = Player.LookInput.Get();
			else
				input = Vector2.zero;

			// Calculate the position & rotation sway.
			m_MovementSway.CalculateSway(-input, Time.deltaTime);
			m_RotationSway.CalculateSway(new Vector2(input.y , -input.x), Time.deltaTime);

			// Apply the sway.
			m_Pivot.localPosition = m_MovementSway.Value;
			m_Pivot.localRotation = Quaternion.Euler(m_RotationSway.Value);

			// Movement bobs.
			float moveSpeed = Player.Velocity.Get().magnitude;
			Vector3 totalBob = Vector3.zero;

			// Movement bob while aiming.
			if(Player.Aim.Active && moveSpeed > 1f)
				totalBob += m_AimBob.CalculateBob(moveSpeed, Time.deltaTime);
			else
				totalBob += m_AimBob.Cooldown(Time.deltaTime);

			// Movement bob while walking.
			if(Player.Walk.Active && !Player.Aim.Active)
				totalBob += m_WalkBob.CalculateBob(moveSpeed, Time.deltaTime);
			else
				totalBob += m_WalkBob.Cooldown(Time.deltaTime);

			// Movement bob while running.
			if(Player.Run.Active)
				totalBob += m_RunBob.CalculateBob(moveSpeed, Time.deltaTime);
			else
				totalBob += m_RunBob.Cooldown(Time.deltaTime);
				
			// Apply the total bob to the local position.
			m_Pivot.localPosition += totalBob;

			// Apply the animation when landed (land bob).
			m_Pivot.localPosition += Vector3.up * m_LandBob.Value;

			bool tooCloseCondition = 
				m_Weapon && 
				!m_Weapon.UseWhileNearObjects && 
				Player.IsCloseToAnObject.Get() &&
				Player.RaycastData.Get().GameObject.layer != LayerMask.NameToLayer("Hitbox") &&
				!Player.RaycastData.Get().GameObject.CompareTag(Tags.LADDER);

			// Position & rotation offset for different states.
			if(m_HolsterActive)
				TryChangeOffset(m_IdleOffset);
			else if(Player.NearLadders.Count > 0)
				TryChangeOffset(m_OnLadderOffset);
			else if(tooCloseCondition)
				TryChangeOffset(m_TooCloseOffset);
			else if(Player.Run.Active)
				TryChangeOffset(m_RunOffset);
			else if(Player.Aim.Active)
				TryChangeOffset(m_AimOffset);
			else if(!Player.IsGrounded.Get())
			{
				TryChangeOffset(m_JumpOffset);
			}
			else
				TryChangeOffset(m_IdleOffset);

			Vector3 m_PosOffset;
			Quaternion m_RotOffset;
			m_CurrentOffset.Update(Time.deltaTime, out m_PosOffset, out m_RotOffset);

			m_Pivot.localPosition += m_PosOffset;
			m_Pivot.localRotation *= m_RotOffset;
		}

		private void TryChangeOffset(TransformOffset newOffset)
		{
			if(m_CurrentOffset != newOffset)
			{
				newOffset.ContinueFrom(m_CurrentOffset);
				m_CurrentOffset = newOffset;
			}
		}
	}
}
