using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    //---------------- SerializeField zone ----------------------------
    [SerializeField]
    private LayerMask platformLayerMask;
    
    [SerializeField]
    private LayerMask EnemyLayerMask;

    [SerializeField]
    private float movementSpd = 3f;

    [SerializeField]
    private float jumpForce = 25f;

    [SerializeField]
    float extraHeight = 1f;
    [SerializeField]
    float extraHeightCeilCheck = 1f;

    [SerializeField]
    float m_rollForce = 6.0f;

    [SerializeField]
    float rollingRate = 2f;

    [SerializeField]
    float AtkingRate = 0.35f;

    [SerializeField]
    GameObject m_Atk1;

    [SerializeField]
    GameObject m_Atk2;

    [SerializeField]
    GameObject m_Atk3;

    //---------------- Private zone ----------------------------
    float nextRollingTime = 0f;
    float AtkingTime = 0f;
    private int m_facingDirection = 1;
    private Animator m_animator;
    private Rigidbody2D m_rgbd2d;
    private BoxCollider2D m_boxCollider2D;
    private CircleCollider2D m_circleCollider2D;
    private SpriteRenderer m_spriteRenderer;
    private Transform m_transform;
    private bool m_isAtking = false;
    private bool m_rolling = false;
    private bool m_isCrouch = false;
    private float m_delayToIdle = 0.0f;
    private string m_buttonPress;

    //---------------- Public Zone ----------------------------
    public Transform attackPoint;
    public TextMeshProUGUI Timer;
    public Text RollCooldownUI;
    public Image RollCooldownUIIcon;
    public const string Right = "Right";
    public const string Left = "Left";
    public const string Jump = "Jump";
    public const string Atk = "Atk";
    public const string RollRight = "RollingRight";
    public const string RollLeft = "RollingLeft";
    public const string Roll = "Rolling";
    public const string Crouch = "Crouch";
    int atkStyleChange = 1;

    // Start is called before the first frame update
    void Start()
    {
        m_rgbd2d = GetComponent<Rigidbody2D>();
        m_boxCollider2D = GetComponent<BoxCollider2D>();
        m_circleCollider2D = GetComponent<CircleCollider2D>();
        m_animator = GetComponent<Animator>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_transform = GetComponent<Transform>();
    }



    // Update is called once per frame
    void Update()
    {
        //--- Checking UI Rolling CD ---
        IsGrounded();
        CeilingCheck();
        float remainingCD = nextRollingTime - Time.time;
        if (remainingCD > 0)
        {
            RollCooldownUI.text = "Rolling CD: " + Math.Round(remainingCD, 1).ToString();
            RollCooldownUIIcon.fillAmount = remainingCD;
        }
        else
        {
            RollCooldownUIIcon.fillAmount = 0;
            RollCooldownUI.text = "Rolling CD: Ready";
        }

        //---- timer ----
        string TimeCount = Math.Round(Time.time, 2).ToString();
        Timer.text = "Timer: " + TimeCount;

        //====== Assign key ======
        if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && !m_rolling && m_boxCollider2D.enabled && Time.time >= AtkingTime)
        {
            m_buttonPress = Right;
        }
        else if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && !m_rolling && m_boxCollider2D.enabled && Time.time >= AtkingTime)
        {
            m_buttonPress = Left;
        }
        else
        {
            m_buttonPress = null;
        }

        //--- jumping assign key --
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space)) && !m_rolling && IsGrounded())
        {
            m_boxCollider2D.enabled = true;
            m_buttonPress = Jump;
            m_isCrouch = false;
        }

        //--- Atking assign key --
        if ((Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.Z)) && Time.time >= AtkingTime && !m_rolling)
        {
            m_boxCollider2D.enabled = true;
            m_buttonPress = Atk;
            m_isAtking = true;
            m_isCrouch = false;
        }
        else if (Input.GetKey(KeyCode.L) && !m_rolling && Time.time >= nextRollingTime)
        {
            m_buttonPress = Roll;
        }
    }
    void FixedUpdate()
    {
        //---------------------------------------------------------
        //------ ButtonPress trigger animation and functions ------
        //---------------------------------------------------------
        //=============================================================================================
        //=== 1. Always update Player characters Y velocity for ground check (jumping or falling) ====
        //=============================================================================================
        m_animator.SetFloat("AirSpeedY", m_rgbd2d.velocity.y);


        //===============================
        //=== 2. Movements and Atk   ====
        //===============================
        //*** Crounching ***
        if (CeilingCheck())
        {
            m_buttonPress = Roll;
        }
        //*** Moving RIGHT ***
        if (m_buttonPress == Right)
        {
            //GetComponent<SpriteRenderer>().flipX = false;
            //flip with scaleX -1
            m_transform.localScale = new Vector3(1, 1, 1);
            m_facingDirection = 1;
            m_rgbd2d.velocity = new Vector2(m_facingDirection * movementSpd, m_rgbd2d.velocity.y);
            if (IsGrounded() && !m_rolling)
            {
                m_delayToIdle = 0.05f;
                m_animator.SetInteger("AnimState", 1);
            }
        }

        //*** Moving LEFT ***
        else if (m_buttonPress == Left)
        {
            //GetComponent<SpriteRenderer>().flipX = true;
            //flip with scaleX -1
            m_transform.localScale = new Vector3(-1, 1, 1);
            m_facingDirection = -1;
            m_rgbd2d.velocity = new Vector2(m_facingDirection * movementSpd, m_rgbd2d.velocity.y);
            if (IsGrounded() && !m_rolling)
            {
                m_delayToIdle = 0.05f;
                m_animator.SetInteger("AnimState", 1);
            }
        }
        //*** Rolling ***
        else if (m_buttonPress == Roll)
        {
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            m_rgbd2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_rgbd2d.velocity.y);
            nextRollingTime = Time.time + rollingRate;
            m_boxCollider2D.enabled = false;
        }

        //*** JUMPING ***
        if (m_buttonPress == Jump)
        {
            m_animator.SetTrigger("Jump");
            m_animator.SetBool("Grounded", IsGrounded());
            m_rgbd2d.velocity = Vector2.up * jumpForce;
        }

        //*** ATK ***
        if (m_buttonPress == Atk)
        {
            m_isAtking = true;
            if (atkStyleChange > 3)
            {
                atkStyleChange = 1;
            }
            if (IsGrounded() && !m_rolling && m_isAtking)
            {
                m_rgbd2d.velocity = new Vector2(0, m_rgbd2d.velocity.y);
                //animator.Play("PlayerAtk");
                m_animator.SetTrigger("Attack" + atkStyleChange);
                atkStyleChange++;
                AtkingTime = Time.time + AtkingRate;
            }
            else if (!IsGrounded() && !m_rolling)
            {
                m_animator.SetTrigger("Attack3");
                AtkingTime = Time.time + AtkingRate;
            }
        }

        //*** NOT DOING ANYTHING ELSE ***
        if (m_buttonPress == null)
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0)
                m_animator.SetInteger("AnimState", 0);
            if (m_rolling)
            {
                m_rolling = false;
            }
            if (m_isAtking)
            {
                m_isAtking = false;
            }
            if (m_isCrouch)
            {
                m_isCrouch = false;
            }
            if (Time.time >= AtkingTime)
            {
                //m_rgbd2d.velocity = new Vector2(0, m_rgbd2d.velocity.y);
            }
        }
    }

    //============================== GROUND CHECK (PRIVATE BOOL) =========================================== 
    private bool IsGrounded()
    {
        RaycastHit2D raycasthit = Physics2D.BoxCast(m_circleCollider2D.bounds.center, m_circleCollider2D.bounds.size, 0f, Vector2.down, extraHeight, platformLayerMask);
        Color rayColor;
        if (raycasthit.collider != null)
        {
            m_animator.SetBool("Grounded", true);
            rayColor = Color.blue;
        }
        else
        {
            m_animator.SetBool("Grounded", false);
            rayColor = Color.red;
        }
        Debug.DrawRay(m_circleCollider2D.bounds.center + new Vector3(m_circleCollider2D.bounds.extents.x, 0), Vector2.down * (m_circleCollider2D.bounds.extents.y + extraHeight), rayColor);
        Debug.DrawRay(m_circleCollider2D.bounds.center - new Vector3(m_circleCollider2D.bounds.extents.x, 0), Vector2.down * (m_circleCollider2D.bounds.extents.y + extraHeight), rayColor);
        Debug.DrawRay(m_circleCollider2D.bounds.center - new Vector3(m_circleCollider2D.bounds.extents.x, m_circleCollider2D.bounds.extents.y), Vector2.right * (m_circleCollider2D.bounds.extents.x), rayColor);
        Debug.Log(raycasthit.collider);
        return raycasthit.collider != null;
    }

    //============================== Ceiling Check (PRIVATE BOOL) =========================================== 
    private bool CeilingCheck()
    {
        RaycastHit2D raycasthit = Physics2D.BoxCast(m_circleCollider2D.bounds.center, m_circleCollider2D.bounds.extents, 0f, Vector2.up, extraHeightCeilCheck, platformLayerMask);
        Color rayColor;
        if (raycasthit.collider != null)
        {
            rayColor = Color.cyan;
        }
        else
        {
            rayColor = Color.yellow;
        }
        Debug.DrawRay(m_circleCollider2D.bounds.center + new Vector3(m_circleCollider2D.bounds.extents.x, 0), Vector2.up * (m_circleCollider2D.bounds.extents.y + extraHeightCeilCheck), rayColor);
        Debug.DrawRay(m_circleCollider2D.bounds.center - new Vector3(m_circleCollider2D.bounds.extents.x, 0), Vector2.up * (m_circleCollider2D.bounds.extents.y + extraHeightCeilCheck), rayColor);
        Debug.DrawRay(m_circleCollider2D.bounds.center - new Vector3(m_circleCollider2D.bounds.extents.x, m_circleCollider2D.bounds.extents.y), Vector2.right * (m_circleCollider2D.bounds.extents.x), rayColor);
        return raycasthit.collider != null;
    }

    //---------------- Animation Events ---------------------
    void AE_ResetRoll()
    {
        m_rolling = false;
        m_boxCollider2D.enabled = true;
        m_rgbd2d.velocity = new Vector2(0, m_rgbd2d.velocity.y);
    }

    void AE_IdleVelocity()
    {
        m_rgbd2d.velocity = new Vector2(0, m_rgbd2d.velocity.y);
    }
    void AE_AtkDamage1()
    {
        GameObject Atk = Instantiate(m_Atk1, attackPoint);
    }
    void AE_AtkDamage2()
    {
        GameObject Atk = Instantiate(m_Atk2, attackPoint);   
    }
    void AE_AtkDamage3()
    {
        GameObject Atk = Instantiate(m_Atk3, attackPoint);
    }
    void AE_AtkDamageEnd()
    {
        GameObject[] DestroyAtk = GameObject.FindGameObjectsWithTag("Slashing");
        foreach (GameObject Atk in DestroyAtk)
        {
            Destroy(Atk);
        }
    }
    void flip()
    {
            m_transform.localScale = new Vector3(-1,1,1);
    }
}