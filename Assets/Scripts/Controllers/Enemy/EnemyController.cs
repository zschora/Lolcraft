using UnityEngine;
/*
// Контроллер врага
public class EnemyController : MonoBehaviour
{
    [Header("Настройки движения")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;
    
    [Header("Точки патрулирования")]
    [SerializeField] private Transform[] patrolPoints;
    
    [Header("Цель")]
    [SerializeField] private Transform player;
    
    private StateMachine stateMachine;

    // Свойства для доступа из состояний
    public int CurrentPatrolIndex { get; set; } = 0;

    public float PatrolSpeed => patrolSpeed;
    public float ChaseSpeed => chaseSpeed;
    public float DetectionRange => detectionRange;
    public float AttackRange => attackRange;

    public Transform[] PatrolPoints => patrolPoints;

    public Transform Player => player;

    void Start()
    {
        // Создаем машину состояний
        stateMachine = new StateMachine();
        
        // Создаем и добавляем состояния
        stateMachine.AddState(new IdleState(this));
        stateMachine.AddState(new PatrolState(this));
        stateMachine.AddState(new ChaseState(this));
        stateMachine.AddState(new AttackState(this));
        
        // Устанавливаем начальное состояние
        stateMachine.ChangeState<IdleState>();
    }
    
    void Update()
    {
        // Обновляем текущее состояние
        stateMachine.Update();
    }
    
    // Методы для использования в состояниях
    public float GetDistanceToPlayer()
    {
        if (Player == null) return float.MaxValue;
        return Vector3.Distance(transform.position, Player.position);
    }
    
    public void MoveTowards(Vector3 target, float speed)
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * (speed * Time.deltaTime);
        
        // Поворот в сторону движения
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
    
    // Публичный доступ к StateMachine для состояний
    public void ChangeState<T>() where T : IState
    {
        stateMachine.ChangeState<T>();
    }
    
    // Визуализация для отладки
    void OnDrawGizmosSelected()
    {
        // Радиус обнаружения
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectionRange);
        
        // Радиус атаки
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}
*/