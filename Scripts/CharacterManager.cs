using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CharacterManager : MonoBehaviour
{

    [Header("Horizontal Movement Settings")]
    [SerializeField] private float walkSpeed = 1;
    
    [Header("Verticla Movement Settings")]
    private float gravity;


    [Header("Ground Check Settings")]
    private float jumpForce = 45;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Recoil")]
    // 이동에 걸리는 시간 (초)
    [SerializeField] public float moveDuration = 0.1f;
    
    [SerializeField] private Animator anim;

    private Rigidbody2D rb;
    PlayerStateList pState;
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
    }

    // Update is called once per frame
    private void Update()
    {
        GetInputs();
        Move();
        Jump();
        Flip();
        Attack();
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.tag == "Enemy")
        {
            Debug.Log("Hit");

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
            rb.velocity = new Vector2(rb.velocity.x, 0);
            pState.jumping = true;
        }


        // �� �κе� ������ GetInouts()��  ���� �������� �ʴ� ������ ���ؼ� ����غ��� ����.
        if (Input.GetButtonDown("Jump") && Grounded())
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce);
            pState.jumping = true;
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
}
