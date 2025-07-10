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
            Debug.LogError("�� ������ ColliderSensor �� �������� ������� 'PlayerSensorButton'");
        }
    }

    void Update()
    {
        if (playerSensor != null)
        {
            // ��������� ������ � ����������� ������������ � ������
            ButtonControllerPressed = playerSensor.State() && playerSensor.myPlayerCollision != null;

            if (ButtonControllerPressed)
            {
                Debug.Log("������ ������");
            }
        }
    }
}
