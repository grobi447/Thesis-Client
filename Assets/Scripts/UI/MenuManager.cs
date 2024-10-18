using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject registerPanel;
    public GameObject messagePanel;
    public GameObject loginPanel;
    public TMPro.TMP_InputField RegisterUsernameInput;
    public TMPro.TMP_InputField LoginUsernameInput;
    public TMPro.TMP_InputField RegisterPasswordInput1;
    public TMPro.TMP_InputField RegisterPasswordInput2;
    public TMPro.TMP_InputField LoginPasswordInput;
    public Button PlayButton;
    public Button RegisterButton;
    public Button LoginButton;
    public Button LogoutButton;
    public API api;

    public void ShowRegisterPanel()
    {
        startPanel.SetActive(false);
        registerPanel.SetActive(true);
    }

    public void ShowStartPanel()
    {
        startPanel.SetActive(true);
        registerPanel.SetActive(false);
        loginPanel.SetActive(false);
    }
    public void ShowLoginPanel()
    {
        startPanel.SetActive(false);
        loginPanel.SetActive(true);
    }
    public void LoggedIn(){
        PlayButton.gameObject.SetActive(true);
        RegisterButton.gameObject.SetActive(false);
        LoginButton.gameObject.SetActive(false);
        LogoutButton.gameObject.SetActive(true);
    }
    public void LogOut(){
        PlayButton.gameObject.SetActive(false);
        RegisterButton.gameObject.SetActive(true);
        LoginButton.gameObject.SetActive(true);
        LogoutButton.gameObject.SetActive(false);
        OnSuccessMessage("Logged out");
    }
    public void Register()
    {
        if (RegisterPasswordInput1.text != RegisterPasswordInput2.text)
        {
            OnErrorMessage("Passwords do not match!");
            return;
        }
        api.Register(RegisterUsernameInput.text, RegisterPasswordInput1.text);
    }

    public void Login()
    {
        api.Login(LoginUsernameInput.text, LoginPasswordInput.text);
    }

    public void OnSuccessMessage(string message)
    {
        Color BGcolor = new Color(0.098f, 0.698f, 0f, 0.22f);
        Color textColor = new Color(0.322f, 1f, 0f, 1f);
        ShowStartPanel();
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
        messagePanel.GetComponent<UnityEngine.UI.Image>().color = color;
        messagePanel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = message;
        messagePanel.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = textColor;
        messagePanel.SetActive(true);
        var canvasGroup = messagePanel.GetComponent<CanvasGroup>();
        yield return FadeIn(canvasGroup, 1f);
        yield return new WaitForSeconds(2);
        yield return FadeOut(canvasGroup, 1f);
        messagePanel.SetActive(false);
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
