using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] private GameObject _messagePanel;
    public GameObject MessagePanel => _messagePanel;

    public void OnSuccessMessage(string message)
    {
        Color BGcolor = new Color(0.098f, 0.698f, 0f, 0.22f);
        Color textColor = new Color(0.322f, 1f, 0f, 1f);
        StartCoroutine(ShowMessage(message, BGcolor, textColor));
    }

    public void OnErrorMessage(string error)
    {
        Color BGcolor = new Color(0.925f, 0.133f, 0f, 0.22f);
        Color textColor = new Color(1f, 0.145f, 0f, 1f);
        StartCoroutine(ShowMessage(error, BGcolor, textColor));
    }

    private IEnumerator ShowMessage(string message, Color color, Color textColor)
    {
        MessagePanel.GetComponent<UnityEngine.UI.Image>().color = color;
        MessagePanel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = message;
        MessagePanel.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = textColor;
        MessagePanel.SetActive(true);
        var canvasGroup = MessagePanel.GetComponent<CanvasGroup>();
        yield return FadeIn(canvasGroup, 1f);
        yield return new WaitForSeconds(2);
        yield return FadeOut(canvasGroup, 1f);
        MessagePanel.SetActive(false);
    }

    private IEnumerator FadeIn(CanvasGroup canvasGroup, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1, time / duration);
            yield return null;
        }

        canvasGroup.alpha = 1;
    }

    private IEnumerator FadeOut(CanvasGroup canvasGroup, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, time / duration);
            yield return null;
        }

        canvasGroup.alpha = 0;
    }
}
