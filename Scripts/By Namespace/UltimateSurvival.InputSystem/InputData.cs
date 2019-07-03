using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival.InputSystem
{
    public class InputData : ScriptableObject
    {
		public ET.InputType InputType { get { return m_InputType; } }

		public List<Button> Buttons { get { return m_Buttons; } }

		public List<Axis> Axes { get { return m_Axes; } }


        [SerializeField]
        private ET.InputType m_InputType = ET.InputType.Standalone;

        [SerializeField]
        private List<Button> m_Buttons = new List<Button>();

        [SerializeField]
        private List<Axis> m_Axes = new List<Axis>();
    }
}