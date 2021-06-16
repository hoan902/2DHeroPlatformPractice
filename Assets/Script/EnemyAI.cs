using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        Roaming,
        DectectPlayer,
        AttackingPlayer,
    }

    [Header("____ Public Transform ____")]
    public Transform target;
    public Transform groundDetection;
    public Transform wallDetection;
    public Transform playerDetectionStartPos;
    public Transform playerDetectionEndPos;
    public Transform AtkRangeStartPos;
    public Transform AtkRangeEndPos;
    public Transform attackPoint;

    [Header("____ Speed Config ____")]
    public float speed = 500;
    public float roamingSpeed = 2;

    [Header("____ Atking Config ___")]
    [SerializeField]
    float AtkingRate = 0.35f;

    [SerializeField]
    GameObject m_Atk1;

    [SerializeField]
    GameObject m_Atk2;

    [Header("____ Distance detection config ____")]
    public float downDistance;
    public float xDistance;
    public float viewDistance;
    public float nextWayPointDistance = 3f;

    public float radiusDistance;
    [SerializeField]
    private float m_radius;

    // --- Private Variable zone --- 
    private int atkStyleChange = 1;
    private Animator m_animator;
    private Grounded m_grounded;
    private Path m_path;
    private int m_currentWayPoint = 0;
    private bool m_reachEndOfPath = false;
    private bool m_movingRight = true;
    private bool m_isAtking = false;
    private float AtkingTime = 0f;

    [Header("____ Current State ____")]
    [SerializeField]
    private State state;

    [Header("____ LayerMask Setting ____")]
    [SerializeField]
    private LayerMask platformLayerMask;
    [SerializeField]
    private LayerMask playerLayerMask;

    
    Seeker m_seeker;
    Rigidbody2D m_rb2D;

    

    private void Awake()
    {
        state = State.Roaming;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_seeker = GetComponent<Seeker>();    
        m_rb2D = GetComponent<Rigidbody2D>();
        m_grounded = GetComponent<Grounded>();

        InvokeRepeating("UpdatePath", 0f, .5f);
        m_seeker.StartPath(m_rb2D.position, target.position, OnPathComplete);
    }

    //Update is called once per fixed second (can change in unity project setting)
    void FixedUpdate()
    {

        switch (state)
        {
            default:
                // ------------- Roaming -----------------
            case State.Roaming:
                m_isAtking = false;
                //What enemy do when roaming
                Debug.LogWarning("Roming now");
                transform.Translate(Vector2.right * roamingSpeed * Time.deltaTime);
                RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, downDistance,platformLayerMask);
                RaycastHit2D wallInfo = Physics2D.Raycast(wallDetection.position, Vector2.down, xDistance, platformLayerMask);
                RaycastHit2D playerInfo = Physics2D.Linecast(playerDetectionStartPos.position, playerDetectionEndPos.position, playerLayerMask);
                Debug.DrawLine(playerDetectionStartPos.position, playerDetectionEndPos.position, Color.red);
                if(groundInfo.collider)
                {
                    m_animator.SetBool("Grounded", true);
                    m_animator.SetInteger("EnemyAnimState", 1);
                }
                if(!groundInfo.collider || wallInfo.collider)
                {
                    if (m_movingRight)
                    {
                        transform.eulerAngles = new Vector3(0, -180, 0);
                        m_movingRight = false;
                    }else
                    {   
                        transform.eulerAngles = new Vector3(0,0,0);
                        m_movingRight = true;
                    }
                }
                if(playerInfo.collider)
                {
                    state = State.DectectPlayer;
                }
                break;
            //===============================================================================================
            //===============================    Detecting player    ========================================
            //===============================================================================================
            case State.DectectPlayer:
                //===============================================================================================
                //===============================================================================================
                //What enemy do when detected a player
                m_isAtking = false;
                Debug.LogWarning("Chasing player now");


                if (m_currentWayPoint >= m_path.vectorPath.Count)
                {
                    //If reached to the path
                    m_reachEndOfPath = true;
                    return;
                }
                else
                {
                    //If havent reached to the path
                    m_reachEndOfPath = false;
                }


                //if there no path
                if (m_path == null)
                    return;

                //If player in enemy atk range
                PlayerInAtkRange();

                //if player out of chasing range
                PlayerOutOfRange();

                //direction that need to go, normalized to 1 and -1 for choosing direction
                Vector2 direction = ((Vector2)m_path.vectorPath[m_currentWayPoint] - m_rb2D.position).normalized;

                //force that apply to enemy movements (mutiply with deltaTime to make sure it not vary depending on framerate)
                Vector2 force = direction * speed * Time.deltaTime;


                //using velocity cause enemy able to fly which is not what i want
                //Using addForce need to multiply with the mass since i set enemy very high so player cannot push enemy
                m_rb2D.AddForce(force * m_rb2D.mass);

                //Distance from enemy to destination (in this case is player)
                float distance = Vector2.Distance(m_rb2D.position, m_path.vectorPath[m_currentWayPoint]);
                if (distance < nextWayPointDistance)
                {
                    m_currentWayPoint++;
                }

                if (m_rb2D.velocity.x >= 0.01f)
                {
                    m_animator.SetInteger("EnemyAnimState", 1);
                    m_animator.SetBool("Grounded", true);
                    //GetComponent<SpriteRenderer>().flipX = false;
                    transform.eulerAngles = new Vector3(0, 0, 0);
                }
                else if (m_rb2D.velocity.x <= -0.01f)
                {
                    m_animator.SetBool("Grounded", true);
                    m_animator.SetInteger("EnemyAnimState", 1);
                    //GetComponent<SpriteRenderer>().flipX = true;
                    transform.eulerAngles = new Vector3(0, -180, 0);
                }
                break;
            //===============================================================================================
            //===============================================================================================


            //===============================================================================================
            //===============================    Attacking player    ========================================
            //===============================================================================================
            case State.AttackingPlayer:
                //===============================================================================================
                //===============================================================================================
                PlayerInAtkRange();
                PlayerOutOfRange();
                if (Time.time >= AtkingTime && m_isAtking)
                {
                    if (atkStyleChange > 2)
                    {
                        atkStyleChange = 1;
                    }
                    if (m_grounded.IsGrounded() && m_isAtking)
                    {
                        m_rb2D.velocity = new Vector2(0, m_rb2D.velocity.y);
                        m_animator.SetTrigger("Atk" + atkStyleChange);
                        atkStyleChange++;
                        AtkingTime = Time.time + AtkingRate;
                    }
                    else if (!m_grounded.IsGrounded())
                    {
                        m_animator.SetTrigger("Atk1");
                        AtkingTime = Time.time + AtkingRate;
                    }
                }
                break;
                //===============================================================================================
                //===============================================================================================
        }
    }


    void UpdatePath()
    {
        if (m_seeker.IsDone())
        {
            m_seeker.StartPath(m_rb2D.position, target.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            m_path = p;
            m_currentWayPoint = 0;
        }
    }


    void PlayerInAtkRange()
    {
        RaycastHit2D playerInAtkRange = Physics2D.Linecast(AtkRangeStartPos.position, AtkRangeEndPos.position, playerLayerMask);
        if (playerInAtkRange.collider)
        {
            m_isAtking = true;
            state = State.AttackingPlayer;
        }
        else
        {
            m_isAtking = false;
            state = State.DectectPlayer;
        }
    }

    void PlayerOutOfRange()
    {
        Collider2D playerInChasingRange = Physics2D.OverlapCircle(transform.position, m_radius, playerLayerMask);
        if (!playerInChasingRange)
        {
            state = State.Roaming;
        }
    }




    //===============================================================================================
    //=================================    Animation Event    =======================================
    //===============================================================================================
    void AE_AtkDamage1()
    {
        GameObject Atk = Instantiate(m_Atk1, attackPoint);
    }
    void AE_AtkDamage2()
    {
        GameObject Atk = Instantiate(m_Atk2, attackPoint);
    }
    void AE_AtkDamageEnd()
    {
        GameObject[] DestroyAtk = GameObject.FindGameObjectsWithTag("Slashing");
        foreach (GameObject Atk in DestroyAtk)
        {
            Destroy(Atk);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, m_radius);
    }
}
