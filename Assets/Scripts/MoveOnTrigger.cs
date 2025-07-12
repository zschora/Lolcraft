using UnityEngine;

public class MoveOnTrigger : MonoBehaviour
{
    [Header("��������")]
    public Vector3 pointA;
    public Vector3 pointB;
    public float speed = 2f;

    [Header("������ �� ������")]
    public ButtonController triggerButton;

    [Header("���� ��������")]
    [SerializeField] private AudioSource audioToB;
    [SerializeField] private AudioSource audioToA;

    private Vector3 target;
    private bool isMoving;

    void Start()
    {
        transform.position = pointA;
        target = pointA;
        isMoving = false;

        // ��������� ���������� �����
        if (audioToA != null) audioToA.Stop();
        if (audioToB != null) audioToB.Stop();
    }

    void Update()
    {
        Vector3 previousPosition = transform.position;

        // ���������� �����������
        if (triggerButton != null && triggerButton.ButtonControllerPressed)
        {
            target = pointB;
        }
        else
        {
            target = pointA;
        }

        // ���������� ���������
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        bool isNowMoving = (transform.position != previousPosition);

        // ���� ��������
        if (isNowMoving)
        {
            // ��������� ������ ��������� ����� ����� �������� �������
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
            // ������ ��� ������������ � ��������� ��
            if (audioToA != null) audioToA.Stop();
            if (audioToB != null) audioToB.Stop();
            isMoving = false;
        }
    }
}
