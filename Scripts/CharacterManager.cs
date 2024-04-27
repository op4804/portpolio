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
    [SerializeField] int recoilXSteps = 5;
    [SerializeField] int recoilYSteps = 5;
    [SerializeField] float recoilXSpeed = 100;
    [SerializeField] float recoilYSpeed = 100;
    
    [SerializeField] private Animator anim;

    private Rigidbody2D rb;
    PlayerStateList pState;
    private float xAxis;
    private float yAxis;
    bool attack = false;
    float timeBetweenAttack, timeSinceAttack;

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
        }
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
        }
        else if (xAxis > 0) 
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
        }
    }

    private void Move()
    {
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
    }

    void Recoil() 
    {
        if (pState.recoilingX) 
        {
          if (pState.lookingRight) 
          {
              rb.velocity = new Vector2(-recoilXSpeed, 0);
          }
          else
          {
              rb.velocity = new Vector2(recoilXSpeed, 0);
          }
        }

        if (pState.recoilingY)
        {
            rb.gravityScale = 0;
           if (yAxis < 0)
           {
              rb.velocity = new Vector2(rb.velocity.x, recoilYSpeed);
           }
           else
           {
              rb.velocity = new Vector2(rb.velocity.x, -recoilYSpeed);
           }
        }
        else
        {
            rb.gravityScale = gravity;
        }
    }

    public bool Grounded()
    {
        // �� �κ� �ڵ� ���ذ� ���� �� �ʿ��ؼ� �� �����ϰ� ����� ���� �� ����. 
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround) 
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround) 
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
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
