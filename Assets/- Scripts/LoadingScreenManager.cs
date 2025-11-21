using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles asynchronous scene loading with a rotating loading screen image and progress bar.
/// </summary>
public class LoadingScreenManager : MonoBehaviour
{
    public Image loadingImage;
    public Slider progressBar;
    public Sprite[] loadingScreens;

    private static LoadingScreenManager _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadAsync(sceneName));
    }

    private IEnumerator LoadAsync(string sceneName)
    {
        // Select a random loading screen sprite
        if (loadingScreens != null && loadingScreens.Length > 0)
        {
            loadingImage.sprite = loadingScreens[Random.Range(0, loadingScreens.Length)];
        }
        loadingImage.gameObject.SetActive(true);
        progressBar.gameObject.SetActive(true);

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            progressBar.value = progress;
            yield return null;
        }
        loadingImage.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(false);
    }
}