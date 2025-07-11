using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{

    [SerializeField] float m_speed = 4.0f;
    //[SerializeField] float      m_jumpForce = 7.5f;
    //[SerializeField] float      m_rollForce = 6.0f;
    [SerializeField] bool m_noBlood = false;
    [SerializeField] GameObject m_slideDust;
    [SerializeField] BoxCollider2D lifeCollider;
    [SerializeField] BoxCollider2D deathCollider;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private StateMachine stateMachine;
    private ColliderSensor m_groundSensor;
    private ColliderSensor attackSensorLeft;
    private ColliderSensor attackSensorRight;

    //private bool                m_isWallSliding = false;
    //private bool                m_rolling = false;
    private int m_facingDirection = 1;
    private int m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;
    private float m_timeSinceBlock = 0.0f;
    private float m_delayToIdle = 0.0f;
    //private float               m_rollDuration = 8.0f / 14.0f;
    //private float               m_rollCurrentTime;
    private bool m_grounded = false;
    private bool isSleep = false;

    public bool isPlayable = true;
    public float hp = 50; // 60
    public float move_speed = 1; // 0.5
    public float min_hp_to_takeover = 1000;
    public bool isOrigin = false;

    private bool isRightOriented = true;
    private int colorState = 0;

    [Header("Attack settings")]
    public float damage = 10; // 25
    public float attackCooldown = 3f;
    [SerializeField] private float attackDelay = 2f; // когда именно ударить после начала анимации
    private bool attackStarted = false;
    [SerializeField] private float attackFeedback = 0.2f;
    private float attackFeedbackTimer = 0f;
    private float attackElapsedTime = 0f;
    public float blockCooldown = 0f;
    public float block_time = 1f;


    [Header("MonsterSettings")]
    // Monster settings
    public float switchDelayTime = 1f;
    private float switchTime = 0f;
    [SerializeField] private float maxDistanceToAttackDistance = 4;
    [SerializeField] private float detectionDistance = 25; // Как далеко "смотрим"
    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private float rayHeight = 1.2f;



    public bool IsGrounded
    {
        get { return m_groundSensor.State(); }
    }

    // Use this for initialization
    void Start()
    {
        if (isPlayable)
        {
            if (isOrigin)
            {
                GameManager.Instance.myOriginPlayer = this;
                GameManager.Instance.myCurrentPlayer = this;
            }
        }

        if (!GameManager.Instance.myPlayers.Contains(this))
        {
            GameManager.Instance.myPlayers.Add(this);
            Debug.Log($"Игрок добавлен, всего игроков: {GameManager.Instance.myPlayers.Count}");
        }

        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<ColliderSensor>();
        attackSensorLeft = transform.Find("AttackSensorLeft").GetComponent<ColliderSensor>();
        attackSensorRight = transform.Find("AttackSensorRight").GetComponent<ColliderSensor>();

        // Создаем машину состояний
        stateMachine = new StateMachine();
        // Добавляем состояния
        stateMachine.AddState(new PlayerIdleState(this));
        stateMachine.AddState(new PlayerWalkState(this));
        stateMachine.AddState(new PlayerRunState(this));
        stateMachine.AddState(new PlayerJumpState(this));
        stateMachine.AddState(new PlayerRollState(this));
        stateMachine.AddState(new PlayerBlockState(this));
        stateMachine.AddState(new PlayerAttackState(this));
        stateMachine.AddState(new PlayerDeathState(this));
        // Начальное состояние
        stateMachine.ChangeState<PlayerIdleState>();

        //m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<ColliderSensor>();
        //m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<ColliderSensor>();
        //m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<ColliderSensor>();
        //m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<ColliderSensor>();
    }

    public void ChangeState<T>() where T : IState
    {
        stateMachine.ChangeState<T>();
    }

    public bool IsInState<T>() where T : IState
    {
        return stateMachine.IsInState<T>();
    }

    private void OnMouseOver()
    {
        //Debug.Log("on mouse over");
        if (Input.GetMouseButtonDown(1))
        {
            //Debug.Log("takeover");
            if (hp < 0.1 || isPlayable)
            {
                return;
            }

            if (hp - min_hp_to_takeover > 0.1)
            {
                return;
            }

            isPlayable = true;
            GameManager.Instance.myCurrentPlayer.isPlayable = false;
            GameManager.Instance.myCurrentMonster = null;

            if (!GameManager.Instance.myCurrentPlayer.isOrigin)
            {
                GameManager.Instance.myCurrentPlayer.Death();

                if (isSleep)
                {
                    WakeUp();
                }
            }
            else
            {
                GameManager.Instance.myCurrentPlayer.Sleep();
            }

            GameManager.Instance.myCurrentPlayer = this;
            GameManager.Instance.playerCameraController.ChangeFollow(gameObject);
        }

        CheckMonsterState(CheckDeath());
    }

    public void Sleep()
    {
        isSleep = true;
        //Debug.Log("Уснул");
        ChangeState<PlayerDeathState>();
        m_animator.SetBool("noBlood", m_noBlood);
        m_animator.SetTrigger("Death");

        GetComponent<BoxCollider2D>().size = new Vector2(2.18f, 0.52f);
        GetComponent<BoxCollider2D>().offset = new Vector2(0f, 0.3f);
        gameObject.layer = LayerMask.NameToLayer("DeadBodies");
    }

    public void WakeUp()
    {
        isSleep = false;
        //Debug.Log("Проснулся");
        ChangeState<PlayerIdleState>();
        m_animator.SetTrigger("Hurt");
        GetComponent<BoxCollider2D>().size = new Vector2(0.88f, 1.26f);
        GetComponent<BoxCollider2D>().offset = new Vector2(0f, 0.68f);
        gameObject.layer = LayerMask.NameToLayer("Players");
    }

    private void Attack()
    {
        //Physics2D.CircleCast((Vector2)attackSensorLeft.transform., 0.5, new Vector2(0,1));
        var myAttackSensor = isRightOriented ? attackSensorRight : attackSensorLeft;
        //var myAttackSensor = attackSensorRight;
        if (myAttackSensor.State())
        {
            //Debug.Log("Есть враг");
            var myEnemy = myAttackSensor.myPlayerCollision;
            if (myEnemy != null && !myEnemy.IsInState<PlayerBlockState>())
            {
                myEnemy.MinusHP(damage);
            }
        }
    }

    private void MinusHP(float value)
    {
        if (hp < 0.01)
        {
            //Debug.Log("Уже мертв");
            return;
        }
        //Debug.Log($"ХП до атаки {hp}");
        hp = System.Math.Max(0, hp - value);
        bool isDead = CheckDeath();
        CheckMonsterState(isDead);
        Debug.Log($"Аттака на {value}, осталось {hp}, умер: {isDead}");
        
        attackFeedbackTimer = 0;
        var aChangeColor = new ChangeColor(GetComponent<SpriteRenderer>());
        aChangeColor.R = 1f;
        aChangeColor.G = 0f;
        aChangeColor.B = 0f;
        aChangeColor.A = 1f;
        aChangeColor.Change();
        Debug.Log("КРАСНЫЙ");

    }

    class ChangeColor
    {
        private SpriteRenderer m_renderer;

        public float R, G, B, A;
        public ChangeColor(SpriteRenderer renderer)
        {
            m_renderer = renderer;

            var aColor = m_renderer.color;
            R = aColor.r;
            G = aColor.g;
            B = aColor.b;
            A = aColor.a;
            Debug.Log($"r:{R}, g: {G}, b: {B}, a: {A}");
        }

        public void Change()
        {
            var aNewColor = new Color(R, G, B, A);
            m_renderer.color = aNewColor;
        }
    }

    void CheckMonsterState(bool isDead)
    {
        if (isOrigin)
        {
            return;
        }

        if (isDead)
        {
            if (colorState != 0)
            {
                var aChangeColor = new ChangeColor(GetComponent<SpriteRenderer>());
                aChangeColor.R = 1f;
                aChangeColor.G = 1f;
                aChangeColor.B = 1f;
                aChangeColor.A = 1f;
                aChangeColor.Change();

                colorState = 0;
                Debug.Log("color state to 0 before death");
            }

            return;
        }

        if (isPlayable)
        { // not dead after takeover
            if (colorState != 2)
            {
                var aChangeColor = new ChangeColor(GetComponent<SpriteRenderer>());
                aChangeColor.R = 240f / 255f;
                aChangeColor.G = 100f / 255f;
                aChangeColor.B = 100f / 255f;
                aChangeColor.Change();

                colorState = 2;
                Debug.Log("color state to 2");
            }
        }
        else if (hp - min_hp_to_takeover < 0.1)
        { // before takeover
            if (colorState != 1)
            {

                Debug.Log("color state to 1");
                var aChangeColor = new ChangeColor(GetComponent<SpriteRenderer>());
                aChangeColor.R = 216f / 255f;
                aChangeColor.G = 238f / 255f;
                aChangeColor.B = 97f / 255f;
                aChangeColor.Change();

                colorState = 1;
            }
        }
    }

    private bool CheckDeath()
    {
        if (hp < 0.01)
        {
            Death();
            return true;
        }
        return false;
    }

    public void Death(bool toAnimate = true)
    {
        if (isOrigin)
        {
            Debug.Log("ГГ умер");
            //GameManager.Instance.RestartLevel();
            GameManager.Instance.ShowGameOver();
        }
        else if (isPlayable)
        {

            Debug.Log("ГГ умер не в своем теле");
            GameManager.Instance.playerCameraController.ChangeFollow(GameManager.Instance.myOriginPlayer.gameObject);
            GameManager.Instance.myOriginPlayer.Death(false);
            GameManager.Instance.ShowGameOver();
        }

        hp = 0;
        Debug.Log("Умер");
        ChangeState<PlayerDeathState>();

        if (toAnimate)
        {
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
        }

        gameObject.layer = LayerMask.NameToLayer("DeadBodies");
        //GetComponent<Collider2D>().enabled = false;

        CheckMonsterState(true);
    }

    // Update is called once per frame
    void Update()
    {
        // Increase timers that controls attack and block
        m_timeSinceAttack += Time.deltaTime;
        m_timeSinceBlock += Time.deltaTime;
        attackFeedbackTimer += Time.deltaTime;

        /*
        // Increase timer that checks roll duration
        if (m_rolling)
            m_rollCurrentTime += Time.deltaTime;

        // Disable rolling if timer extends duration
        if (m_rollCurrentTime > m_rollDuration)
            m_rolling = false;
        */
        //Check if character just landed on the ground
        if (attackFeedbackTimer > attackFeedback && attackFeedbackTimer < attackFeedback+0.1)
        {
            Debug.Log("БЕЛЫЙ");
            var aChangeColor = new ChangeColor(GetComponent<SpriteRenderer>());
            aChangeColor.R = 1f;
            aChangeColor.G = 1f;
            aChangeColor.B = 1f;
            aChangeColor.A = 1f;
            aChangeColor.Change();
        }

        if (!m_grounded && m_groundSensor.State())
        {
            ChangeState<PlayerIdleState>();
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        if (!isPlayable && !isOrigin && !IsInState<PlayerDeathState>())
        {
            AIUpdate();
            return;
        }

        if (!isPlayable || IsInState<PlayerDeathState>())
        {
            //ToIdle();
            return;
        }

        // Check block
        if (IsInState<PlayerBlockState>())
        {
            if (stateMachine.GetCurrentState().ActiveTime > block_time)
            {
                ChangeState<PlayerIdleState>();
                var aRenderer = GetComponent<SpriteRenderer>();
                var aNewColor = aRenderer.color;
                aNewColor.a = 1f;
                aRenderer.color = aNewColor;
                //m_animator.SetBool("IdleBlock", false);
            }
            return;
            /*{
                ChangeState<PlayerIdleState>();
                m_animator.SetBool("IdleBlock", false);
            }*/
        }
        // -------------------------------------------------------------

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }

        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        // Move
        //if (!m_rolling)
        m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);
        if (m_body2d.velocity.x > 0.1)
        {
            isRightOriented = true;
        }
        else if (m_body2d.velocity.x < -0.1)
        {
            isRightOriented = false;
        }

        //Debug.Log($"Velocity: {m_body2d.velocity}");

        /*
        //Death
        if (Input.GetKeyDown("e") && !m_rolling)
        {
            ChangeState<PlayerDeathState>();
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
        }

        //Hurt
        else if (Input.GetKeyDown("q") && !m_rolling)
        {
            ChangeState<PlayerIdleState>();
            m_animator.SetTrigger("Hurt");
        }
        */
        //Attack
        if (Input.GetMouseButtonDown(0) && m_timeSinceAttack > attackCooldown) // && !m_rolling)
        {
            Attack();
            ChangeState<PlayerAttackState>();
            m_currentAttack++;

            // Loop back to one after third attack
            if (m_currentAttack > 3)
                m_currentAttack = 1;

            // Reset Attack combo if time since last attack is too large
            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            m_animator.SetTrigger("Attack" + m_currentAttack);

            // Reset timer
            m_timeSinceAttack = 0.0f;
        }

        // Block
        else if ((Input.GetKeyDown("left shift") || Input.GetKeyDown("right shift")) && m_timeSinceBlock > blockCooldown)
        {
            // ???????????
            m_body2d.velocity = Vector2.zero;

            ChangeState<PlayerBlockState>();

            var aRenderer = GetComponent<SpriteRenderer>();
            var aNewColor = aRenderer.color;
            aNewColor.a = 0.5f;
            aRenderer.color = aNewColor;
            //m_animator.SetTrigger("Block");
            //m_animator.SetBool("IdleBlock", true);
            m_timeSinceBlock = 0f;
        }

        /*

        //Jump
        
        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        else if (Input.GetKeyDown("space") && m_grounded && !m_rolling)
        {
            ChangeState<PlayerJumpState>();
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }

        // Roll
        else if (Input.GetKeyDown("left shift") && !m_rolling && !m_isWallSliding)
        {
            ChangeState<PlayerRollState>();
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
        }
        */

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            ChangeState<PlayerRunState>();
            // Reset timer
            m_delayToIdle = 0.05f;
        }

        //Idle
        else
        {
            ToIdle();
        }

        Animate();
    }

    private void ToIdle()
    {
        // Prevents flickering transitions to idle
        m_delayToIdle -= Time.deltaTime;
        if (m_delayToIdle < 0)
        {
            ChangeState<PlayerIdleState>();
        }
    }

    private void Animate()
    {
        if (IsInState<PlayerIdleState>())
        {
            Debug.Log("Animate PlayerIdleState()");
            m_animator.SetInteger("AnimState", 0);
        }
        else if (IsInState<PlayerRunState>())
        {
            Debug.Log("Animate PlayerRunState()");
            m_animator.SetInteger("AnimState", 1);
        }
        /*
        else if (IsInState<PlayerWalkState>())
        {
            ////////////////////////////////////////////////////////////////////////////
        }
        else if (IsInState<PlayerRunState>())
        {
            m_animator.SetInteger("AnimState", 1);
        }
        else if (IsInState<PlayerJumpState>())
        {
            m_animator.SetTrigger("Jump");
            m_animator.SetBool("Grounded", IsGrounded);
        }
        else if (IsInState<PlayerRollState>())
        {
            m_animator.SetTrigger("Roll");
        }
        else if (IsInState<PlayerBlockState>())
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }
        else
        {
            Debug.LogWarning("Unhandled state in Animate");
        }

        if (!IsInState<PlayerBlockState>())
        {
            m_animator.SetBool("IdleBlock", false);
        }
        */
    }

    void AIUpdate()
    {
        switchTime += Time.deltaTime;
        m_timeSinceAttack += Time.deltaTime;
        attackElapsedTime += Time.deltaTime;
        

        float distance;
        bool aPlayerDetected = DetectPlayer(out distance);
        Debug.Log($"aPlayerDetected: {aPlayerDetected}");


        if (aPlayerDetected && !IsInState<PlayerRunState>() && !IsInState<PlayerAttackState>())
        {
            if (GameManager.Instance.myCurrentMonster is null)
            {
                GameManager.Instance.myCurrentMonster = this;
                //Debug.Log("detected");
                ChangeState<PlayerRunState>();
            }
            //m_delayToIdle = 0.05f;
        } else if (!aPlayerDetected && !IsInState<PlayerIdleState>())
        {
            if (GameManager.Instance.myCurrentMonster == this)
            {
                GameManager.Instance.myCurrentMonster = null;
            }
            //Debug.Log("not player");
            ChangeState<PlayerIdleState>();
        }

        if (IsInState<PlayerIdleState>())
        {
            //Debug.Log("idle");
            SwitchDirection();
        } else if (IsInState<PlayerRunState>())
        {
            Debug.Log("run");
            if (!aPlayerDetected)
            {
                if (GameManager.Instance.myCurrentMonster == this)
                {
                    GameManager.Instance.myCurrentMonster = null;
                }
                ChangeState<PlayerIdleState>();
                return;
            }
            //Debug.Log("REALrun");
            // Идём к цели
            //float stopDistance = .1f;
            var myAttackSensor = isRightOriented ? attackSensorRight : attackSensorLeft;
            //var myAttackSensor = attackSensorRight;
            if (!myAttackSensor.State() || myAttackSensor.State() && myAttackSensor.myPlayerCollision is null)
            {
             //   if (true)//distance > stopDistance)
            //{
                m_body2d.velocity = new Vector2(m_facingDirection * m_speed * move_speed, m_body2d.velocity.y);
            } else
            {
                ChangeState<PlayerAttackState>();
            }
            /*
            else
            {
                m_body2d.velocity = new Vector2(0, m_body2d.velocity.y);
                ToIdle();

                // Атакуем, если можно
                if (m_timeSinceAttack > attackCooldown)
                {
                    Attack();
                    ChangeState<PlayerAttackState>();
                    m_currentAttack = (m_currentAttack % 3) + 1;
                    m_animator.SetTrigger("Attack" + m_currentAttack);
                    m_timeSinceAttack = 0;
                }
            }
            */
        } else if (IsInState<PlayerAttackState>())
        {
            //Debug.Log($"Distance: {distance}");
            if (!aPlayerDetected || distance > 1.1)
            {
                if (GameManager.Instance.myCurrentMonster == this)
                {
                    GameManager.Instance.myCurrentMonster = null;
                }

                ChangeState<PlayerIdleState>();
                return;
            }
            
            if (!attackStarted && m_timeSinceAttack > attackCooldown)
            {
                attackStarted = true;
                attackElapsedTime = 0f;

                // Reset timer  
                m_timeSinceAttack = 0.0f;
                m_currentAttack++;

                // Loop back to one after third attack
                if (m_currentAttack > 3)
                    m_currentAttack = 1;

                // Reset Attack combo if time since last attack is too large
                if (m_timeSinceAttack > 1.0f)
                    m_currentAttack = 1;

                // Call one of three attack animations "Attack1", "Attack2", "Attack3"
                m_animator.SetTrigger("Attack" + m_currentAttack);
            } else if (attackStarted && attackElapsedTime > attackDelay)
            {
                Attack();

                attackStarted = false;
            }
        }
        
        
        Animate();

        /*
        // Найти ближайшего живого игрока
        var players = GameManager.Instance.myPlayers
            .Where(p => p != null && p.isPlayable && p.hp > 0)
            .ToList();

        if (players.Count == 0)
            return;

        var target = players
            .OrderBy(p => Vector2.Distance(p.transform.position, transform.position))
            .First();

        float distance = Vector2.Distance(transform.position, target.transform.position);

        Vector2 direction = (target.transform.position - transform.position).normalized;

        // Поворачиваемся к цели
        if (direction.x > 0.1f)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
            isRightOriented = true;
        }
        else if (direction.x < -0.1f)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
            isRightOriented = false;
        }

        // Идём к цели
        float stopDistance = 1.5f;
        if (distance > stopDistance)
        {
            m_body2d.velocity = new Vector2(direction.x * m_speed * move_speed, m_body2d.velocity.y);
            ChangeState<PlayerRunState>();
        }
        else
        {
            m_body2d.velocity = new Vector2(0, m_body2d.velocity.y);
            ToIdle();

            // Атакуем, если можно
            if (m_timeSinceAttack > attackCooldown)
            {
                Attack();
                ChangeState<PlayerAttackState>();
                m_currentAttack = (m_currentAttack % 3) + 1;
                m_animator.SetTrigger("Attack" + m_currentAttack);
                m_timeSinceAttack = 0;
            }
        }
        */
    }

    private void SwitchDirection()
    {
        if (IsInState<PlayerDeathState>())
        {
            return;
        }

        if (switchTime > switchDelayTime)
        {
            if (isRightOriented)
            {
                GetComponent<SpriteRenderer>().flipX = true;
                m_facingDirection = -1;
                isRightOriented = false;
            }
            else
            {

                GetComponent<SpriteRenderer>().flipX = false;
                m_facingDirection = 1;
                isRightOriented = true;
            }

            switchTime = 0f;
        }
    }

    private bool DetectPlayer(out float distance)
    {
        distance = 0;
        Vector2 origin = transform.position;
        origin.y += rayHeight;
        Vector2 direction = new Vector2(m_facingDirection, 0);

        RaycastHit2D[] hit = Physics2D.RaycastAll(origin, direction, detectionDistance, detectionLayer);
        Debug.DrawRay(origin, direction * detectionDistance, Color.red);
        //Debug.Log($"Столкновений: {hit.Length}");
        foreach (var ray in hit)
        {
            if (ray.collider is null) continue;
            var aPlayer = ray.collider.gameObject.GetComponent<PlayerController>();

            if (aPlayer != null && aPlayer.isPlayable)
            {
                distance = ray.distance;
                if (distance > maxDistanceToAttackDistance)
                {
                    break;
                }

                //Debug.Log($"Обнаружен плеер на расстоянии {distance}, игрок: {aPlayer.isPlayable}");
                return aPlayer.isPlayable;
            }
        }

        return false;

    }
}
