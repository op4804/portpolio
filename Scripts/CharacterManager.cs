using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class CharacterManager : MonoBehaviour
{
    [Header("Horizontal Movement Settings")]
    [SerializeField] private float walkSpeed = 1;
    

    [Header("Vertical Movement Settings")]
    [SerializeField] private float jumpForce = 45;
    private float gravity;  
    private int jumpBufferCounter = 0;
    [SerializeField] private int jumpBufferFrames;
    private float coyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime;
    private int airJumpCounter = 0;
    [SerializeField] private int maxAirJumps;


    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Recoil Settings")]
    // 이동에 걸리는 시간 (초)
    [SerializeField] public float moveDuration = 0.1f;    
    [SerializeField] Animator anim;


    [Header("HP Settings")]
    [SerializeField] float hp = 10;
    [SerializeField] float curHp = 10;
    [SerializeField] Slider hpBar;



    [Header("Attack Settings")]
    [SerializeField] int knockbackSpeed = 10;
    // for enemy attack
    [SerializeField] float damage = 1;
    [SerializeField] float meleeCooltime = 0.5f;
    float timeBetweenAttack, timeSinceAttack;
    GameObject bullets;
    public GameObject bullet;
    public Transform bulletPos;
    public float bulletCooltime;
    
    private float curTime;


    [Header("Stage Setting")]
    [SerializeField] GameObject startPoint;
    [SerializeField] GameObject endPoint;


    // Noraml Variables
    private Rigidbody2D rb;
    PlayerStateList pState;
    SpriteRenderer sr;
    bool isHurt;
    bool isKnockback;
    Color halfA = new Color(1, 1, 1, 0.5f);
    Color fullA = new Color(1, 1, 1, 1);
    private float xAxis;
    bool attack = false;
    // 리코일의 이동할 목표 위치
    private Vector2 targetPosition;
    public static CharacterManager Instance;


    GameObject sm; 
    GameObject gameOverWindow;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        pState = GetComponent<PlayerStateList>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        // hpBar.value = (float)curHp / (float)hp;
        whatIsGround = LayerMask.GetMask("Ground");

        sm = GameObject.Find("StageManager");
        gameOverWindow = GameObject.Find("GameOverWindow");
    }

    // Update is called once per frame
    private void Update()
    {
        if (sm.GetComponent<StageManager>().isPaused)
        {
            Debug.Log("Paused!!!!");
        }
        else
        {
            GetInputs();
            UpdateJumpVariables();
            Move();
            Jump();
            Flip();
            Attack();
            ShotAttack();
        }

    }

    // Recoil when Player attack with weapon
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.tag == "Enemy")
        {
            Debug.Log("Hit");

            // 적 공격 부
            _other.GetComponent<Enemy>().Hit(damage);

            // 이부분 리코일 따로 빼줄 수 있나?
            float recoilPower = 2f;
            pState.recoilingX = true;

            if (transform.position.x - _other.transform.position.x > 0) 
            {
                // 목표 위치 설정
                targetPosition = transform.position + new Vector3(recoilPower, 0, 0);
            }
            else if (transform.position.x - _other.transform.position.x < 0)
            {
                // 목표 위치 설정
                targetPosition = transform.position + new Vector3(-recoilPower, 0, 0);
            }

            // Coroutine 시작
            StartCoroutine(MoveOverTime(targetPosition));
        }
    }


    // Recoil when Player gets hit with Enemy
    private void OnCollisionStay2D(Collision2D _other)
    {
        if (_other.gameObject.tag == "Enemy")
        {
            Debug.Log("Body Hit!");

            Hurt(_other.gameObject.GetComponent<Enemy>().enemyDamage, _other.transform.position);

            // if (transform.position.x - _other.transform.position.x > 0) 
            // {
            //     // 목표 위치 설정
            //     targetPosition = transform.position + new Vector3(2, 2, 0);
            // }
            // else if (transform.position.x - _other.transform.position.x < 0)
            // {
            //     // 목표 위치 설정
            //     targetPosition = transform.position + new Vector3(-2, 2, 0);
            // }

            // // Coroutine 시작
            // StartCoroutine(MoveOverTime());
        }
    }

    // 서서히 리코일하는 Coroutine
    private IEnumerator MoveOverTime(Vector2 _targetPosition)
    {
        float elapsedTime = 0f;
        Vector2 startPosition = transform.position;

        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;
            transform.position = Vector2.Lerp(startPosition, _targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 이동 완료 후 마지막 위치 설정 (목표 위치)
        transform.position = _targetPosition;
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        attack = Input.GetMouseButtonDown(0);
    }

    void Flip()
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-5, transform.localScale.y);
            pState.lookingRight = false;
        }
        else if (xAxis > 0) 
        {
            transform.localScale = new Vector2(5, transform.localScale.y);
            pState.lookingRight = true;
        }
    }

    private void Move()
    {
        if (this.transform.position.x < startPoint.transform.position.x)
        {
            StartCoroutine(Knockback(-1));
        }
        if (this.transform.position.x > endPoint.transform.position.x)
        {
            Debug.Log("StageClear!");
        }
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
        anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());
    }

    public bool Grounded()
    {

        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround) 
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround) 
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            pState.jumping = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    void Jump()
    {
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            pState.jumping = false; 
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        if (!pState.jumping)
        {
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce);
                pState.jumping = true;
            }
            else if (!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump"))
            {
                pState.jumping = true;
                airJumpCounter ++;
                rb.velocity = new Vector3(rb.velocity.x, jumpForce);
            }
            anim.SetBool("Jumping", !Grounded());
        }
    }

    void UpdateJumpVariables()
    {
        if (Grounded())
        {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else 
        {
            jumpBufferCounter--;
        }
    }

    void Attack()
    {
        if (curTime <= 0)
        {
            timeSinceAttack = Time.deltaTime;

            if (attack && timeSinceAttack >= timeBetweenAttack)
            {
                timeSinceAttack = 0;
                anim.SetTrigger("Attack");
                curTime = meleeCooltime;
            }
        }
        else
        {
            curTime -= Time.deltaTime;
        }
    }

    void ShotAttack()
    {
        if (curTime <= 0)
        {
            if (Input.GetKey(KeyCode.Z))
            {
                Debug.Log("shot!");
                bullets = Instantiate(bullet, bulletPos.position, transform.rotation);
                bullets.GetComponent<bullet>().dir = pState.lookingRight;
                curTime = bulletCooltime;
            }
            
        }
        curTime -= Time.deltaTime;
    }

    // Player gets damage function
    public void Hurt(float damage, Vector2 pos)
    {
        if (!isHurt)
        {
            isHurt = true;
            curHp = curHp - damage;
            if (curHp <= 0)
            {
                hpBar.value = 0;
                // player dead
                
                gameOverWindow.SetActive(true);
                
                Destroy(gameObject);              
                
            }
            else
            {
                HandleHp();
                float x = transform.position.x - pos.x;
                if (x < 0)
                  x = 1;
                else
                  x = -1;

                StartCoroutine(Knockback(x));
                StartCoroutine(Invulnerable());
                StartCoroutine(AlphaBlink());
            }
        }
    }

    private void HandleHp()
    {
        hpBar.value = (float)curHp / (float)hp;
    }

    // Recoil Function
    private IEnumerator Knockback(float dir)
    {
        isKnockback = true;
        float ctime = 0;
        while(ctime < 0.2f)
        {
            if (transform.rotation.y == 0)
            {
                transform.Translate(Vector2.left * knockbackSpeed * Time.deltaTime * dir);
            }
            else
            {
                transform.Translate(Vector2.left * knockbackSpeed * Time.deltaTime * -1f * dir);
            }

            ctime += Time.deltaTime;
            yield return null;
        }
        isKnockback = false;
    }

    // Player blinks while player's status is invulnerable
    private IEnumerator AlphaBlink()
    {
        while(isHurt)
        {
            yield return new WaitForSeconds(0.1f);
            sr.color = halfA;
            yield return new WaitForSeconds(0.1f);
            sr.color = fullA;
        }
    }
    
    // Turning player's status to invulnerable for a second
    private IEnumerator Invulnerable()
    {
        yield return new WaitForSeconds(1f);
        isHurt = false;
    }
}