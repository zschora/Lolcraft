using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UICanvasController : MonoBehaviour
{
    [Header("Ссылки на UI")]
    [SerializeField] private GameObject menuPanel;

    [Header("Звук перезапуска")]
    [SerializeField] private AudioSource restartSound;
    [SerializeField] private float restartDelay = 1f;

    [Header("Визуальный эффект")]
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private Vector3 explosionPosition;
    [SerializeField] private float explosionDelay = 0.3f;

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
        // Проигрываем звук
        if (restartSound != null)
        {
            restartSound.Play();
        }

        // Запускаем корутину для взрыва с задержкой
        if (explosionEffectPrefab != null)
        {
            StartCoroutine(PlayExplosionAfterDelay(explosionDelay));
        }

        // Перезапуск сцены
        StartCoroutine(RestartWithDelay(restartDelay));
    }

    private System.Collections.IEnumerator PlayExplosionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject effect = Instantiate(explosionEffectPrefab, explosionPosition, Quaternion.identity);
        Animator anim = effect.GetComponent<Animator>();
        float animDuration = anim != null ? anim.GetCurrentAnimatorStateInfo(0).length : 1f;
        Destroy(effect, animDuration + 0.1f);
        if (isMenuOpen)
            CloseMenu();
    }

    private System.Collections.IEnumerator RestartWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToggleMenu()
    {
        if (menuPanel == null) return;

        if (isMenuOpen)
            CloseMenu();
        else
            OpenMenu();
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
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
