using UnityEngine;

namespace UltimateSurvival.AI
{
    [System.Serializable]
    public class EntityAnimation
    {
        private AIBrain m_Brain;
        private Animator m_Animator;


        public void Initialize(AIBrain brain)
        {
            m_Brain = brain;

            m_Animator = m_Brain.GetComponent<Animator>();
        }

        public bool ParameterExists(string paramName)
        {
            if (m_Animator.parameterCount == 0)
                return false;

            bool exists = false;

            AnimatorControllerParameter[] paramTs = m_Animator.parameters;
            for (int i = 0; i < paramTs.Length; i++)
            {
                if (paramTs[i].name == paramName)
                    exists = true;
            }

            return exists;
        }

		public void SetTrigger(string paramName)
		{
			if (!ParameterExists(paramName))
				return;

			m_Animator.SetTrigger(paramName);
		}

        /// <summary>
        /// Function used to change the value of an animation in the entities animator.
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        public void ToggleBool(string paramName, bool value)
        {
            if (!ParameterExists(paramName))
                return;

            m_Animator.SetBool(paramName, value);
        }

        /// <summary>
        /// Function helpful for checking whether a animation is currently running on an entity.
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public bool IsBoolToggled(string paramName)
        {
            if (!ParameterExists(paramName))
            {
                Debug.LogError("Parameter with name " + paramName + " does not exist.");

                return false;
            }

            bool isToggled = m_Animator.GetBool(paramName);

            return isToggled;
        }
    }
}