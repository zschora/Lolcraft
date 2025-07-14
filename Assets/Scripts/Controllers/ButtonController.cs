using UnityEngine;

public class ButtonController : MonoBehaviour
{
    private ColliderSensor2 playerSensor;

    public bool ButtonControllerPressed { get; private set; }

    [Header("����")]
    [SerializeField] private AudioSource click;

    [Header("������� ������")]
    [SerializeField] private SpriteRenderer baseRenderer;     // �� ������
    [SerializeField] private SpriteRenderer pressedRenderer;  // ������

    private bool wasPressedLastFrame = false;

    void Start()
    {
        Transform sensorTransform = transform.Find("PlayerSensorButton");
        if (sensorTransform != null)
        {
            playerSensor = sensorTransform.GetComponent<ColliderSensor2>();
        }

        if (playerSensor == null)
        {
            Debug.LogError("�� ������ ColliderSensor �� �������� ������� 'PlayerSensorButton'");
        }

        ButtonControllerPressed = false;
        UpdateRender(false); // �� ���������: �� ������
    }

    void Update()
    {
        if (playerSensor == null) return;

        bool isPressedNow = playerSensor.State() && playerSensor.myPlayerCollision != null;
        ButtonControllerPressed = isPressedNow;

        if (!wasPressedLastFrame && isPressedNow)
        {
            click?.Play();
            Debug.Log("������ ������ (����)");
        }

        if (wasPressedLastFrame != isPressedNow)
        {
            UpdateRender(isPressedNow);
        }

        wasPressedLastFrame = isPressedNow;
    }

    private void UpdateRender(bool pressed)
    {
        if (baseRenderer != null) baseRenderer.enabled = !pressed;
        if (pressedRenderer != null) pressedRenderer.enabled = pressed;
    }
}
