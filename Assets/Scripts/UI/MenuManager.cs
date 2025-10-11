using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private NotificationManager notificationManager;
    public GameObject startPanel;
    public GameObject registerPanel;
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
    public Button MapEditorButton;
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
    public void LoggedIn()
    {
        PlayButton.gameObject.SetActive(true);
        MapEditorButton.gameObject.SetActive(true);
        RegisterButton.gameObject.SetActive(false);
        LoginButton.gameObject.SetActive(false);
        LogoutButton.gameObject.SetActive(true);
    }
    public void LogOut()
    {
        PlayButton.gameObject.SetActive(false);
        MapEditorButton.gameObject.SetActive(false);
        RegisterButton.gameObject.SetActive(true);
        LoginButton.gameObject.SetActive(true);
        LogoutButton.gameObject.SetActive(false);
        notificationManager.OnSuccessMessage("Logged out");
    }
    public void Register()
    {
        if (RegisterPasswordInput1.text != RegisterPasswordInput2.text)
        {
            notificationManager.OnErrorMessage("Passwords do not match!");
            return;
        }
        api.Register(RegisterUsernameInput.text, RegisterPasswordInput1.text);
    }

    public void Login()
    {
        api.Login(LoginUsernameInput.text, LoginPasswordInput.text);
    }


    
    public void OpenMapEdiorScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MapEditor");
    }
}
