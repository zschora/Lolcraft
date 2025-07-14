using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    // Состояния игры
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver
    }

    public PlayerController myOriginPlayer;
    public PlayerController myCurrentPlayer;
    //public PlayerController myCurrentMonster;
    public System.Collections.Generic.List<PlayerController> myPlayers = new System.Collections.Generic.List<PlayerController>();
    public GameObject gameOverPanel;
    //public Button restartButton;
    public PlayerCameraController playerCameraController;
    public int deadMonsterCount = 0;


    private GameState currentState = GameState.MainMenu;
    
    // События для подписки других скриптов
    public delegate void OnStateChanged(GameState newState);
    public event OnStateChanged onStateChanged;

    
    // Свойство для получения текущего состояния
    public GameState CurrentState 
    { 
        get { return currentState; }
    }
    
    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
        var gameOverCanvas = GameObject.Find("GameOverCanvas");
        //restartButton.onClick.AddListener(RestartLevel);
        gameOverPanel.SetActive(true);
        gameOverPanel.SetActive(false);

        //DontDestroyOnLoad(gameOverPanel);
        //DontDestroyOnLoad(restartButton);
        DontDestroyOnLoad(gameOverCanvas);
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        Debug.Log("OnSceneLoaded" + ((mode == LoadSceneMode.Single) ? "Single" : "Additive"));
    }

    // Изменение состояния игры
    public void ChangeState(GameState newState)
    {
        if (currentState == newState)
        {
            return;
        }
        
        currentState = newState;
        
        // Вызываем событие
        onStateChanged?.Invoke(newState);
        
        // Логика для каждого состояния
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
    
    // Базовые методы управления игрой
    public void StartGame()
    {
        ChangeState(GameState.Playing);
    }
    
    public void PauseGame()
    {
        if (currentState == GameState.Playing)
            ChangeState(GameState.Paused);
    }
    
    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
            ChangeState(GameState.Playing);
    }
    
    public void GameOver()
    {
        ChangeState(GameState.GameOver);
    }
    
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    public void RestartLevel()
    {
        myCurrentPlayer = null;
        myOriginPlayer = null;
        myPlayers.Clear();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        gameOverPanel.SetActive(false);
        //ChangeState(GameState.Playing);
    }

    public void ShowGameOver()
    {
        if (gameOverPanel is not null)
        {
            //Debug.Log("ShowGameOver(), gameOverPanel not null");
            gameOverPanel.SetActive(true);
            //Time.timeScale = 0f; // останавливает игру
        } else
        {
            //Debug.Log("ShowGameOver(), gameOverPanel is null");
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
