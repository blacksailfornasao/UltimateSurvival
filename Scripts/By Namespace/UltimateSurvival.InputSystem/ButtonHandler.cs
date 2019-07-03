using UnityEngine;
using System;

namespace UltimateSurvival.InputSystem
{
    /// <summary>
    /// Used for holding button-specific information(like the up-down states).
    /// </summary>
    public class ButtonHandler : MonoBehaviour
    {
        public ET.ButtonState State
        {
            get; private set;
        }
        public event Action OnButtonDown;


        private void Start()
        {
            State = ET.ButtonState.Up;
        }

        public void SetUpState()
        {
            State = ET.ButtonState.Up;
        }

        public void SetDownState()
        {
            State = ET.ButtonState.Down;

            if (OnButtonDown != null)
                OnButtonDown();
        }
    }
}