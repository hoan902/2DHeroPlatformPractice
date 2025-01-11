using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TerrianDetection : MonoBehaviour
{
    private Collider2D m_collider2D;
    private Animator m_animator;
    private Bounds m_bounds;

    [FormerlySerializedAs("extraHeight")] 
    [SerializeField] private float m_extraHeight = 1f;
    [FormerlySerializedAs("platformLayerMask")] 
    [SerializeField] private LayerMask m_platformLayerMask;
    [SerializeField] private LayerMask m_groundLayer;
    [SerializeField] private LayerMask m_wallLayer;

    // Start is called before the first frame update
    void Awake()    
    {
        m_collider2D = GetComponent<Collider2D>();
        m_animator = GetComponent<Animator>();
        m_bounds = m_collider2D.bounds;
    }
    //============================== GROUND CHECK (PRIVATE BOOL) =========================================== 
    public bool IsGrounded()
    {
        RaycastHit2D raycasthit = Physics2D.BoxCast(m_collider2D.bounds.center, m_collider2D.bounds.size, 0f, Vector2.down, m_extraHeight, m_platformLayerMask);
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
        Debug.DrawRay(m_collider2D.bounds.center + new Vector3(m_collider2D.bounds.extents.x, 0), Vector2.down * (m_collider2D.bounds.extents.y + m_extraHeight), rayColor);
        Debug.DrawRay(m_collider2D.bounds.center - new Vector3(m_collider2D.bounds.extents.x, 0), Vector2.down * (m_collider2D.bounds.extents.y + m_extraHeight), rayColor);
        Debug.DrawRay(m_collider2D.bounds.center - new Vector3(m_collider2D.bounds.extents.x, m_collider2D.bounds.extents.y), Vector2.right * (m_collider2D.bounds.extents.x), rayColor);
        return raycasthit.collider != null;
    }
    /*void CheckGrounded()
    {
        RaycastHit2D leftCast = Physics2D.Raycast(m_collider2D.bounds.center + new Vector3(-m_collider2D.bounds.extents.x, 0), Vector2.down, m_collider2D.bounds.extents.y + 0.1f, m_groundLayer);
        RaycastHit2D rightCast = Physics2D.Raycast(m_collider2D.bounds.center + new Vector3(m_collider2D.bounds.extents.x, 0), Vector2.down, m_collider2D.bounds.extents.y + 0.1f, m_groundLayer);
#if UNITY_EDITOR
        Debug.DrawRay(m_collider2D.bounds.center + new Vector3(-m_collider2D.bounds.extents.x, 0), Vector3.down * (m_collider2D.bounds.extents.y + 0.1f), Color.yellow);
        Debug.DrawRay(m_collider2D.bounds.center + new Vector3(m_collider2D.bounds.extents.x, 0), Vector3.down * (m_collider2D.bounds.extents.y + 0.1f), Color.yellow);
#endif
        m_isGrounded = leftCast.collider != null || rightCast.collider != null;
    }*/
    public bool CheckAbyss(int direction)
    {
        bool rightCheck = direction > 0;
        RaycastHit2D raycast = Physics2D.Raycast(m_collider2D.bounds.center + new Vector3(rightCheck ? m_bounds.extents.x : -m_bounds.extents.x, 0), Vector2.down, m_bounds.extents.y + 0.2f, m_groundLayer);
#if UNITY_EDITOR
        Debug.DrawRay(m_collider2D.bounds.center + new Vector3(rightCheck ? m_bounds.extents.x : -m_bounds.extents.x, 0), Vector2.down * (m_bounds.extents.y + 0.2f), Color.black);
#endif
        return raycast.collider == null;
    }
    public bool CheckWall(int direction)
    {
        bool rightCheck = direction > 0;
        RaycastHit2D raycast = Physics2D.Raycast(m_collider2D.bounds.center, rightCheck ? Vector2.right : Vector2.left, m_bounds.extents.x + 0.2f, m_wallLayer);
#if UNITY_EDITOR
        Debug.DrawRay(m_collider2D.bounds.center, (rightCheck ? Vector2.right : Vector2.left) * (m_bounds.extents.x + 0.2f), Color.black);
#endif
        return raycast.collider != null;
    }
}
