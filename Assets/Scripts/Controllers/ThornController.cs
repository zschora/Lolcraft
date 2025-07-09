using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornController : MonoBehaviour
{
    private ColliderSensor playerSensor;
    // Start is called before the first frame update
    void Start()
    {
        playerSensor = transform.Find("PlayerSensor").GetComponent<ColliderSensor>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerSensor.State())
        {
            Debug.Log("Наткнулся на колючку");
            var myPlayerCollision = playerSensor.myPlayerCollision;
            if (myPlayerCollision != null)
            {
                myPlayerCollision.Death();
            }
        }
    }
}
