using UnityEngine;

public class ButtonController : MonoBehaviour
{
    private ColliderSensor2 playerSensor;

    public bool ButtonControllerPressed { get; private set; }

    [Header("Звук")]
    [SerializeField] private AudioSource click;

    [Header("Рендеры кнопки")]
    [SerializeField] private SpriteRenderer baseRenderer;     // не нажата
    [SerializeField] private SpriteRenderer pressedRenderer;  // нажата

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
            Debug.LogError("Не найден ColliderSensor на дочернем объекте 'PlayerSensorButton'");
        }

        ButtonControllerPressed = false;
        UpdateRender(false); // по умолчанию: не нажата
    }

    void Update()
    {
        if (playerSensor == null) return;

        bool isPressedNow = playerSensor.State() && playerSensor.myPlayerCollision != null;
        ButtonControllerPressed = isPressedNow;

        if (!wasPressedLastFrame && isPressedNow)
        {
            click?.Play();
            Debug.Log("Кнопка нажата (звук)");
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
