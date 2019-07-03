using UnityEngine;

namespace UltimateSurvival.InputSystem
{
    [System.Serializable]
    public class Button
    {
		public string Name { get { return m_ButtonName; } set { m_ButtonName = value; } }

		public KeyCode Key { get { return m_Key; } }

        [SerializeField]
        private string m_ButtonName;

        [SerializeField]
        private KeyCode m_Key;

       // [SerializeField]
        //private ButtonHandler m_Handler;


        public Button(string name)
        {
            m_ButtonName = name;
        }

        public Button(string name, KeyCode key)
        {
            m_ButtonName = name;
            m_Key = key;
        }

        public Button(string name, ButtonHandler handler)
        {
            m_ButtonName = name;
           // m_Handler = handler;
        }
    }
}