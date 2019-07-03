using UnityEngine;
using UnityEngine.EventSystems;

namespace UltimateSurvival.InputSystem
{
    /// <summary>
    /// Allows a game object to be dragged and moved around in a circle(A mobile joystick).
    /// </summary>
    public class Joystick : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler
    {
        [SerializeField]
        [Tooltip("How far this can be moved(in pixels).")]
        private float m_MovementRange = 48f;

        private Canvas m_ParentCanvas;
        private Vector3 m_StartPosition;
        private Vector3 m_CurrentPosition;
        private float m_InitialMovementRange;


        private void Start()
        {
            // Get Canvas component so we adjust the calculations based on it's scale factor.
            m_ParentCanvas = GetComponentInParent<Canvas>();

            // Store the starting position.
            m_StartPosition = transform.position;

            // Store the initial movement range, it will be multiplied by parentCanvas.scaleFactor later.
            m_InitialMovementRange = m_MovementRange;
        }

        /// <summary>
        /// Called by Unity,
        /// moves this object to the drag position.
        /// </summary>
        public void OnDrag(PointerEventData data)
        {
            m_MovementRange = m_InitialMovementRange * m_ParentCanvas.scaleFactor;
            m_CurrentPosition.x = (int)(data.position.x - m_StartPosition.x);
            m_CurrentPosition.y = (int)(data.position.y - m_StartPosition.y);
            m_CurrentPosition = Vector3.ClampMagnitude(m_CurrentPosition, m_MovementRange);

            transform.position = m_StartPosition + m_CurrentPosition;
        }

        /// <summary>
        /// Called by Unity,
        /// when the user stops dragging, reset the joystick position.
        /// </summary>
        public void OnPointerUp(PointerEventData data)
        {
            transform.position = m_StartPosition;
            m_CurrentPosition = Vector3.zero;
        }

        public void OnPointerDown(PointerEventData data)
        {
        }

        /// <summary>
        /// How much the joystick is moved horizontally (1 == full right, -1 == full left).
        /// </summary>
        public float GetHorizontalInput()
        {
            return m_CurrentPosition.x / m_MovementRange;
        }

        /// <summary>
        /// How much the joystick is moved vertically (1 == full top, -1 == full bottom).
        /// </summary>
        public float GetVerticalInput()
        {
            return m_CurrentPosition.y / m_MovementRange;
        }

        /// <summary>
        /// If this returns 1, it means the joystick is at the limit, and 0 means is in the center.
        /// </summary>
        public float GetNormalizedMagnitude()
        {
            return m_CurrentPosition.magnitude / m_MovementRange;
        }
    }
}