using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UICanvasController : MonoBehaviour
{
    [Header("������ �� UI")]
    [SerializeField] private GameObject menuPanel; // ������ � ����
    private bool isMenuOpen = false;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
    }

    public void CloseMenu()
    {
        if (menuPanel == null) return;

        menuPanel.SetActive(false);
        isMenuOpen = false;
        EventSystem.current.SetSelectedGameObject(null);
    }


    public void QuitGame()
    {
#if UNITY_EDITOR
        // ���� � ��������� � ������ ���������� Play Mode
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // ���� ������ � ������� ����������
        Application.Quit();
#endif
    }
}
