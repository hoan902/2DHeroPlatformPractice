using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

enum KeyPressType
{
    None,
    Right,
    Left,
    Jump,
    Atk,
    StartRoll,
    Rolling
}

public class PlayerMovement : MonoBehaviour
{

    #region === SerializeField ------------------------------------------
    [FormerlySerializedAs("platformLayerMask")] [SerializeField]
    private LayerMask m_platformLayerMask;

    [FormerlySerializedAs("movementSpd")] [SerializeField]
    private float m_movementSpd = 3f;

    [FormerlySerializedAs("jumpForce")] [SerializeField]
    private float m_jumpForce = 25f;

    [SerializeField]
    float m_rollForce = 6.0f;

    [FormerlySerializedAs("rollingRate")] [SerializeField]
    float m_rollingRate = 2f;
    
    [FormerlySerializedAs("blockingRate")] [SerializeField]
    float m_blockingRate = 2f;

    [FormerlySerializedAs("AtkingRate")] [SerializeField]
    float m_atkingRate = 0.35f;

    [SerializeField] PlayerDamageDeal m_atk1;

    [SerializeField] PlayerDamageDeal m_atk2;

    [SerializeField] PlayerDamageDeal m_atk3;
    
    [Header("---- Audio Sounds")]
    [SerializeField] private AudioClip m_slashAudio;
    [SerializeField] private AudioClip m_blockAudio;
    [SerializeField] private AudioClip m_hurtAudio;
    [SerializeField] private AudioClip m_walkAudio;
    [SerializeField] private AudioClip m_jumpAudio;
    [SerializeField] private AudioClip m_rollAudio;
    [Header("----------------------------")]
    #endregion

    #region === Private ------------------------------------------
    float m_nextRollingTime = 0f;
    float m_nextBlockingTime = 0f;
    float m_atkingTime = 0f;

    TerrianDetection m_terrianDetection;
    CeilingCheck m_ceilingCheck;

    private Vector2 m_baseScale;
    private int m_direction = 1;
    private Animator m_animator;
    private Rigidbody2D m_rgbd2d;
    private BoxCollider2D m_boxCollider2D;
    private CircleCollider2D m_circleCollider2D;
    private SpriteRenderer m_spriteRenderer;
    private bool m_isAtking = false;
    private bool m_rolling = false;
    private float m_delayToIdle = 0.0f;
    private KeyPressType m_buttonPress;
    private bool m_isBlocking = false;
    #endregion

    #region === Public ------------------------------------------
    public Transform attackPoint;
    public TextMeshProUGUI Timer;
    public Text RollCooldownUI;
    public Image RollCooldownUIIcon;
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
        
