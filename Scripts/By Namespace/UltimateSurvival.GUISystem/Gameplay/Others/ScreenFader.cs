using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateSurvival.GUISystem
{
	public class ScreenFader : GUIBehaviour
	{
		[SerializeField]
		private Image m_Image;
		
		[SerializeField]
		private float m_FadeSpeed = 0.3f;


		private void Start()
		{
			Player.Death.AddListener(On_Death);
			Player.Respawn.AddListener(On_Respawn);
		}

		private void On_Death()
		{
			StopAllCoroutines();
			StartCoroutine(C_FadeScreen(1f));
		}

		private void On_Respawn()
		{
			StopAllCoroutines();
			StartCoroutine(C_FadeScreen(0f));
		}

		private IEnumerator C_FadeScreen(float targetAlpha)
		{
			while(Mathf.Abs(m_Image.color.a - targetAlpha) > 0f)
			{
				m_Image.color = MoveTowardsAlpha(m_Image.color, targetAlpha, Time.deltaTime * m_FadeSpeed);
				AudioListener.volume = 1f - m_Image.color.a;

				yield return null;
			}
		}

		private Color MoveTowardsAlpha(Color color, float alpha, float maxDelta)
		{
			return new Color(color.r, color.g, color.b, Mathf.MoveTowards(color.a, alpha, maxDelta));
		}
	}
}
