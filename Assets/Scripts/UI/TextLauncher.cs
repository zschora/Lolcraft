using System.Collections;
using UnityEngine;
using TMPro;

public class MeshWordSwitcher : MonoBehaviour
{
    [Header("��������� ������")]
    [TextArea]
    public string fullText = "Welcome to the machine. The process has begun.";

    public float delayBeforeStart = 10f;     // �������� ����� �������
    public float wordInterval = 1f;          // ����� ������ ������� �����

    [Header("����������")]
    public TextMeshPro textMesh;             // ��������� ��������� (TextMeshPro ��� UGUI)
    [SerializeField] private AudioSource wordSound; // ���� ��������� �����

    private void Start()
    {
        if (textMesh == null)
        {
            Debug.LogError("TextMeshPro �� ��������!");
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
                wordSound.Stop();   // �� ������, ���� ���� �������� � �������������
                wordSound.Play();
            }

            yield return new WaitForSeconds(wordInterval);
        }

        // ����� ���������� ����� ��� � �������
        yield return new WaitForSeconds(wordInterval);
        textMesh.text = "";
    }
}
