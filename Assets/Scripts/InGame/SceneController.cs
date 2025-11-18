using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InGame
{
public class SceneController : MonoBehaviour
{
    public static SceneController Instance;
    public Animator transitionAnimator;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene()
    {
        transitionAnimator = GameObject.Find("Load").GetComponent<Animator>();
        StartCoroutine(LoadLevel());
    }

    IEnumerator LoadLevel()
    {
        transitionAnimator.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadSceneAsync("InGame");
        transitionAnimator.SetTrigger("Start");
    }
}
}