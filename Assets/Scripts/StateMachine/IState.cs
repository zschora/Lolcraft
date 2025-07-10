using UnityEngine;

// Интерфейс для всех состояний
abstract public class IState
{
    private float beginTime = 0.0f;
    protected bool debug = false;

    public virtual void Enter()      // Вызывается при входе в состояние
    {
        beginTime = Time.time;
    }

    abstract public void Execute();  // Вызывается каждый кадр
    abstract public void Exit();     // Вызывается при выходе из состояния

    // Время пребывания в состоянии
    public float ActiveTime => Time.time - beginTime;

}