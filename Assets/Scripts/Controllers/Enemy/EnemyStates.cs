using UnityEngine;
/*
// Состояние покоя
public class IdleState : IState
{
    private EnemyController enemy;
    private float idleTimer = 0f;
    private float idleDuration = 2f;
    
    public IdleState(EnemyController enemy)
    {
        this.enemy = enemy;
    }
    
    public void Enter()
    {
        Debug.Log("Вход в состояние Idle");
        idleTimer = 0f;
    }
    
    public void Execute()
    {
        idleTimer += Time.deltaTime;
        
        // Проверяем игрока
        if (enemy.GetDistanceToPlayer() <= enemy.DetectionRange)
        {
            enemy.ChangeState<ChaseState>();
            return;
        }
        
        // Переходим к патрулированию после ожидания
        if (idleTimer >= idleDuration)
        {
            enemy.ChangeState<PatrolState>();
        }
    }
    
    public void Exit()
    {
        Debug.Log("Выход из состояния Idle");
    }
}

// Состояние патрулирования
public class PatrolState : IState
{
    private EnemyController enemy;
    private Transform currentTarget;
    
    public PatrolState(EnemyController enemy)
    {
        this.enemy = enemy;
    }
    
    public void Enter()
    {
        Debug.Log("Вход в состояние Patrol");
        
        if (enemy.PatrolPoints.Length > 0)
        {
            currentTarget = enemy.PatrolPoints[enemy.CurrentPatrolIndex];
        }
    }
    
    public void Execute()
    {
        // Проверяем игрока
        if (enemy.GetDistanceToPlayer() <= enemy.DetectionRange)
        {
            enemy.ChangeState<ChaseState>();
            return;
        }
        
        // Патрулирование
        if (currentTarget != null)
        {
            enemy.MoveTowards(currentTarget.position, enemy.PatrolSpeed);
            
            // Проверяем достижение точки
            if (Vector3.Distance(enemy.transform.position, currentTarget.position) < 0.5f)
            {
                // Переходим к следующей точке
                enemy.CurrentPatrolIndex = (enemy.CurrentPatrolIndex + 1) % enemy.PatrolPoints.Length;
                enemy.ChangeState<IdleState>();
            }
        }
    }
    
    public void Exit()
    {
        Debug.Log("Выход из состояния Patrol");
    }
}

// Состояние преследования
public class ChaseState : IState
{
    private EnemyController enemy;
    
    public ChaseState(EnemyController enemy)
    {
        this.enemy = enemy;
    }
    
    public void Enter()
    {
        Debug.Log("Вход в состояние Chase");
    }
    
    public void Execute()
    {
        float distanceToPlayer = enemy.GetDistanceToPlayer();
        
        // Если игрок слишком далеко - возвращаемся к патрулированию
        if (distanceToPlayer > enemy.DetectionRange * 1.5f)
        {
            enemy.ChangeState<PatrolState>();
            return;
        }
        
        // Если игрок близко - атакуем
        if (distanceToPlayer <= enemy.AttackRange)
        {
            enemy.ChangeState<AttackState>();
            return;
        }
        
        // Преследуем игрока
        if (enemy.Player != null)
        {
            enemy.MoveTowards(enemy.Player.position, enemy.ChaseSpeed);
        }
    }
    
    public void Exit()
    {
        Debug.Log("Выход из состояния Chase");
    }
}

// Состояние атаки
public class AttackState : IState
{
    private EnemyController enemy;
    private float attackTimer = 0f;
    private float attackCooldown = 1f;
    
    public AttackState(EnemyController enemy)
    {
        this.enemy = enemy;
    }
    
    public void Enter()
    {
        Debug.Log("Вход в состояние Attack");
        attackTimer = 0f;
    }
    
    public void Execute()
    {
        attackTimer += Time.deltaTime;
        
        float distanceToPlayer = enemy.GetDistanceToPlayer();
        
        // Если игрок убежал - преследуем
        if (distanceToPlayer > enemy.AttackRange)
        {
            enemy.ChangeState<ChaseState>();
            return;
        }
        
        // Атакуем с интервалом
        if (attackTimer >= attackCooldown)
        {
            Attack();
            attackTimer = 0f;
        }
        
        // Поворачиваемся к игроку
        if (enemy.Player != null)
        {
            Vector3 lookDirection = (enemy.Player.position - enemy.transform.position).normalized;
            lookDirection.y = 0;
            if (lookDirection != Vector3.zero)
            {
                enemy.transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }
    }
    
    public void Exit()
    {
        Debug.Log("Выход из состояния Attack");
    }
    
    private void Attack()
    {
        Debug.Log("Враг атакует!");
        // Здесь можно добавить логику нанесения урона
        // Воспроизвести звук атаки
        // Запустить анимацию атаки
    }
}
*/