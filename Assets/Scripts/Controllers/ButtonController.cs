using UnityEngine;

public class ButtonController : MonoBehaviour
{
    private ColliderSensor2 playerSensor;

    public bool ButtonControllerPressed { get; private set; }

    [SerializeField] private AudioSource click;

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
    }

    void Update()
    {
        if (playerSensor != null)
        {
            bool isPressedNow = playerSensor.State() && playerSensor.myPlayerCollision != null;

            // Сохраняем состояние
            ButtonControllerPressed = isPressedNow;

            // Если кнопка была не нажата, а теперь нажата — играем звук
            if (!wasPressedLastFrame && isPressedNow)
            {
                click?.Play();
                Debug.Log("Кнопка нажата (звук)");
            }

            // Обновляем состояние для следующего кадра
            wasPressedLastFrame = isPressedNow;
        }
    }
}
