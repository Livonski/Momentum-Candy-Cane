using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [SerializeField] private string _gameScene;
    [SerializeField] private string _mainMenuScene;

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(_mainMenuScene);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(_gameScene);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
}
