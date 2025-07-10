using UnityEngine;

public class ColliderSensor2 : MonoBehaviour
{
    private int m_ColCount = 0;
    private float m_DisableTimer = 0;

    public PlayerController myPlayerCollision;

    private void OnEnable()
    {
        m_ColCount = 0;
        myPlayerCollision = null;
    }

    public bool State()
    {
        if (m_DisableTimer > 0)
            return false;

        return m_ColCount > 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponentInParent<PlayerController>();
        if (player != null)
        {
            m_ColCount++;
            myPlayerCollision = player;
            Debug.Log("Игрок вошёл, коллайдеров: " + m_ColCount);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var player = other.GetComponentInParent<PlayerController>();
        if (player != null)
        {
            m_ColCount = Mathf.Max(0, m_ColCount - 1);
            Debug.Log("Игрок вышел, коллайдеров осталось: " + m_ColCount);

            if (m_ColCount == 0)
            {
                myPlayerCollision = null;
                Debug.Log("Все части игрока вышли — кнопка отпущена");
            }
        }
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
