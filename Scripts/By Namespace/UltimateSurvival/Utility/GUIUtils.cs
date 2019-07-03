using UnityEngine;
using UnityEngine.UI;
using UltimateSurvival.GUISystem;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public static class GUIUtils 
	{
		/// <summary>
		/// 
		/// </summary>
		public static Text CreateTextUnder(string name, RectTransform parent, TextAnchor anchor, Vector2 offset)
		{
			Text text = new GameObject(name, typeof(Text)).GetComponent<Text>();
			text.transform.SetParent(parent);
			text.transform.localPosition = offset;
			text.transform.localScale = Vector3.one;
			text.rectTransform.pivot = Vector2.one * 0.5f;
			text.rectTransform.sizeDelta = parent.sizeDelta;
			text.alignment = anchor;
			return text;
		}

		/// <summary>
		/// 
		/// </summary>
		public static Image CreateImageUnder(string name, RectTransform parent, Vector2 offset, Vector2 size)
		{
			Image image = new GameObject(name, typeof(Image)).GetComponent<Image>();
			image.transform.SetParent(parent);
			image.transform.localPosition = offset;
			image.transform.localScale = Vector3.one;
			image.rectTransform.pivot = Vector2.one * 0.5f;
			image.rectTransform.sizeDelta = size;
			image.raycastTarget = false;
			return image;
		}
	}
}
