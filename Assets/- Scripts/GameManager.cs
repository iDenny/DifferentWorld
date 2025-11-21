using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the high level state of the game: main menu, starting scenarios,
/// loading scenes and handling game modes (sandbox, story, tutorial).
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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

    /// <summary>
    /// Called from UI buttons to start a new game.  Pass the name of the scene to load.
    /// </summary>
    public void StartNewGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Quit the application.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}