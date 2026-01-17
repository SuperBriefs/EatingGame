using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    private Animator anim;

    [SerializeField] private LayerMask jumpableGround;

    private float dirX = 0f;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 7f;

    [SerializeField] private LayerMask obstacleLayer; //需检测的障碍物图层（如Wall、Obstacle）
    [SerializeField] private float obstacleCheckDistance = 0.2f; //前方检测距离

    private int isJump = 0;
    private float jumpTimeCounter = 0;



    private enum MovementState { idle,running,jumping,falling}

    [SerializeField] private AudioSource jumpSoundEffect;
    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
        if (Input.GetButtonDown("Jump") && isJump <= 0)
        {
            if(jumpSoundEffect != null) jumpSoundEffect.Play();
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJump++;
        }

        //控制跳跃的数量
        if(isJump > 0)
        {
            jumpTimeCounter += Time.deltaTime;
            if(jumpTimeCounter > 0.1f && IsGrounded())
            {
                isJump = 0;
                jumpTimeCounter = 0;
            }
        }

        HandleJumpObstacleCollision();

        UpdateAnimationState();
    }
    private void UpdateAnimationState()
    {
        MovementState state;

        if(dirX>0f)
        {
            state = MovementState.running;
            sprite.flipX = false;
        }
        else if(dirX<0f)
        {
            state = MovementState.running;
            sprite.flipX = true;
        }
        else
        {
            state = MovementState.idle;
        }
        if(rb.velocity.y> .1f)
        {
            state = MovementState.jumping;
        }
        else if(rb.velocity.y<-.1f)
        {
            state = MovementState.falling;
        }
            anim.SetInteger("state", (int)state);
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down,.1f, jumpableGround);
    }

    /// <summary>
    /// 检测前方是否有障碍物
    /// </summary>
    /// <returns></returns>
    private bool IsObstacleInFront()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            coll.bounds.center,
            coll.bounds.size,
            0f,
            new Vector2(dirX, 0), //检测方向=输入方向
            obstacleCheckDistance,
            obstacleLayer
        );

        if(hit.collider != null)
        {
            Debug.Log("Obstacle detected: " + hit.collider.name);
        }

        return hit.collider != null;
    }

    private bool IsJumping()
    {
        return isJump > 0;
    }

    /// <summary>
    /// 处理跳跃中撞障碍物的速度重置
    /// </summary>
    private void HandleJumpObstacleCollision()
    {
        //仅当 跳跃中 + 有向前输入 + 前方有障碍物 时，清空水平速度
        if (IsObstacleInFront() && IsJumping() && dirX != 0)
        {
            rb.velocity = new Vector2(0, rb.velocity.y); // 只清空水平速度，保留竖直跳跃速度
        }
    }
}
