using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Движение")]
    public Vector3 pointA;
    public Vector3 pointB;
    public float speed = 2f;

    [Header("Ссылка на кнопку")]
    public ButtonController triggerButton;

    private Vector3 target;

    void Start()
    {
        // Начальная позиция — точка A
        transform.position = pointA;
        target = pointA;
    }

    void Update()
    {
        if (triggerButton != null && triggerButton.ButtonControllerPressed)
        {
            target = pointB;
        }
        else
        {
            target = pointA;
        }

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }
}
