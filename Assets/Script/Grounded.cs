using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grounded : MonoBehaviour
{
    private CircleCollider2D m_circleCollider2D;
    private Animator m_animator;

    [SerializeField]
    private float extraHeight = 1f;

    [SerializeField]
    private LayerMask platformLayerMask;

    // Start is called before the first frame update
    void Start()    
    {
        m_circleCollider2D = GetComponent<CircleCollider2D>();
        m_animator = GetComponent<Animator>();
    }
    //============================== GROUND CHECK (PRIVATE BOOL) =========================================== 
    public bool IsGrounded()
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
        return raycasthit.collider != null;
    }
}
