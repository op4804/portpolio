using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    
    [SerializeField] private Animator anim;



    [Header("HP & Attack Settings")]
    [SerializeField] int hp = 10;
    [SerializeField] int knockbackSpeed = 10;
    // for enemy attack
    [SerializeField] float damage = 1;
    public GameObject bullet;
    public Transform bulletPos;
    public float bulletCooltime;
    private float curTime;


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
    float timeBetweenAttack, timeSinceAttack;
    // 리코일의 이동할 목표 위치
    private Vector2 targetPosition;
    public static CharacterManager Instance;




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
    }

    // Update is called once per frame
    private void Update()
    {
        GetInputs();
        UpdateJumpVariables( );
        Move();
        Jump();
        Flip();
        Attack();
        ShotAttack();
    }

    // Recoil when Player attack with weapon
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.tag == "Enemy")
        {
            Debug.Log("Hit");

            // 적 공격 부
            _other.GetComponent<Enemy>().Hit(damage);



            pState.recoilingX = true;

            if (transform.position.x - _other.transform.position.x > 0) 
            {
                // 목표 위치 설정
                targetPosition = transform.position + new Vector3(4, 0, 0);
            }
            else if (transform.position.x - _other.transform.position.x < 0)
            {
                // 목표 위치 설정
                targetPosition = transform.position + new Vector3(-4, 0, 0);
            }

            // Coroutine 시작
            StartCoroutine(MoveOverTime());
        }
    }


    // Recoil when Player gets hit with Enemy
    private void OnCollisionStay2D(Collision2D _other)
    {
        if (_other.gameObject.tag == "Enemy")
        {
            Debug.Log("Body Hit!");

            Hurt(1, _other.transform.position);

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
    private IEnumerator MoveOverTime()
    {
        float elapsedTime = 0f;
        Vector2 startPosition = transform.position;

        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;
            transform.position = Vector2.Lerp(startPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 이동 완료 후 마지막 위치 설정 (목표 위치)
        transform.position = targetPosition;
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
            transform.localScale = new Vector2(1, transform.localScale.y);
            pState.lookingRight = false;
        }
        else if (xAxis > 0) 
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
            pState.lookingRight = true;
        }
    }

    private void Move()
    {
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
    }

    public bool Grounded()
    {
        // �� �κ� �ڵ� ���ذ� ���� �� �ʿ��ؼ� �� �����ϰ� ����� ���� �� ����. 
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
        timeSinceAttack = Time.deltaTime;

        if (attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            anim.SetTrigger("Attack");
        }
    }

    void ShotAttack()
    {
        if (curTime <= 0)
        {
            if (Input.GetKey(KeyCode.Z))
            {
                Debug.Log("shot!");
                Instantiate(bullet, bulletPos.position, transform.rotation);
            }
            curTime = bulletCooltime;
        }
        curTime -= Time.deltaTime;
    }

    // Player gets damage function
    public void Hurt(int damage, Vector2 pos)
    {
        if (!isHurt)
        {
            isHurt = true;
            hp = hp - damage;
            if (hp < 0)
            {
                Destroy(gameObject);
            }
            else
            {
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