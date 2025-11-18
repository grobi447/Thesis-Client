using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using UnityEngine.SceneManagement;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        public static bool GameIsPaused = false;
        public GameObject pauseMenuUI;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameIsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

        public void Resume()
        {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            GameIsPaused = false;
            if (SceneManager.GetActiveScene().name == "InGame")
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        void Pause()
        {
            if (SceneManager.GetActiveScene().name == "InGame")
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

            }
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            GameIsPaused = true;
        }

        public void LoadMenu()
        {
            Time.timeScale = 1f;
            SceneManager.sceneLoaded += OnMainMenuLoaded;
            SceneManager.LoadScene("MainMenu");
        }

        private void OnMainMenuLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnMainMenuLoaded;

            MenuManager menuManager = FindObjectOfType<MenuManager>();
            menuManager?.LoggedIn();
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}