using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    #region === SerializeField ------------------------------------------
    [SerializeField]
    private LayerMask platformLayerMask;

    [SerializeField]
    private float movementSpd = 3f;

    [SerializeField]
    private float jumpForce = 25f;

    [SerializeField]
    float m_rollForce = 6.0f;

    [SerializeField]
    float rollingRate = 2f;
    
    [SerializeField]
    float blockingRate = 2f;

    [SerializeField]
    float AtkingRate = 0.35f;

    [SerializeField]
    GameObject m_Atk1;

    [SerializeField]
    GameObject m_Atk2;

    [SerializeField]
    GameObject m_Atk3;
    #endregion

    #region === Private ------------------------------------------
    float nextRollingTime = 0f;
    float nextBlockingTime = 0f;
    float AtkingTime = 0f;

    TerrianDetection m_terrianDetection;
    CeilingCheck m_ceilingCheck;

    private int m_facingDirection = 1;
    private Animator m_animator;
    private Rigidbody2D m_rgbd2d;
    private BoxCollider2D m_boxCollider2D;
    private CircleCollider2D m_circleCollider2D;
    private SpriteRenderer m_spriteRenderer;
    private bool m_isAtking = false;
    private bool m_rolling = false;
    private float m_delayToIdle = 0.0f;
    private string m_buttonPress;
    private bool m_isBlocking = false;
    #endregion

    #region === Public ------------------------------------------
    public Transform attackPoint;
    public TextMeshProUGUI Timer;
    public Text RollCooldownUI;
    public Image RollCooldownUIIcon;
    #endregion

    #region === Const String --------------------------------------
    const string right = "Right";
    const string left = "Left";
    const string jump = "Jump";
    const string atk = "Atk";
    const string roll = "Rolling";
    const string rolling = "Keep Rolling";
    #endregion
    
    int atkStyleChange = 1;

    #region XXXX Singleton Pattern (Not using but keep here to remind what singleton is)

    /*//--------------------------------- Singleton Pattern (Not using but keep here to remind what singleton is) ---------------------------
    public static PlayerMovement Instance { get; private set; }
    private void Awake()
    {
        if(Instance != null)
        {
            //already have unique instance in this class? destroy this one
            Destroy(gameObject);
        }
        else
        {
            //no unique instance? create new one and prevent it from destroy on load in new scene
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }*/

    #endregion
    

    // Start is called before the first frame update
    void Start()
    {
        m_rgbd2d = GetComponent<Rigidbody2D>();
        m_boxCollider2D = GetComponent<BoxCollider2D>();
        m_animator = GetComponent<Animator>();
        m_terrianDetection = GetComponent<TerrianDetection>();
        m_ceilingCheck = GetComponent<CeilingCheck>();
    }



    // Update is called once per frame
    void Update()
    {
        //--- Checking UI Rolling CD ---
        m_terrianDetection.IsGrounded();
        m_ceilingCheck.IsCeilingCheck();
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
        if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && !m_isBlocking && !m_rolling && m_boxCollider2D.enabled)
            m_buttonPress = right;
        else if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && !m_isBlocking && !m_rolling && m_boxCollider2D.enabled)
            m_buttonPress = left;
        else
            m_buttonPress = null;

        //--- jumping assign key --
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space)) && !m_rolling && m_terrianDetection.IsGrounded())
        {
            m_isBlocking = false;
            m_boxCollider2D.enabled = true;
            m_buttonPress = jump;
        }

        //--- Atking assign key --
        if ((Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.Z)) && Time.time >= AtkingTime && !m_rolling)
        {
            m_isBlocking = false;
            m_boxCollider2D.enabled = true;
            m_buttonPress = atk;
            m_isAtking = true;
        }
        else if (Input.GetKey(KeyCode.L) && !m_rolling && Time.time >= nextRollingTime)
        {
            m_isBlocking = false;
            m_buttonPress = roll;
        }
        // Block
        else if (Input.GetKeyDown(KeyCode.K) && !m_rolling && m_boxCollider2D.enabled && Time.time >= nextBlockingTime)
        {
            m_isBlocking = true;
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
            nextBlockingTime = Time.time + blockingRate;
        }
        else if (Input.GetKeyUp(KeyCode.K))
        {
            m_isBlocking = false;
            m_animator.SetBool("IdleBlock", false);
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
        //*** Check invincible rolling ***
        if (!m_boxCollider2D.enabled && m_rolling)
        {
            Physics2D.IgnoreLayerCollision(3, 8, true);
        }
        if (m_boxCollider2D.enabled && !m_rolling)
        {
            Physics2D.IgnoreLayerCollision(3, 8, false);
        }
        //*** keep Rolling ***
        if (m_ceilingCheck.IsCeilingCheck())
        {
            m_buttonPress = rolling;
        }
        //*** Moving RIGHT ***
        if (m_buttonPress == right)
        {
            //GetComponent<SpriteRenderer>().flipX = false;
            //flip with scaleX -1
            transform.eulerAngles = new Vector3(0, 0, 0);
            m_facingDirection = 1;
            if(m_isAtking && m_terrianDetection.IsGrounded() || m_isBlocking && m_terrianDetection.IsGrounded())
            {
                m_rgbd2d.velocity = new Vector2(0, m_rgbd2d.velocity.y);
            }
            else
            {
                m_rgbd2d.velocity = new Vector2(m_facingDirection * movementSpd, m_rgbd2d.velocity.y);
            }
            if (m_terrianDetection.IsGrounded() && !m_rolling)
            {
                m_delayToIdle = 0.05f;
                m_animator.SetInteger("AnimState", 1);
            }
        }

        //*** Moving LEFT ***
        else if (m_buttonPress == left)
        {
            //GetComponent<SpriteRenderer>().flipX = true; this only flipx the sprite (colider might not flip with it)
            //flip with scaleX -1
            transform.eulerAngles = new Vector3(0, -180, 0);
            m_facingDirection = -1;
            if (m_isAtking && m_terrianDetection.IsGrounded() || m_isBlocking && m_terrianDetection.IsGrounded())
            {
                m_rgbd2d.velocity = new Vector2(0, m_rgbd2d.velocity.y);
            }
            else
            {
                m_rgbd2d.velocity = new Vector2(m_facingDirection * movementSpd, m_rgbd2d.velocity.y);
            }
            if (m_terrianDetection.IsGrounded() && !m_rolling)
            {
                m_delayToIdle = 0.05f;
                m_animator.SetInteger("AnimState", 1);
            }
        }
        //*** Rolling ***
        else if (m_buttonPress == roll)
        {
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            m_rgbd2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_rgbd2d.velocity.y);
            nextRollingTime = Time.time + rollingRate;
            m_boxCollider2D.enabled = false;
        }
        else if (m_buttonPress == rolling)
        {
            m_rolling = true;
            m_animator.SetTrigger("Rolling");
            m_rgbd2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_rgbd2d.velocity.y);
            nextRollingTime = Time.time + rollingRate;
            m_boxCollider2D.enabled = false;
        }

        //*** JUMPING ***
        if (m_buttonPress == jump)
        {
            m_animator.SetTrigger("Jump");
            m_animator.SetBool("Grounded", m_terrianDetection.IsGrounded());
            m_rgbd2d.velocity = Vector2.up * jumpForce;
        }

        //*** ATK ***
        if (m_buttonPress == atk)
        {
            m_isAtking = true;
            if (atkStyleChange > 3)
            {
                atkStyleChange = 1;
            }
            if (m_terrianDetection.IsGrounded() && !m_rolling && m_isAtking)
            {
                m_rgbd2d.velocity = new Vector2(0, m_rgbd2d.velocity.y);
                //animator.Play("PlayerAtk");
                m_animator.SetTrigger("Attack" + atkStyleChange);
                atkStyleChange++;
                AtkingTime = Time.time + AtkingRate;
            }
            else if (!m_terrianDetection.IsGrounded() && !m_rolling)
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
            if (Time.time >= AtkingTime)
            {
                //m_rgbd2d.velocity = new Vector2(0, m_rgbd2d.velocity.y);
            }
        }
    }
    //---------------- Public Variables Class ---------------------

    public bool IsBlocking()
    {
        return m_isBlocking;
    }
  

    //---------------- Animation Events ---------------------
    void AE_ResetRoll()
    {
        m_rolling = false;
        m_boxCollider2D.enabled = true;
        m_rgbd2d.velocity = new Vector2(0, m_rgbd2d.velocity.y);
        if (!m_ceilingCheck.IsCeilingCheck())
            m_animator.Play("Idle");
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
        m_isAtking = false;
        GameObject[] DestroyAtk = GameObject.FindGameObjectsWithTag("Slashing");
        foreach (GameObject Atk in DestroyAtk)
        {
            Destroy(Atk);
        }
    }
}