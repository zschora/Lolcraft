using System.Collections;
using UnityEngine;
using TMPro;

public class MeshWordSwitcher : MonoBehaviour
{
    [Header("Настройки текста")]
    [TextArea]
    public string fullText = "Welcome to the machine. The process has begun.";

    public float delayBeforeStart = 10f;     // Задержка перед началом
    public float wordInterval = 1f;          // Время показа каждого слова

    [Header("Компоненты")]
    public TextMeshPro textMesh;             // Текстовый компонент (TextMeshPro или UGUI)
    [SerializeField] private AudioSource wordSound; // Звук появления слова

    private void Start()
    {
        if (textMesh == null)
        {
            Debug.LogError("TextMeshPro не назначен!");
            return;
        }

        textMesh.text = "";
        StartCoroutine(WordSequence());
    }

    private IEnumerator WordSequence()
    {
        yield return new WaitForSeconds(delayBeforeStart);

        string[] words = fullText.Split(' ');

        foreach (string word in words)
        {
            textMesh.text = word;

            if (wordSound != null)
            {
                wordSound.Stop();   // На случай, если звук короткий и накладывается
                wordSound.Play();
            }

            yield return new WaitForSeconds(wordInterval);
        }

        // После последнего слова ждём и очищаем
        yield return new WaitForSeconds(wordInterval);
        textMesh.text = "";
    }
}
