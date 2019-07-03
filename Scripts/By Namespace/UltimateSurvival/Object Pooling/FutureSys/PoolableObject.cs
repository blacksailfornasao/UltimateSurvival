using UnityEngine;

namespace UltimateSurvival
{
    public class PoolableObject : MonoBehaviour
    {
        public virtual void OnSpawn() { }//Runs when the object is enabled.

        public virtual void OnDespawn() { }//Runs when the object is disabled.

        public virtual void OnPoolableDestroy() { }//Runs when the object is destroyed.
    }
}