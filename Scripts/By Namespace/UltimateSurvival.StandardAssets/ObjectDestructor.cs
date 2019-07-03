using UnityEngine;

namespace UltimateSurvival
{
    public class ObjectDestructor : MonoBehaviour
    {
        [SerializeField] 
		private float m_TimeOut = 1f;

        [SerializeField] 
		private bool m_DetachChildren;


        private void Awake()
        {
            Invoke("DestroyNow", m_TimeOut);
        }
			
        private void DestroyNow()
        {
            if (m_DetachChildren)
                transform.DetachChildren();
			
            DestroyObject(gameObject);
        }
    }
}
