using UnityEngine;
using System.Collections;

public class ColliderSensor : MonoBehaviour {

    private int m_ColCount = 0;

    private float m_DisableTimer = 0;

    public PlayerController myPlayerCollision;

    private void OnEnable()
    {
        m_ColCount = 0;
    }

    public bool State()
    {
        if (m_DisableTimer > 0)
        {
            return false;
        }

        return m_ColCount > 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        myPlayerCollision = other.GetComponent<PlayerController>();
        Debug.Log((myPlayerCollision != null) ? "Есть враг" : "Нет врага");
        m_ColCount++;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (myPlayerCollision != null) {
            myPlayerCollision = null;
        }
        Debug.Log((myPlayerCollision != null) ? "Есть враг" : "Нет врага");
        m_ColCount--;
    }

    void Update()
    {
        m_DisableTimer -= Time.deltaTime;
    }

    public void Disable(float duration)
    {
        m_DisableTimer = duration;
    }
}
