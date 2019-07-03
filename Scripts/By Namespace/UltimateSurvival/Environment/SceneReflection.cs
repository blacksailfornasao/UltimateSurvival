using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival
{
	public class SceneReflection : MonoBehaviour 
	{
		[SerializeField]
		private ReflectionProbe m_ReflectionProbe;


		private IEnumerator Start()
		{
			var waitInterval = new WaitForSeconds(0.2f);

			while(true)
			{
				m_ReflectionProbe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
				m_ReflectionProbe.RenderProbe();
				yield return waitInterval;
			}
		}
	}
}
