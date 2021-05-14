using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private LayerMask platformLayerMask;

    [SerializeField]
    private float movementSpd = 3f;

    [SerializeField]
    private float jumpForce = 25f;

    [SerializeField]
    float extraHeight = 1f;
    [SerializeField]
    float m_rollForce = 6.0f;
    [SerializeField]
    float rollingRate = 2f;
    float nextRollingTime = 0f;

    private Animator animator;
    private Rigidbody2D rgbd2d;
    private BoxCollider2D boxCollider2D;
    private CircleCollider2D circleCollider2D;
    private SpriteRenderer spriteRenderer;
    private bool isAtking = false;
    private bool m_rolling = false;
    private string buttonPress;

    public TextMeshProUGUI Timer;
    public Text RollCooldownUI;
    public Image RollCooldownUIIcon;
    public const string Right = "Right";
    public const string Left = "Left";
    public const string Jump = "Jump";
    public const string Atk = "Atk";
    public const string RollRight = "RollingRight";
    public const string RollLeft = "RollingLeft";
    public const string Crouch = "Crouch";

    // Start is called before the first frame update
    void Start()
    {
        rgbd2d = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextRollingTime - 0.5f && !m_rolling)
        {
            boxCollider2D.enabled = true;
        }
        string TimeCount = Math.Round(Time.time, 2).ToString();
        Timer.text = "Timer: " + TimeCount;
        //------ Assign key  ------
        if (Input.GetKey(KeyCode.S) && IsGrounded() && !m_rolling)
        {
                buttonPress = Crouch;
        }



        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            m_rolling = false;
            buttonPress = Right;
            //-- CD for next roll --
            if (Input.GetKey(KeyCode.S) && IsGrounded() && !m_rolling)
            {
                if  (Time.time >= nextRollingTime)
                {
                    m_rolling = true;
                    buttonPress = RollRight;
                }
            }
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            m_rolling = false;
            buttonPress = Left;
            if  (Input.GetKey(KeyCode.S) && IsGrounded() && !m_rolling)
            {
                if (Time.time >= nextRollingTime)
                {
                    m_rolling = true;
                    buttonPress = RollLeft;
                }
            }
        }
        else
        {
            buttonPress = null;
        }



        //--- jumping assign key --
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space))
        {
            if (IsGrounded())
                buttonPress = Jump;
        }



        //--- Atking assign key --
        if (Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.Z))
        {
            buttonPress = Atk;
            isAtking = true;
        }
    }
    void FixedUpdate()
    {
        //--- Checking UI Rolling CD ---
        float remainingCD = nextRollingTime - Time.time;
        if (remainingCD > 0)
        {
            RollCooldownUI.text = "Rolling CD: " + Math.Round(remainingCD,1).ToString();
            RollCooldownUIIcon.fillAmount = remainingCD;
        }
        else
        {
            RollCooldownUIIcon.fillAmount = 0;
            RollCooldownUI.text = "Rolling CD: Ready";
        }
        



        //------ ButtonPress trigger animation and functions ------
        //=== 1. Always update Player characters Y velocity for ground check ===
        animator.SetFloat("AirSpeedY", rgbd2d.velocity.y);



        //=== 2. Movements on ground ===
        //*** Moving RIGHT ***
        if (buttonPress == Crouch)
        {
            //make it like run but slow speed and disable box
            if (IsGrounded() && !m_rolling)
            {
                //animator.SetTrigger("Crouch");
            }
            //transform.localScale = new Vector3(0.3f, 0.3f, 1.6f);
        }



        //*** Moving RIGHT ***
        if (buttonPress == Right)
        {
            rgbd2d.velocity = new Vector2(movementSpd, rgbd2d.velocity.y);
            if (IsGrounded() && !m_rolling)
            {
                animator.SetInteger("AnimState", 1);
            }
            spriteRenderer.flipX = false;
            //transform.localScale = new Vector3(0.3f, 0.3f, 1.6f);
        }



        //*** ROLL to the RIGHT ***
        else if (buttonPress == RollRight)
        {
            Vector2 rollRight = new Vector2(rgbd2d.position.x + m_rollForce * Time.deltaTime, rgbd2d.position.y);
            rgbd2d.MovePosition(rollRight);
            //rgbd2d.velocity = new Vector2(movementSpd * m_rollForce * Time.deltaTime, rgbd2d.velocity.y);
            isAtking = false;
            if (IsGrounded() && m_rolling)
            {
                animator.Play("Roll");
                nextRollingTime = Time.time + rollingRate;
            }
            boxCollider2D.enabled = false;
            spriteRenderer.flipX = false;
            //transform.localScale = new Vector3(0.3f, 0.3f, 1.6f);

        }



        //*** Moving LEFT ***
        else if (buttonPress == Left)
        {
            rgbd2d.velocity = new Vector2(-movementSpd, rgbd2d.velocity.y);
            if (IsGrounded() && !m_rolling)
            {
                animator.SetInteger("AnimState", 1);
            }
            spriteRenderer.flipX = true;
            //transform.localScale = new Vector3(-0.3f, 0.3f, 1.6f);
        }



        //*** ROLL to the LEFT ***
        else if (buttonPress == RollLeft)
        {
            Vector2 rollLeft = new Vector2(rgbd2d.position.x - m_rollForce * Time.deltaTime, rgbd2d.position.y);
            rgbd2d.MovePosition(rollLeft);
            //rgbd2d.velocity = new Vector2(-movementSpd * m_rollForce * Time.deltaTime, rgbd2d.velocity.y);
            isAtking = false;
            if (IsGrounded() && m_rolling)
            {
                animator.Play("Roll");
                nextRollingTime = Time.time + rollingRate;
            }
            boxCollider2D.enabled = false;
            spriteRenderer.flipX = true;
            //transform.localScale = new Vector3(-0.3f, 0.3f, 1.6f);
        }



        //*** JUMPING ***
        else if (buttonPress == Jump)
        {
            if (m_rolling == true)
            {
                m_rolling = false;
            }
            isAtking = false;
            rgbd2d.velocity = Vector2.up * jumpForce;
            animator.SetTrigger("Jump");
        }




        //*** ATK ***
        else if (buttonPress == Atk)
        {
            if (m_rolling == true)
            {
                m_rolling = false;
            }
            if (IsGrounded() && !m_rolling)
            {
                //animator.Play("PlayerAtk");
                animator.SetTrigger("Attack1");
            }
            else if (!IsGrounded() && !m_rolling)
            {
                animator.SetTrigger("AirAtk");
            }
            isAtking = false;
        }




        //*** NOT DOING ANYTHING ELSE ***
        else
        {
            animator.SetInteger("AnimState", 0);
            isAtking = false;
            if (m_rolling == true)
            {
                m_rolling = false;
            }
            rgbd2d.velocity = new Vector2(0, rgbd2d.velocity.y);
            if (IsGrounded() && !isAtking && !m_rolling)
            {
                animator.Play("Idle");
            }
        }
    }




    //ground check  
    private bool IsGrounded()
    {
        RaycastHit2D raycasthit = Physics2D.BoxCast(circleCollider2D.bounds.center, circleCollider2D.bounds.size, 0f, Vector2.down, extraHeight, platformLayerMask);
        Color rayColor;
        if (raycasthit.collider != null)
        {
            animator.SetBool("Grounded", true);
            rayColor = Color.blue;
        }
        else
        {
            animator.SetBool("Grounded", false);
            rayColor = Color.red;
        }
        Debug.DrawRay(circleCollider2D.bounds.center + new Vector3(circleCollider2D.bounds.extents.x, 0), Vector2.down * (circleCollider2D.bounds.extents.y + extraHeight), rayColor);
        Debug.DrawRay(circleCollider2D.bounds.center - new Vector3(circleCollider2D.bounds.extents.x, 0), Vector2.down * (circleCollider2D.bounds.extents.y + extraHeight), rayColor);
        Debug.DrawRay(circleCollider2D.bounds.center - new Vector3(circleCollider2D.bounds.extents.x, circleCollider2D.bounds.extents.y), Vector2.right * (circleCollider2D.bounds.extents.x), rayColor);
        Debug.Log(raycasthit.collider);
        return raycasthit.collider != null;
    }




    void AE_ResetRoll()
    {
        m_rolling = false;
    }
}
