using System.Collections;
using UnityEngine;

namespace UltimateSurvival
{
    public class ParticleDestroyer : MonoBehaviour
    {
        // Allows a particle system to exist for a specified duration,
        // then shuts off emission, and waits for all particles to expire
        // before destroying the gameObject

		[SerializeField]
		private float m_MinDuration = 8;

		[SerializeField]
		private float m_MaxDuration = 10;

        private float m_MaxLifetime;
        private bool m_EarlyStop;


        private IEnumerator Start()
        {
            var systems = GetComponentsInChildren<ParticleSystem>();

            // Find out the maximum lifetime of any particles in this effect
            foreach (var system in systems)
				m_MaxLifetime = Mathf.Max(system.main.startLifetimeMultiplier, m_MaxLifetime);

            // Wait for random duration

            float stopTime = Time.time + Random.Range(m_MinDuration, m_MaxDuration);

            while (Time.time < stopTime || m_EarlyStop)
            {
                yield return null;
            }
            //Debug.Log("stopping " + name);

            // Turn off emission
            foreach (var system in systems)
            {
                var emission = system.emission;
                emission.enabled = false;
            }

            // Wait for any remaining particles to expire
            yield return new WaitForSeconds(m_MaxLifetime);

            Destroy(gameObject);
        }


        public void Stop()
        {
            // Stops the particle system early
            m_EarlyStop = true;
        }
    }
}
