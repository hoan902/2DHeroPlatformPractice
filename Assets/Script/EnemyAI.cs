using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        Roaming,
        ArgoPlayer,
        AttackingPlayer,
    }

    //HoanDN 31/12/2024 -> Make bool for dead status for both enemies and player => to check whether should able to
    //--Restart lv/Revive/Enemies deny runing script (avoid null)/ second chance triggered?/ enemies have revive?/ Trigger explosion?/ etc
    [Header("____ Transform ____")]
    [SerializeField] Transform m_target;
    [SerializeField] Transform m_attackPoint;
    [FormerlySerializedAs("m_argoTransform")]
    [Header("____ Detecting Collider ____")]
    [SerializeField] Collider2D m_argoCollider;
    [SerializeField] Collider2D m_detectVisionCollider;
    [SerializeField] Collider2D m_attackingRangeCollider;

    [Header("____ Speed Config ____")]
    [SerializeField] float m_speed = 4;
    [SerializeField] float m_roamingSpeed = 2;

    [Header("____ Atking Config ___")]
    [SerializeField] float m_atkingRate = 1.35f;
    [SerializeField] GameObject m_atk1;
    [SerializeField] GameObject m_atk2;
    
    [Header("____ Distance detection config ____")]
    [SerializeField] float m_downDistance;
    [SerializeField] float m_xDistance;
    [SerializeField] float m_viewDistance;
    [SerializeField] float m_nextWayPointDistance = 3f;
    [SerializeField] float m_radiusDistance;

    // --- Private Variables zone --- 
    private int m_atkStyleChange = 1;
    private Animator m_animator;
    private TerrianDetection m_terrianDetection;
    private PlayerInfo m_playerInfo;
    private int m_currentWayPoint = 0;
    private bool m_isAtking = false;
    private float m_atkingTime = 0f;
    private int m_direction;
    private Vector2 m_baseScale;

    [Header("____ Current State ____")]
    [SerializeField] private State m_state;

    [Header("____ LayerMask Setting ____")]
    [SerializeField] private LayerMask m_platformLayerMask;
    [SerializeField] private LayerMask m_playerLayerMask;
    
    Rigidbody2D m_rb2D;
    
    //---- Animator Static Parameter Names -----
    private static readonly int EnemyWalking = Animator.StringToHash("EnemyWalking");
    private static readonly int Grounded = Animator.StringToHash("Grounded");


    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_rb2D = GetComponent<Rigidbody2D>();
        m_terrianDetection = GetComponent<TerrianDetection>();
        m_state = State.Roaming;
        m_baseScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        m_direction = transform.localScale.x > 0 ? 1 : -1;
    }

    // Start is called before the first frame update
    void Start()
    {
        Physics2D.IgnoreLayerCollision(8, 8, true); //Ignore self collision with others enemies
    }

    //Update is called once per fixed second (can change in unity project setting)
    void FixedUpdate()
    {
        switch (m_state)
        {
            // ------------- Roaming -----------------
            case State.Roaming:
                m_isAtking = false;
                m_detectVisionCollider.enabled = true;
                m_argoCollider.enabled = false;
                m_attackingRangeCollider.enabled = false;
                //What enemy do when roaming
                m_rb2D.velocity = (m_direction > 0 ? Vector2.right : Vector2.left) * m_roamingSpeed;
                if (m_terrianDetection.IsGrounded())
                {
                    m_animator.SetBool(Grounded, true);
                    m_animator.SetInteger(EnemyWalking, 1);
                }
                if (m_terrianDetection.CheckWall(m_direction) || m_terrianDetection.CheckAbyss(m_direction))
                    UpdateDirection();
                break;
            //===============================================================================================
            //===============================    Argo player    ========================================
            //===============================================================================================
            case State.ArgoPlayer:
                //===============================================================================================
                if (m_isAtking || !m_playerInfo)
                    return;
                if (m_playerInfo.currentHealth <= 0)
                {
                    m_state = State.Roaming;
                    return;
                }
                UpdateDirection(m_playerInfo.transform);
                m_rb2D.velocity = new Vector2(m_direction * m_speed, m_rb2D.velocity.y);
                m_animator.SetInteger(EnemyWalking, 1);
                m_animator.SetBool(Grounded, true);
                break;
            //===============================================================================================
            //===============================================================================================


            //===============================================================================================
            //===============================    Attacking player    ========================================
            //===============================================================================================
            case State.AttackingPlayer:
                //===============================================================================================
                //===============================================================================================
                if (m_playerInfo.currentHealth <= 0)
                {
                    m_state = State.Roaming;
                    return;
                }
                if (Time.time >= m_atkingTime && m_isAtking)
                {
                    if (m_atkStyleChange > 2)
                        m_atkStyleChange = 1;
                    if (m_terrianDetection.IsGrounded() && m_isAtking)
                    {
                        m_rb2D.velocity = new Vector2(0, m_rb2D.velocity.y);
                        m_animator.SetTrigger("Atk" + m_atkStyleChange);
                        m_atkStyleChange++;
                        m_atkingTime = Time.time + m_atkingRate;
                    }
                    //This is air atk, but enemy haven't have jump skill yet so it might not needed yet
                    /*else if (!m_grounded.IsGrounded())
                    {
                        m_animator.SetTrigger("Atk1");
                        AtkingTime = Time.time + AtkingRate;
                    }*/
                }
                break;
                //===============================================================================================
                //===============================================================================================
        }
    }
    
    void UpdateDirection()
    {
        m_direction = -m_direction;
        transform.localScale = new Vector3(m_baseScale.x * m_direction, m_baseScale.y, 1);
    }
    
    void UpdateDirection(Transform target)
    {
        if (transform.position.x - target.position.x > 0)
            m_direction = -1;
        else
            m_direction = 1;
        transform.localScale = new Vector3(m_baseScale.x * m_direction, m_baseScale.y, 1);
    }

    public void DetectedPlayer(Collider2D other)
    {
        if (!other) 
            return;
        m_playerInfo = other.transform.GetComponent<PlayerInfo>();
        if (!m_playerInfo || m_playerInfo.currentHealth <= 0)
        {
            m_state = State.Roaming;
            return;
        }
        m_isAtking = false;
        m_detectVisionCollider.enabled = false;
        m_argoCollider.enabled = true;
        m_attackingRangeCollider.enabled = true;
        m_state = State.ArgoPlayer;
    }
    
    public void PlayerInAtkRange(Collider2D other)
    {
        if (!other) 
            return;
        m_playerInfo = other.transform.GetComponent<PlayerInfo>();
        if (!m_playerInfo || m_playerInfo.currentHealth <= 0)
        {
            m_state = State.Roaming;
            return;
        }
        Debug.LogError("+ Player In Atk Range");
        m_isAtking = true;
        m_state = State.AttackingPlayer;
    }

    public void PlayerOutAtkRange(Collider2D other)
    {
        if (!other) 
            return;
        m_playerInfo = other.transform.GetComponent<PlayerInfo>();
        if (!m_playerInfo || m_playerInfo.currentHealth <= 0)
        {
            m_state = State.Roaming;
            return;
        }
        Debug.LogError("- Player Out Atk Range");
        m_isAtking = false;
        m_state = State.ArgoPlayer;
    }
    public void PlayerInOfArgoRange(Collider2D other)
    {
        if (!other.transform.GetComponent<PlayerInfo>() || m_terrianDetection.CheckWall(m_direction))
        {
            m_state = State.Roaming;
            return;
        }
        Debug.LogError("+ Player In Argo Range");
        m_playerInfo = other.transform.GetComponent<PlayerInfo>();
        if (m_playerInfo.currentHealth > 0)
            m_state = State.ArgoPlayer;
        else if (m_playerInfo.currentHealth <= 0)
            m_state = State.Roaming;
    }

    public void PlayerOutOfArgoRange(Collider2D other)
    {
        if (!other.transform.GetComponent<PlayerInfo>())
            return;
        Debug.LogError("- Player In Argo Range");
        m_playerInfo = null;
        m_state = State.Roaming;
    }
    
    //===============================================================================================
    //=================================    Animation Event    =======================================
    //===============================================================================================
    void AE_AtkDamage1()
    {
        //GameObject Atk = Instantiate(m_atk1, m_attackPoint);
        m_atk1.SetActive(true);
    }
    void AE_AtkDamage2()
    {
        m_atk2.SetActive(true);
    }
    void AE_AtkDamageEnd()
    {
        m_atk1.SetActive(false);
        m_atk2.SetActive(false);
    }
}
