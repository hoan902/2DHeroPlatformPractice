using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingCheck : MonoBehaviour
{
    private CircleCollider2D m_circleCollider2D;

    [SerializeField]
    private float extraHeightCeilCheck = 1f;

    [SerializeField]
    private LayerMask platformLayerMask;
    // Start is called before the first frame update
    void Start()
    {
        m_circleCollider2D = GetComponent<CircleCollider2D>();
    }

    //============================== Ceiling Check (PRIVATE BOOL) =========================================== 
    public bool IsCeilingCheck()
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
}
