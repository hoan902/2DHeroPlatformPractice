using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Animator m_animator;
    private Rigidbody2D m_rgbd2d;
    private BoxCollider2D m_boxCollider2D;
    private SpriteRenderer m_spriteRenderer;
    private CircleCollider2D m_circleCollider2D;

    private bool m_isAtking = false;
    private string m_buttonPress;
    int atkStyleChange = 1;

    public const string Atk = "Atk";

    [SerializeField]
    float AtkingRate = 0.35f;

    [SerializeField]
    float extraHeight = 1f;

    [SerializeField]
    private LayerMask platformLayerMask;

    float AtkingTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        m_circleCollider2D = GetComponent<CircleCollider2D>();
        m_rgbd2d = GetComponent<Rigidbody2D>();
        m_boxCollider2D = GetComponent<BoxCollider2D>();
        m_animator = GetComponent<Animator>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //--- Atking assign key --
        if ((Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.Z)) && Time.time >= AtkingTime)
        {
            m_boxCollider2D.enabled = true;
            m_buttonPress = Atk;
            m_isAtking = true;
        }
    }
    private void FixedUpdate()
    {
        //*** ATK ***
        if (m_buttonPress == Atk)
        {
            m_isAtking = true;
            if (atkStyleChange > 3)
            {
                atkStyleChange = 1;
            }
            if (IsGrounded() && m_isAtking)
            {
                //animator.Play("PlayerAtk");
                m_animator.SetTrigger("Attack" + atkStyleChange);
                atkStyleChange++;
                AtkingTime = Time.time + AtkingRate;
            }
            else if (!IsGrounded())
            {
                m_animator.SetTrigger("Attack3");
                AtkingTime = Time.time + AtkingRate;
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
}
