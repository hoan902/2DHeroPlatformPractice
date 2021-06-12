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
    }


    public Transform target;

    public float speed = 500;

    public float nextWayPointDistance = 3f;

    private Animator m_animator;
    private Path m_path;
    private int m_currentWayPoint = 0;
    private bool m_reachEndOfPath = false;
    private State state;

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

        InvokeRepeating("UpdatePath", 0f, .5f);
        m_seeker.StartPath(m_rb2D.position, target.position, OnPathComplete);
    }

    void UpdatePath()
    {
        if(m_seeker.IsDone())
        {
            m_seeker.StartPath(m_rb2D.position, target.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            m_path = p;
            m_currentWayPoint = 0;
        }
    }
    //Update is called once per fixed second (can change in unity project setting)
    void FixedUpdate()
    {
        switch (state)
        {
            default:
            case State.Roaming:

                break;
            case State.DectectPlayer:
                break;
        }

        //if there no path
        if (m_path == null)
            return;

        //If reached to the path
        if(m_currentWayPoint >= m_path.vectorPath.Count)
        {
            m_reachEndOfPath = true;
            return;
        //If havent reached to the path
        }else
        {
            m_reachEndOfPath = false;
        }

        //direction that need to go, normalized to 1 and -1 for choosing direction
        Vector2 direction = ((Vector2)m_path.vectorPath[m_currentWayPoint] - m_rb2D.position).normalized;

        //force that apply to enemy movements (mutiply with deltaTime to make sure it not vary depending on framerate)
        Vector2 force = direction * speed * Time.deltaTime;
        m_rb2D.AddForce(force);

        //Distance frome enemy to destination
        float distance = Vector2.Distance(m_rb2D.position, m_path.vectorPath[m_currentWayPoint]);
        
        if(distance < nextWayPointDistance)
        {
            m_currentWayPoint++;
        }

        if(m_rb2D.velocity.x >= 0.01f)
        {
            m_animator.SetInteger("EnemyAnimState", 1);
            m_animator.SetBool("Grounded",true);
            //GetComponent<SpriteRenderer>().flipX = false;
            transform.localScale = new Vector3(2.4f, 2.4f, 2.4f);
        }
        else if(m_rb2D.velocity.x <= -0.01f)
        {
            m_animator.SetBool("Grounded", true);
            m_animator.SetInteger("EnemyAnimState", 1);
            //GetComponent<SpriteRenderer>().flipX = true;
            transform.localScale = new Vector3(-2.4f, 2.4f, 2.4f);
        }
    }

    public bool RaycastVision()
    {
        return false;
    }
}
