using UnityEngine;

namespace UltimateSurvival.InputSystem
{
    [System.Serializable]
    public class Axis
    {
        public string AxisName
        {
            get
            {
                return m_AxisName;
            }

            set
            {
                m_AxisName = value;
            }
        }

        public ET.StandaloneAxisType AxisType
        {
            get
            {
                return m_AxisType;
            }
        }

        public string UnityAxisName
        {
            get
            {
                return m_UnityAxisName;
            }
        }

        public KeyCode NegativeKey
        {
            get
            {
                return m_NegativeKey;
            }
        }

        public KeyCode PositiveKey
        {
            get
            {
                return m_PositiveKey;
            }
        }

        public bool Normalize
        {
            get
            {
                return m_Normalize;
            }
        }

        [SerializeField]
        private string m_AxisName;

        [SerializeField]
        private bool m_Normalize;

        [SerializeField]
        private ET.StandaloneAxisType m_AxisType;

      //  [SerializeField]
        //private Joystick m_JoyStick;

        [SerializeField]
        private string m_UnityAxisName;

        [SerializeField]
        private KeyCode m_PositiveKey;

        [SerializeField]
        private KeyCode m_NegativeKey;

        public Axis(string name, ET.StandaloneAxisType axisType)
        {
            m_AxisName = name;
            m_AxisType = axisType;
        }

        public Axis(string name, ET.StandaloneAxisType axisType, Joystick joystick)
        {
            m_AxisName = name;
            m_AxisType = axisType;

           // m_JoyStick = joystick;
        }

        public Axis(string name, ET.StandaloneAxisType axisType, string unityAxisName)
        {
            m_AxisName = name;
            m_AxisType = axisType;

            m_UnityAxisName = unityAxisName;
        }

        public Axis(string name, ET.StandaloneAxisType axisType, KeyCode positiveKey, KeyCode negativeKey, string unityAxisName)
        {
            m_AxisName = name;
            m_AxisType = axisType;

            m_PositiveKey = positiveKey;
            m_NegativeKey = negativeKey;

            m_UnityAxisName = unityAxisName;
        }
    }
}