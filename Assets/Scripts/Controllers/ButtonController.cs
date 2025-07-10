using UnityEngine;

public class ButtonController : MonoBehaviour
{
    private ColliderSensor2 playerSensor;

    public bool ButtonControllerPressed { get; private set; }

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
    }

    void Update()
    {
        if (playerSensor != null)
        {
            // Учитываем случай с несколькими коллайдерами у игрока
            ButtonControllerPressed = playerSensor.State() && playerSensor.myPlayerCollision != null;

            if (ButtonControllerPressed)
            {
                Debug.Log("Кнопка нажата");
            }
        }
    }
}
