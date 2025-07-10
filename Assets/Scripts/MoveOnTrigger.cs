using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("��������")]
    public Vector3 pointA;
    public Vector3 pointB;
    public float speed = 2f;

    [Header("������ �� ������")]
    public ButtonController triggerButton;

    private Vector3 target;

    void Start()
    {
        // ��������� ������� � ����� A
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
