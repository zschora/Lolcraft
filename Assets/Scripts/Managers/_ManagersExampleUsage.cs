using UnityEngine;

public class _ManagersExampleUsage : MonoBehaviour
{
    void Start()
    {
        // Подписка на события
        InputManager.Instance.OnJumpPressed += HandleJump;
        InputManager.Instance.OnPausePressed += HandlePause;
        GameManager.Instance.onStateChanged += OnGameStateChanged;
    }
    
    void HandleJump()
    {
        // Воспроизводим звук прыжка
        AudioManager.Instance.PlaySound("Jump");
        Debug.Log("Прыжок!");
    }
    
    void HandlePause()
    {
        if (GameManager.Instance.CurrentState == GameManager.GameState.Playing)
        {
            GameManager.Instance.PauseGame();
            AudioManager.Instance.PauseMusic();
        }
        else if (GameManager.Instance.CurrentState == GameManager.GameState.Paused)
        {
            GameManager.Instance.ResumeGame();
            AudioManager.Instance.UnpauseMusic();
        }
    }
    
    void OnGameStateChanged(GameManager.GameState newState)
    {
        switch (newState)
        {
            case GameManager.GameState.Playing:
                AudioManager.Instance.PlayMusic("GameplayMusic");
                break;
            case GameManager.GameState.GameOver:
                AudioManager.Instance.PlaySound("GameOver");
                AudioManager.Instance.StopMusic();
                break;
        }
    }
    
    void OnDestroy()
    {
        // Отписка от событий
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnJumpPressed -= HandleJump;
            InputManager.Instance.OnPausePressed -= HandlePause;
        }
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.onStateChanged -= OnGameStateChanged;
        }
    }
}