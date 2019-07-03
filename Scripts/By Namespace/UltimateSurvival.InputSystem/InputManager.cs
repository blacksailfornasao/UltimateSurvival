using System.Linq;
using UnityEngine;

namespace UltimateSurvival.InputSystem
{
    public class InputManager : MonoBehaviour
    {
		/// <summary> </summary>
		public InputData InputData
		{ 
			get { return m_InputData; }
			set { m_InputData = value; }
		}

        [SerializeField]
        private InputData m_InputData;


        public void SetupDefaults(ET.InputType inputType, ET.InputMode inputMode)
        {
            if (inputType == ET.InputType.Standalone)
            {
                if (inputMode == ET.InputMode.Axes)
                {
                    AddAxis(new Axis("Horizontal Axis", ET.StandaloneAxisType.Unity, "Horizontal"));
                    AddAxis(new Axis("Vertical Axis", ET.StandaloneAxisType.Unity, "Vertical"));

                    AddAxis(new Axis("Mouse X", ET.StandaloneAxisType.Unity, "Mouse X"));
                    AddAxis(new Axis("Mouse Y", ET.StandaloneAxisType.Unity, "Mouse Y"));
                }
                else
                    AddDefaultButtons();
            }
            else if (inputType == ET.InputType.Mobile)
            {
                if (inputMode == ET.InputMode.Axes)
                    AddAxis(new Axis("Movement Axis", ET.StandaloneAxisType.Custom, new Joystick()));
                else
                    AddDefaultButtons();
            }
        }

        private void AddDefaultButtons()
        {
            AddButton(new Button("Sprint", KeyCode.LeftShift));
            AddButton(new Button("Attack", KeyCode.Mouse0));
            AddButton(new Button("Jump", KeyCode.Space));
            AddButton(new Button("Crouch", KeyCode.C));
            AddButton(new Button("Reload", KeyCode.R));
        }

        public void Clear(ET.InputMode inputMode)
        {
            if (inputMode == ET.InputMode.Axes)
                m_InputData.Axes.Clear();
            else if (inputMode == ET.InputMode.Buttons)
                m_InputData.Buttons.Clear();
        }

        public void ClearAll()
        {
            m_InputData.Axes.Clear();

            m_InputData.Buttons.Clear();
        }

        public float GetAxis(string name)
        {
            Axis axis = FindAxis(name);
            float value = 0f;

            if (axis != null)
            {
                if (axis.AxisType == ET.StandaloneAxisType.Unity)
                    value += UnityEngine.Input.GetAxis(axis.UnityAxisName);
                if (axis.AxisType == ET.StandaloneAxisType.Custom)
                    value += -GetKeyPress(axis.NegativeKey) + GetKeyPress(axis.PositiveKey);
            }

            return (axis.Normalize) && (axis.AxisType != ET.StandaloneAxisType.Unity) ? Mathf.Clamp(value, -1f, 1f) : value;
        }

		public float GetAxisRaw(string name)
		{
			Axis axis = FindAxis(name);
			float value = 0f;

			if (axis != null)
			{
				if (axis.AxisType == ET.StandaloneAxisType.Unity)
					value += UnityEngine.Input.GetAxisRaw(axis.UnityAxisName);
				if (axis.AxisType == ET.StandaloneAxisType.Custom)
					value += -GetKeyPress(axis.NegativeKey) + GetKeyPress(axis.PositiveKey);
			}

			return (axis.Normalize) && (axis.AxisType != ET.StandaloneAxisType.Unity) ? Mathf.Clamp(value, -1f, 1f) : value;
		}

        public bool GetButton(string name)
        {
            Button button = FindButton(name);
            bool value = false;

            if (button != null)
                value = UnityEngine.Input.GetKey(button.Key);

            return value;
        }

        public bool GetButtonDown(string name)
        {
            Button button = FindButton(name);
            bool value = false;

            if (button != null)
                value = Input.GetKeyDown(button.Key);

            return value;
        }

		public bool GetButtonUp(string name)
		{
			Button button = FindButton(name);
			bool value = false;

			if (button != null)
				value = Input.GetKeyUp(button.Key);

			return value;
		}

        private void AddButton(Button toAdd)
        {
            m_InputData.Buttons.Add(toAdd);
        }

        private void AddAxis(Axis toAdd)
        {
            m_InputData.Axes.Add(toAdd);
        }

        private Button FindButton(string name)
        {
			for(int i = 0;i < m_InputData.Buttons.Count;i ++)
			{
				if(name == m_InputData.Buttons[i].Name)
					return m_InputData.Buttons[i];
			}

			return null;
        }

        private Axis FindAxis(string name)
        {
			for(int i = 0;i < m_InputData.Axes.Count;i ++)
			{
				if(name == m_InputData.Axes[i].AxisName)
					return m_InputData.Axes[i];
			}

			return null;
        }

        private int GetKeyPress(KeyCode key)
        {
            if (UnityEngine.Input.GetKey(key))
                return 1;

            return 0;
        }
    }
}