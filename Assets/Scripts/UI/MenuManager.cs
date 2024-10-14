using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MenuManager : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject registerPanel;
    public TMPro.TMP_InputField usernameInput;
    public TMPro.TMP_InputField passwordInput;
    public API api;

    public void ShowRegisterPanel(){
        startPanel.SetActive(false);
        registerPanel.SetActive(true);
    }

    public void ShowStartPanel(){
        startPanel.SetActive(true);
        registerPanel.SetActive(false);
    }
    public void Register(){
        string username = usernameInput.text;
        string password = passwordInput.text;
        api.Register(username, password);
    }
}
