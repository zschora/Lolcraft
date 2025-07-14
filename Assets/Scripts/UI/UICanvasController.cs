using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UICanvasController : MonoBehaviour
{
    [Header("Ссылки на UI")]
    [SerializeField] private GameObject menuPanel; // Канвас с меню
    private bool isMenuOpen = false;

    private void Start()
    {
        DontDestroyOnLoad(menuPanel);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }

        if (isMenuOpen && GameManager.Instance.menuOver != true)
        {
            GameManager.Instance.menuOver = true;
        }
    }

    public void RestartLevel()
    {
        GameManager.Instance.RestartLevel();
        //menuPanel.SetActive(false);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

   

    public void ToggleMenu()
    {
        if (menuPanel == null) return;

        if (isMenuOpen)
        {
            CloseMenu();
        }
        else
        {
            OpenMenu();
        }
    }

    public void OpenMenu()
    {
        if (menuPanel == null) return;

        menuPanel.SetActive(true);
        isMenuOpen = true;
        EventSystem.current.SetSelectedGameObject(null);
        GameManager.Instance.menuOver = true;
        GameManager.Instance.menuEntered = true;
    }

    public void CloseMenu()
    {
        if (menuPanel == null) return;

        menuPanel.SetActive(false);
        isMenuOpen = false;
        EventSystem.current.SetSelectedGameObject(null);
        GameManager.Instance.menuOver = false;
        GameManager.Instance.menuEntered = false;
    }


    public void QuitGame()
    {
#if UNITY_EDITOR
        // Если в редакторе — просто остановить Play Mode
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Если сборка — закрыть приложение
        Application.Quit();
#endif
    }
}
