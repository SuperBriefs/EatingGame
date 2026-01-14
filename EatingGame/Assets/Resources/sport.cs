using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("移动参数")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float airControlMultiplier = 0.8f;

    [Header("地面检测")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private LayerMask groundLayer;

    [Header("组件引用")]
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer sprite;

    private Rigidbody2D rb;
    private float dirX;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (anim == null)
            anim = GetComponent<Animator>();
        if (sprite == null)
            sprite = GetComponent<SpriteRenderer>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.freezeRotation = true;
        }
    }

    void Update()
    {
        // 获取输入
        dirX = Input.GetAxisRaw("Horizontal");

        // 跳跃检测
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        // 小跳效果
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        // 更新动画状态
        UpdateAnimationState();
    }

    void FixedUpdate()
    {
        Move();
        CheckGround();
    }

    void Move()
    {
        float currentMoveSpeed = moveSpeed;

        if (!isGrounded)
        {
            currentMoveSpeed *= airControlMultiplier;
        }

        Vector2 velocity = rb.velocity;
        velocity.x = dirX * currentMoveSpeed;
        rb.velocity = velocity;
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        isGrounded = false;

        if (anim != null)
        {
            anim.SetBool("jumping", true);
        }
    }

    void CheckGround()
    {
        if (groundCheck != null)
        {
            bool wasGrounded = isGrounded;
            isGrounded = Physics2D.OverlapBox(
                groundCheck.position,
                groundCheckSize,
                0f,
                groundLayer
            );

            if (anim != null)
            {
                anim.SetBool("grounded", isGrounded);
                anim.SetBool("jumping", !isGrounded);
            }
        }
    }

    private void UpdateAnimationState()
    {
        if (anim == null) return;

        // **关键修复：统一使用"running"参数（注意拼写）**
        // 检测水平移动
        bool isMoving = Mathf.Abs(dirX) > 0.1f;
        anim.SetBool("running", isMoving);

        // 翻转精灵
        if (sprite != null && isMoving)
        {
            sprite.flipX = dirX < 0f;
        }

       
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }
    }
}