using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private NotificationManager notificationManager;
        public GameObject mainMenuPanel;
        public GameObject startPanel;
        public GameObject registerPanel;
        public GameObject loginPanel;
        public GameObject mapSelectorPanel;
        public TMPro.TMP_InputField RegisterUsernameInput;
        public TMPro.TMP_InputField LoginUsernameInput;
        public TMPro.TMP_InputField RegisterPasswordInput1;
        public TMPro.TMP_InputField RegisterPasswordInput2;
        public TMPro.TMP_InputField LoginPasswordInput;
        public Button PlayButton;
        public Button MenuRegisterButton;
        public Button MenuLoginButton;
        public Button RegisterButton;
        public Button LoginButton;
        public Button LogoutButton;
        public Button MapEditorButton;
        public GameObject registerSpinner;
        public GameObject loginSpinner;
        public API api;
        public MapSelector mapSelector;
        public GameObject RegisterBackButton;
        public GameObject LoginBackButton;

        public void Awake()
        {
            api.GetMaps();
        }
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
        public void ShowMapSelectorPanel()
        {
            mainMenuPanel.SetActive(false);
            mapSelectorPanel.SetActive(true);
            mapSelector.LoadMaps();
            mapSelector.scrollbar.value = 0;
        }
        public void BackToMainMenu()
        {
            mainMenuPanel.SetActive(true);
            mapSelectorPanel.SetActive(false);
            LoggedIn();
        }
        public void LoggedIn()
        {
            ShowStartPanel();
            PlayButton.gameObject.SetActive(true);
            MapEditorButton.gameObject.SetActive(true);
            MenuRegisterButton.gameObject.SetActive(false);
            MenuLoginButton.gameObject.SetActive(false);
            LogoutButton.gameObject.SetActive(true);
        }
        public void LogOut()
        {
            PlayButton.gameObject.SetActive(false);
            MapEditorButton.gameObject.SetActive(false);
            MenuRegisterButton.gameObject.SetActive(true);
            MenuLoginButton.gameObject.SetActive(true);
            LogoutButton.gameObject.SetActive(false);
            notificationManager.OnSuccessMessage("Logged out");
            UserManager.Instance.username = null;
        }
        public void Register()
        {
            if (RegisterPasswordInput1.text != RegisterPasswordInput2.text)
            {
                notificationManager.OnErrorMessage("Passwords do not match!");
                return;
            }
            StartCoroutine(RegisterRoutine(RegisterUsernameInput.text, RegisterPasswordInput1.text));
        }

        private IEnumerator RegisterRoutine(string username, string password)
        {
            if (registerSpinner != null)
                registerSpinner.SetActive(true);
            RegisterButton.gameObject.SetActive(false);
            RegisterBackButton.SetActive(false);
            if (RegisterButton != null)
                RegisterButton.interactable = false;
            yield return api.RegisterRequest(username, password);

            if (registerSpinner != null)
                registerSpinner.SetActive(false);
            RegisterButton.gameObject.SetActive(true);
            RegisterBackButton.SetActive(true);
            if (RegisterButton != null)
                RegisterButton.interactable = true;
        }

        public void Login()
        {
            StartCoroutine(LoginRoutine(LoginUsernameInput.text, LoginPasswordInput.text));
        }

        private IEnumerator LoginRoutine(string username, string password)
        {
            if (loginSpinner != null)
                loginSpinner.SetActive(true);
            LoginButton.gameObject.SetActive(false);
            LoginBackButton.SetActive(false);
            if (LoginButton != null)
                LoginButton.interactable = false;

            yield return api.LoginRequest(username, password);

            if (loginSpinner != null)
                loginSpinner.SetActive(false);
            LoginButton.gameObject.SetActive(true);
            LoginBackButton.SetActive(true);
            if (LoginButton != null)
                LoginButton.interactable = true;
        }

        public void OpenMapEdiorScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MapEditor");
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
