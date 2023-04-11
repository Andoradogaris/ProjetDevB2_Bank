using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject loadingPanel;
    [SerializeField]
    private GameObject menuPanel;
    [SerializeField]
    private Slider slider;
    private bool isPaused;

    public GameObject pauseScreen;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Time.timeScale = 1f;
        loadingPanel.SetActive(false);
        menuPanel.SetActive(true);
    }
    public void Play()
    {
        StartCoroutine(LoadLevelAsync(1));
    }
    public void Quit()
    {
        Application.Quit();
    }

    IEnumerator LoadLevelAsync(int _sceneIndex)
    {

        AsyncOperation operation = SceneManager.LoadSceneAsync(_sceneIndex);

        loadingPanel.SetActive(true);
        menuPanel.SetActive(false);

        while (!operation.isDone)
        {

            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            slider.value = progress;

            yield return null;

        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().buildIndex == 1)
        {
            isPaused = !isPaused;

            if (isPaused)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }

    private void Pause()
    {
        Time.timeScale = 0f;
        pauseScreen.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        pauseScreen.SetActive(false);
    }
}