using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

namespace UI
{
    public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public TextMeshProUGUI text;
        public Color defaultColor;
        public Color hoverColor;

        public void OnPointerEnter(PointerEventData eventData)
        {
            text.color = hoverColor;
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            text.color = defaultColor;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            text.color = defaultColor;
        }
    }
}
