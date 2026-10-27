using TMPro;
using UnityEngine;

namespace Code.Ui.FloatingTexts
{
	public class FloatingText : MonoBehaviour
	{
		[SerializeField] private TMP_Text text;

		[SerializeField] private RectTransform rectTransform;

		public void SetText(string value) => text.text = value;
		public void SetScreenPosition(Vector2 position) => rectTransform.position = position;
	}
}