        m_baseScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        m_direction = transform.localScale.x > 0 ? 1 : -1;
    }



    // Update is called once per frame
    void Update()
    {
        //--- Checking UI Rolling CD ---
        m_terrianDetection.IsGrounded();
        m_ceilingCheck.IsCeilingCheck();
        float remainingCD = m_nextRollingTime - Time.time;
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
            m_buttonPress = KeyPressType.Right;
        else if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && !m_isBlocking && !m_rolling && m_boxCollider2D.enabled)
            m_buttonPress = KeyPressType.Left;
        else
            m_buttonPress = KeyPressType.None;

        //--- jumping assign key --
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space)) && !m_rolling && m_terrianDetection.IsGrounded())
        {
            m_isBlocking = false;
            m_boxCollider2D.enabled = true;
            m_buttonPress = KeyPressType.Jump;
        }

        //--- Atking assign key --
        if ((Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.Z)) && Time.time >= m_atkingTime && !m_rolling)
        {
            m_isBlocking = false;
            m_boxCollider2D.enabled = true;
            m_buttonPress = KeyPressType.Atk;
            m_isAtking = true;
        }
        else if (Input.GetKey(KeyCode.L) && !m_rolling && Time.time >= m_nextRollingTime)
        {
            m_isBlocking = false;
            m_buttonPress = KeyPressType.StartRoll;
        }
        // Block
        else if (Input.GetKeyDown(KeyCode.K) && !m_rolling && m_boxCollider2D.enabled && Time.time >= m_nextBlockingTime)
        {
            m_isBlocking = true;
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
            m_nextBlockingTime = Time.time + m_blockingRate;
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
            Physics2D.IgnoreLayerCollision(3, 8, true);
        if (m_boxCollider2D.enabled && !m_rolling)
            Physics2D.IgnoreLayerCollision(3, 8, false);
        
        //*** keep Rolling ***
        if (m_ceilingCheck.IsCeilingCheck())
            m_buttonPress = KeyPressType.Rolling;

        switch (m_buttonPress)
        {
            case KeyPressType.None:
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
                /*if (Time.time >= m_atkingTime)
                {
                    m_rgbd2d.velocity = new Vector2(0, m_rgbd2d.velocity.y);
                }*/
                break;
            case KeyPressType.Right:
                MovingDirection(1);
                break;
            case KeyPressType.Left:
                MovingDirection(-1);
                break;
            case KeyPressType.Jump:
                SoundManager.PlaySound(m_jumpAudio, false);
                m_animator.SetTrigger("Jump");
                m_animator.SetBool("Grounded", m_terrianDetection.IsGrounded());
                m_rgbd2d.velocity = Vector2.up * m_jumpForce;
                break;
            case KeyPressType.Atk:
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
                    m_atkingTime = Time.time + m_atkingRate;
                }
                else if (!m_terrianDetection.IsGrounded() && !m_rolling)
                {
                    m_animator.SetTrigger("Attack3");
                    m_atkingTime = Time.time + m_atkingRate;
                }
                break;
            case KeyPressType.StartRoll:
                SoundManager.PlaySound(m_rollAudio,false);
                m_rolling = true;
                m_animator.SetTrigger("Roll");
                m_rgbd2d.velocity = new Vector2(m_direction * m_rollForce, m_rgbd2d.velocity.y);
                m_nextRollingTime = Time.time + m_rollingRate;
                m_boxCollider2D.enabled = false;
                break;
            case KeyPressType.Rolling:
                m_rolling = true;
                m_animator.SetTrigger("Rolling");
                m_rgbd2d.velocity = new Vector2(m_direction * m_rollForce, m_rgbd2d.velocity.y);
                m_nextRollingTime = Time.time + m_rollingRate;
                m_boxCollider2D.enabled = false;
                break;
        }
    }
    //---------------- Public Variables Classes ---------------------

    public bool IsBlocking()
    {
        if (m_isBlocking)
            SoundManager.PlaySound(m_blockAudio, false);
        else
            SoundManager.PlaySound(m_hurtAudio, false);
        return m_isBlocking;
    }
  
    //-------------------- Private Classes ------------------
    void UpdateDirection(int dir)
    {
        m_direction = dir;
        transform.localScale = new Vector3(m_baseScale.x * m_direction, m_baseScale.y, 1);
    }

    void MovingDirection(int dir)
    {
        UpdateDirection(dir);
        if(m_isAtking && m_terrianDetection.IsGrounded() || m_isBlocking && m_terrianDetection.IsGrounded())
            m_rgbd2d.velocity = new Vector2(0, m_rgbd2d.velocity.y);
        else
            m_rgbd2d.velocity = new Vector2(m_direction * m_movementSpd, m_rgbd2d.velocity.y);
        if (m_terrianDetection.IsGrounded() && !m_rolling)
        {
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }
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
        SoundManager.PlaySound(m_slashAudio,false);
        m_atk1.EnableAttack();
    }
    void AE_AtkDamage2()
    {
        SoundManager.PlaySound(m_slashAudio,false);
        m_atk2.EnableAttack();
    }
    void AE_AtkDamage3()
    {
        SoundManager.PlaySound(m_slashAudio,false);
        m_atk3.EnableAttack();
    }
    void AE_AtkDamageEnd()
    {
        m_isAtking = false;
        m_atk1.DisableAttack();
        m_atk2.DisableAttack();
        m_atk3.DisableAttack();
    }
}