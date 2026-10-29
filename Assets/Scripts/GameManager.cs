using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int world { get; private set; }
    public int stage { get; private set; }
    public int lives { get; private set; }
    public int coins { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 60;

        lives = 3;
        coins = 0;
        world = 1;
        string scene = SceneManager.GetActiveScene().name;
        stage = scene.StartsWith("1-") && int.TryParse(scene.Substring(2), out int number) ? number : 0;
    }

    public void NewGame()
    {
        StartLevel(1);
    }

    public void StartLevel(int number)
    {
        CancelInvoke();
        Time.timeScale = 1;
        lives = 3;
        coins = 0;
        LoadLevel(1, Mathf.Clamp(number, 1, 4));
    }

    public void ShowMenu()
    {
        CancelInvoke();
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    public void GameOver()
    {
        CancelInvoke();
        SceneManager.LoadScene("GameOver");
    }

    public void LoadLevel(int world, int stage)
    {
        this.world = world;
        this.stage = stage;

        SceneManager.LoadScene($"{world}-{stage}");
    }

    public void NextLevel()
    {
        LoadLevel(world, stage + 1);
    }

    public void ResetLevel(float delay)
    {
        CancelInvoke(nameof(ResetLevel));
        Invoke(nameof(ResetLevel), delay);
    }

    public void ResetLevel()
    {
        lives--;

        if (lives > 0) {
            LoadLevel(world, stage);
        } else {
            GameOver();
        }
    }

    public void AddCoin()
    {
        coins++;

        if (coins == 100)
        {
            coins = 0;
            AddLife();
        }
    }

    public void AddLife()
    {
        lives++;
    }

}

