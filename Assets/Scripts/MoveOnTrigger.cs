using UnityEngine;

public class MoveOnTrigger : MonoBehaviour
{
    [Header("Движение")]
    public Vector3 pointA;
    public Vector3 pointB;
    public float speed = 2f;

    [Header("Ссылка на кнопку")]
    public ButtonController triggerButton;

    [Header("Звук движения")]
    [SerializeField] private AudioSource audioToB;
    [SerializeField] private AudioSource audioToA;

    private Vector3 target;
    private bool isMoving;

    void Start()
    {
        transform.position = pointA;
        target = pointA;
        isMoving = false;

        // отключаем автозапуск звука
        if (audioToA != null) audioToA.Stop();
        if (audioToB != null) audioToB.Stop();
    }

    void Update()
    {
        Vector3 previousPosition = transform.position;

        // Определяем направление
        if (triggerButton != null && triggerButton.ButtonControllerPressed)
        {
            target = pointB;
        }
        else
        {
            target = pointA;
        }

        // Перемещаем платформу
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        bool isNowMoving = (transform.position != previousPosition);

        // Если движется
        if (isNowMoving)
        {
            // Остановка любого ненужного звука перед запуском нужного
            if (target == pointB)
            {
                if (audioToA != null && audioToA.isPlaying) audioToA.Stop();
                if (audioToB != null && !audioToB.isPlaying) audioToB.Play();
            }
            else // target == pointA
            {
                if (audioToB != null && audioToB.isPlaying) audioToB.Stop();
                if (audioToA != null && !audioToA.isPlaying) audioToA.Play();
            }

            isMoving = true;
        }
        else if (isMoving)
        {
            // Только что остановились — выключаем всё
            if (audioToA != null) audioToA.Stop();
            if (audioToB != null) audioToB.Stop();
            isMoving = false;
        }
    }
}
