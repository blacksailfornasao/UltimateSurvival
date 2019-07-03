using System;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class MouseLook : PlayerBehaviour
	{
		[Header("General")]

		[SerializeField] 
		[Tooltip("The camera root which will be rotated up & down (on the X axis).")]
		private Transform m_LookRoot;

		[SerializeField]
		private Transform m_PlayerRoot;

		[SerializeField] 
		[Tooltip("The up & down rotation will be inverted, if checked.")]
		private bool m_Invert;	

		[SerializeField] 
		[Tooltip("If checked, a button will show up which can lock the cursor.")]
		private bool m_ShowLockButton = true;

		[SerializeField]
		[Tooltip("If checked, you can unlock the cursor by pressing the Escape / Esc key.")]
		private bool m_CanUnlock = true;

		[Header("Motion")]

		[SerializeField] 
		[Tooltip("The higher it is, the faster the camera will rotate.")]
		private float m_Sensitivity = 5f;

		[SerializeField]
		[Range(0, 20)]
		private int m_SmoothSteps = 10;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_SmoothWeight = 0.4f;

		[SerializeField]
		private float m_RollAngle = 10f;

		[SerializeField]
		private float m_RollSpeed = 3f;

		[Header("Rotation Limits")]

		[SerializeField]
		private Vector2 m_DefaultLookLimits = new Vector2(-60f, 90f);

		private float m_CurrentRollAngle;
		private bool m_InventoryIsOpen;
	
		private Vector2 m_LookAngles;

		private int m_LastLookFrame;
		private Vector2 m_CurrentMouseLook;
		private Vector2 m_SmoothMove;
		private List<Vector2> m_SmoothBuffer = new List<Vector2>();


		private void Start()
		{
			if(!m_LookRoot)
			{
				Debug.LogErrorFormat(this, "Assign the look root in the inspector!", name);
				enabled = false;
			}

			InventoryController.Instance.State.AddChangeListener(OnChanged_InventoryState);

			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}

		private void OnChanged_InventoryState()
		{
			m_InventoryIsOpen = !InventoryController.Instance.IsClosed;

			if(m_InventoryIsOpen)
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			}
			else
			{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}
		}

		private void OnGUI()
		{
			if(!m_ShowLockButton)
				return;

			Vector2 buttonSize = new Vector2(256f, 24f);

			// NOTE: While in Unity Editor, pressing Esc will always unlock the cursor.
			if (Event.current.type == EventType.KeyDown && m_CanUnlock)
			{
				if (Event.current.keyCode == KeyCode.Escape)
				{
					Cursor.visible = true;
					Cursor.lockState = CursorLockMode.None;
				}
			}	

			if(Cursor.lockState == CursorLockMode.None && !m_InventoryIsOpen)
			{
				if(GUI.Button(new Rect(Screen.width * 0.5f - buttonSize.x / 2f, 16f, buttonSize.x, buttonSize.y), "Lock Cursor (Hit 'Esc' to unlock)"))
				{
					Cursor.visible = false;
					Cursor.lockState = CursorLockMode.Locked;
				}	
			}
		}

		private void Update()
		{
			if(Player.ViewLocked.Is(false) && Cursor.lockState == CursorLockMode.Locked && !Player.Sleep.Active && Player.Health.Get() > 0f)
				LookAround();

			Player.ViewLocked.Set(Cursor.lockState != CursorLockMode.Locked || Player.SelectBuildable.Active);
		}

		/// <summary>
		/// Rotates the camera and character and creates a sensation of looking around.
		/// </summary>
		private void LookAround()
		{
			CalculateMouseInput(Time.deltaTime);

			m_LookAngles.x += m_CurrentMouseLook.x * m_Sensitivity * (m_Invert ? 1f : -1f);
			m_LookAngles.y += m_CurrentMouseLook.y * m_Sensitivity;

			m_LookAngles.x = ClampAngle(m_LookAngles.x, m_DefaultLookLimits.x, m_DefaultLookLimits.y);
		
			m_CurrentRollAngle = Mathf.Lerp(m_CurrentRollAngle, Player.LookInput.Get().x * m_RollAngle, Time.deltaTime * m_RollSpeed);

			// Apply the current up & down rotation to the look root.
			m_LookRoot.localRotation = Quaternion.Euler(m_LookAngles.x, 0f, m_CurrentRollAngle);

			m_PlayerRoot.localRotation = Quaternion.Euler(0f, m_LookAngles.y, 0f);

			Player.LookDirection.Set(m_LookRoot.forward);
		}

		/// <summary>
		/// Clamps the given angle between min and max degrees.
		/// </summary>
		private float ClampAngle(float angle, float min, float max) 
		{
			if(angle > 360f)
				angle -= 360f;
			else if(angle < -360f)
				angle += 360f;
			
			return Mathf.Clamp(angle, min, max);
		}

		private void CalculateMouseInput(float deltaTime)
		{
			if (m_LastLookFrame == Time.frameCount)
				return;

			m_LastLookFrame = Time.frameCount;

			m_SmoothMove = new Vector2(Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"));

			m_SmoothSteps = Mathf.Clamp(m_SmoothSteps, 1, 20);
			m_SmoothWeight = Mathf.Clamp01(m_SmoothWeight);

			while (m_SmoothBuffer.Count > m_SmoothSteps)
				m_SmoothBuffer.RemoveAt(0);

			m_SmoothBuffer.Add(m_SmoothMove);

			float weight = 1f;
			Vector2 average = Vector2.zero;
			float averageTotal = 0f;

			for(int i = m_SmoothBuffer.Count - 1; i > 0; i--)
			{
				average += m_SmoothBuffer[i] * weight;
				averageTotal += weight;
				weight *= m_SmoothWeight / (deltaTime * 60f);
			}
				
			averageTotal = Mathf.Max(1f, averageTotal);
			m_CurrentMouseLook = average / averageTotal;
		}
	}
}
