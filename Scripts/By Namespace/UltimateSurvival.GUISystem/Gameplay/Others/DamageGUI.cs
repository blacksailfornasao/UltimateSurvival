using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateSurvival.GUISystem
{
	[Serializable]
	public class ImageFader
	{
		public bool Fading { get; private set; }

		[SerializeField]
		private Image m_Image;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_MinAlpha = 0.4f;

		[SerializeField]
		[Range(0f, 100f)]
		private float m_FadeInSpeed = 25f;

		[SerializeField]
		[Range(0f, 100f)]
		private float m_FadeOutSpeed = 0.3f;

		[SerializeField]
		[Range(0f, 10f)]
		private float m_FadeOutPause = 0.5f;

		private Coroutine m_FadeHandler;


		public void DoFadeCycle(MonoBehaviour parent, float targetAlpha)
		{
			if(m_Image == null)
			{
				Debug.LogError("[ImageFader] - The image to fade is not assigned!");
				return;
			}

			targetAlpha = Mathf.Clamp01(Mathf.Max(Mathf.Abs(targetAlpha), m_MinAlpha));

			if(m_FadeHandler != null)
				parent.StopCoroutine(m_FadeHandler);

			m_FadeHandler = parent.StartCoroutine(C_DoFadeCycle(targetAlpha));
		}

		private IEnumerator C_DoFadeCycle(float targetAlpha)
		{
			Fading = true;

			while(Mathf.Abs(m_Image.color.a - targetAlpha) > 0.01f)
			{
				m_Image.color = Color.Lerp(m_Image.color, new Color(m_Image.color.r, m_Image.color.g, m_Image.color.b, targetAlpha), m_FadeInSpeed * Time.deltaTime);

				yield return null;
			}

			m_Image.color = new Color(m_Image.color.r, m_Image.color.g, m_Image.color.b, targetAlpha);

			if(m_FadeOutPause > 0f)
				yield return new WaitForSeconds(m_FadeOutPause);

			while(m_Image.color.a > 0.01f)
			{
				m_Image.color = Color.Lerp(m_Image.color, new Color(m_Image.color.r, m_Image.color.g, m_Image.color.b, 0f), m_FadeOutSpeed * Time.deltaTime);
				yield return null;
			}

			m_Image.color = new Color(m_Image.color.r, m_Image.color.g, m_Image.color.b, 0f);

			Fading = false;
		}
	}

	public class DamageGUI : GUIBehaviour 
	{
		[Header("Blood Screen")]

		[SerializeField]
		private ImageFader m_BloodScreenFader;

		[Header("Damage Indicator")]

		[SerializeField]
		private RectTransform m_IndicatorRT;

		[SerializeField]
		private ImageFader m_IndicatorFader;

		[SerializeField]
		[Clamp(0f, 512)]
		[Tooltip("Damage indicator distance (in pixels) from the screen center.")]
		private int m_IndicatorDistance = 128;

		private Vector3 m_LastHitPoint;

	
		private void Start()
		{
			Player.ChangeHealth.AddListener(OnSuccess_ChangeHealth);
		}

		private void Update()
		{
			if(!m_IndicatorFader.Fading)
				return;

			Vector3 lookDir = Vector3.ProjectOnPlane(Player.LookDirection.Get(), Vector3.up).normalized;
			Vector3 dirToPoint = Vector3.ProjectOnPlane(m_LastHitPoint - Player.transform.position, Vector3.up).normalized;

			Vector3 rightDir = Vector3.Cross(lookDir, Vector3.up);

			float angle = Vector3.Angle(lookDir, dirToPoint) * Mathf.Sign(Vector3.Dot(rightDir, dirToPoint));
			m_IndicatorRT.localEulerAngles = Vector3.forward * angle;
			m_IndicatorRT.localPosition = m_IndicatorRT.up * m_IndicatorDistance;
		}

		private void OnSuccess_ChangeHealth(HealthEventData healthEventData)
		{
			if(healthEventData.Delta < 0f)
			{
				m_BloodScreenFader.DoFadeCycle(this, healthEventData.Delta / 100f);

				if(healthEventData.HitPoint != Vector3.zero)
				{
					m_LastHitPoint = healthEventData.HitPoint;
					m_IndicatorFader.DoFadeCycle(this, 1f);
				}
			}
		}
	}
}
