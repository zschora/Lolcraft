using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : Singleton<GameManager>
{
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver
    }

    public PlayerController myOriginPlayer;
    public PlayerController myCurrentPlayer;
    public System.Collections.Generic.List<PlayerController> myPlayers = new();
    public GameObject gameOverPanel;
    public Button restartButton;
    public PlayerCameraController playerCameraController;
    public int deadMonsterCount = 0;

    [Header("Эффекты перезапуска")]
    [SerializeField] private AudioSource restartSound;
    [SerializeField] private float restartDelay = 1f;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private Transform explosionSpawnPoint;

    private GameState currentState = GameState.MainMenu;

    public GameState CurrentState => currentState;

    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
        var gameOverCanvas = GameObject.Find("GameOverCanvas");
        restartButton.onClick.AddListener(RestartWithEffects);
        gameOverPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        DontDestroyOnLoad(gameOverCanvas);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
    }

    public void ChangeState(GameState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        onStateChanged?.Invoke(newState);

        switch (newState)
        {
            case GameState.MainMenu:
                Time.timeScale = 1f;
                Debug.Log("Главное меню");
                break;

            case GameState.Playing:
                Time.timeScale = 1f;
                Debug.Log("Игра началась");
                gameOverPanel.SetActive(false);
                break;

            case GameState.Paused:
                Time.timeScale = 0f;
                Debug.Log("Игра на паузе");
                break;

            case GameState.GameOver:
                Time.timeScale = 0f;
                Debug.Log("Игра окончена");
                gameOverPanel.SetActive(true);
                break;
        }
    }

    public void StartGame() => ChangeState(GameState.Playing);
    public void PauseGame() { if (currentState == GameState.Playing) ChangeState(GameState.Paused); }
    public void ResumeGame() { if (currentState == GameState.Paused) ChangeState(GameState.Playing); }
    public void GameOver() => ChangeState(GameState.GameOver);
    public void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);

    public void RestartWithEffects()
    {
        StartCoroutine(PlayExplosionAndRestart());
    }

    private IEnumerator PlayExplosionAndRestart()
    {
        // звук
        if (restartSound != null)
        {
            GameObject soundObject = new GameObject("RestartSound");
            AudioSource tempAudio = soundObject.AddComponent<AudioSource>();
            tempAudio.clip = restartSound.clip;
            tempAudio.volume = restartSound.volume;
            tempAudio.outputAudioMixerGroup = restartSound.outputAudioMixerGroup;
            tempAudio.Play();

            DontDestroyOnLoad(soundObject);
            Destroy(soundObject, restartSound.clip.length + 0.1f);
        }

        // эффект взрыва
        if (explosionPrefab != null && explosionSpawnPoint != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, explosionSpawnPoint.position, Quaternion.identity);
            DontDestroyOnLoad(explosion);

            Animator anim = explosion.GetComponent<Animator>();
            if (anim != null)
            {
                anim.Play("Explosion");
                float duration = anim.GetCurrentAnimatorStateInfo(0).length;
                Destroy(explosion, duration + 0.1f);
            }
            else
            {
                Destroy(explosion, 1f);
            }
        }

        yield return new WaitForSeconds(restartDelay);

        RestartLevel();
    }

    public void RestartLevel()
    {
        myCurrentPlayer = null;
        myOriginPlayer = null;
        myPlayers.Clear();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public event OnStateChanged onStateChanged;
    public delegate void OnStateChanged(GameState newState);
}
