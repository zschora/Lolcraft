using UnityEngine;

// Состояние покоя игрока
public class PlayerIdleState : IState
{
    private PlayerController player;
    
    public PlayerIdleState(PlayerController player)
    {
        this.player = player;
    }

    public override void Enter()
    {
        base.Enter();
        if (debug) Debug.Log("Игрок в состоянии покоя");
    }
    
    public override void Execute()
    {
        // Получаем ввод
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // Проверяем переходы
        if (Input.GetKeyDown(KeyCode.Space) && player.IsGrounded)
        {
            player.ChangeState<PlayerJumpState>();
        }
        else if (horizontal != 0 || vertical != 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                player.ChangeState<PlayerRunState>();
            }
            else
            {
                player.ChangeState<PlayerWalkState>();
            }
        }
    }
    
    public override void Exit()
    {
        // Ничего не делаем при выходе
    }
}

// Состояние ходьбы
public class PlayerWalkState : IState
{
    private PlayerController player;
    
    public PlayerWalkState(PlayerController player)
    {
        this.player = player;
    }
    
    public override void Enter()
    {
        base.Enter();
        if (debug) Debug.Log("Игрок идет");
    }
    
    public override void Execute()
    {
        // Получаем ввод
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
        
        // Движение
        //player.Move(direction, player.WalkSpeed);
        
        // Проверяем переходы
        if (Input.GetKeyDown(KeyCode.Space) && player.IsGrounded)
        {
            player.ChangeState<PlayerJumpState>();
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            player.ChangeState<PlayerRunState>();
        }
        else if (direction == Vector3.zero)
        {
            player.ChangeState<PlayerIdleState>();
        }
    }
    
    public override void Exit()
    {
        // Ничего не делаем при выходе
    }
}

// Состояние бега
public class PlayerRunState : IState
{
    private PlayerController player;
    
    public PlayerRunState(PlayerController player)
    {
        this.player = player;
    }
    
    public override void Enter()
    {
        base.Enter();
        if (debug) Debug.Log("Игрок бежит");
    }
    
    public override void Execute()
    {
        // Получаем ввод
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
        
        // Движение
        //player.Move(direction, player.RunSpeed);
        
        // Проверяем переходы
        if (Input.GetKeyDown(KeyCode.Space) && player.IsGrounded)
        {
            player.ChangeState<PlayerJumpState>();
        }
        else if (!Input.GetKey(KeyCode.LeftShift) && direction != Vector3.zero)
        {
            player.ChangeState<PlayerWalkState>();
        }
        else if (direction == Vector3.zero)
        {
            player.ChangeState<PlayerIdleState>();
        }
    }
    
    public override void Exit()
    {
        // Ничего не делаем при выходе
    }
}

// Состояние прыжка
public class PlayerJumpState : IState
{
    private PlayerController player;
    private float jumpTimer = 0f;
    
    public PlayerJumpState(PlayerController player)
    {
        this.player = player;
    }
    
    public override void Enter()
    {
        base.Enter();
        if (debug) Debug.Log("Игрок прыгает");
        //player.Jump();
        jumpTimer = 0f;
    }
    
    public override void Execute()
    {
        jumpTimer += Time.deltaTime;
        
        // Движение в воздухе
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
        
        // Уменьшенная скорость в воздухе
        //player.Move(direction, player.WalkSpeed * 0.7f);
        
        // Проверяем приземление
        if (player.IsGrounded && jumpTimer > 0.1f)
        {
            if (direction != Vector3.zero)
            {
                player.ChangeState<PlayerWalkState>();
            }
            else
            {
                player.ChangeState<PlayerIdleState>();
            }
        }
    }
    
    public override void Exit()
    {
        // Ничего не делаем при выходе
    }
}

// Состояние кувырка
public class PlayerRollState : IState
{
    private PlayerController player;
    private float jumpTimer = 0f;

    public PlayerRollState(PlayerController player)
    {
        this.player = player;
    }

    public override void Enter()
    {
        base.Enter();
        if (debug) Debug.Log("Игрок кувыркается");
        //player.Jump();
        //jumpTimer = 0f;
    }

    public override void Execute()
    {
        /*
        jumpTimer += Time.deltaTime;

        // Движение в воздухе
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        // Уменьшенная скорость в воздухе
        player.Move(direction, player.WalkSpeed * 0.7f);

        // Проверяем приземление
        if (player.IsGrounded && jumpTimer > 0.1f)
        {
            if (direction != Vector3.zero)
            {
                player.ChangeState<PlayerWalkState>();
            }
            else
            {
                player.ChangeState<PlayerIdleState>();
            }
        }
        */
    }

    public override void Exit()
    {
        // Ничего не делаем при выходе
    }
}

// Состояние блока
public class PlayerBlockState : IState
{
    private PlayerController player;
    private float jumpTimer = 0f;

    public PlayerBlockState(PlayerController player)
    {
        this.player = player;
    }

    public override void Enter()
    {
        base.Enter();
        if (debug) Debug.Log("Игрок в блоке");
        //player.Jump();
        //jumpTimer = 0f;
    }

    public override void Execute()
    {
        /*
        jumpTimer += Time.deltaTime;

        // Движение в воздухе
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        // Уменьшенная скорость в воздухе
        player.Move(direction, player.WalkSpeed * 0.7f);

        // Проверяем приземление
        if (player.IsGrounded && jumpTimer > 0.1f)
        {
            if (direction != Vector3.zero)
            {
                player.ChangeState<PlayerWalkState>();
            }
            else
            {
                player.ChangeState<PlayerIdleState>();
            }
        }
        */
    }

    public override void Exit()
    {
        // Ничего не делаем при выходе
    }
}

// Состояние атаки
public class PlayerAttackState : IState
{
    private PlayerController player;
    private float jumpTimer = 0f;

    public PlayerAttackState(PlayerController player)
    {
        this.player = player;
    }

    public override void Enter()
    {
        base.Enter();
        if (debug) Debug.Log("Игрок атакует");
        //player.Jump();
        //jumpTimer = 0f;
    }

    public override void Execute()
    {
        /*
        jumpTimer += Time.deltaTime;

        // Движение в воздухе
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        // Уменьшенная скорость в воздухе
        player.Move(direction, player.WalkSpeed * 0.7f);

        // Проверяем приземление
        if (player.IsGrounded && jumpTimer > 0.1f)
        {
            if (direction != Vector3.zero)
            {
                player.ChangeState<PlayerWalkState>();
            }
            else
            {
                player.ChangeState<PlayerIdleState>();
            }
        }
        */
    }

    public override void Exit()
    {
        // Ничего не делаем при выходе
    }
}

// Состояние смерти
public class PlayerDeathState : IState
{
    private PlayerController player;
    private float jumpTimer = 0f;

    public PlayerDeathState(PlayerController player)
    {
        this.player = player;
    }

    public override void Enter()
    {
        base.Enter();
        if (debug) Debug.Log("Игрок умер");
        //player.Jump();
        //jumpTimer = 0f;
    }

    public override void Execute()
    {
        /*
        jumpTimer += Time.deltaTime;

        // Движение в воздухе
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        // Уменьшенная скорость в воздухе
        player.Move(direction, player.WalkSpeed * 0.7f);

        // Проверяем приземление
        if (player.IsGrounded && jumpTimer > 0.1f)
        {
            if (direction != Vector3.zero)
            {
                player.ChangeState<PlayerWalkState>();
            }
            else
            {
                player.ChangeState<PlayerIdleState>();
            }
        }
        */
    }

    public override void Exit()
    {
        // Ничего не делаем при выходе
    }
}