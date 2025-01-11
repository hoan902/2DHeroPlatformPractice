using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingCheck : MonoBehaviour
{
    [SerializeField]
    private Collider2D m_collider2D;

    [SerializeField]
    private float extraHeightCeilCheck = 1f;

    [SerializeField]
    private LayerMask platformLayerMask;

    //============================== Ceiling Check (PRIVATE BOOL) =========================================== 
    public bool IsCeilingCheck()
    {
        RaycastHit2D raycasthit = Physics2D.BoxCast(m_collider2D.bounds.center, m_collider2D.bounds.extents, 0f, Vector2.up, extraHeightCeilCheck, platformLayerMask);
        Color rayColor;
        if (raycasthit.collider)
            rayColor = Color.cyan;
        else
            rayColor = Color.yellow;
        Debug.DrawRay(m_collider2D.bounds.center + new Vector3(m_collider2D.bounds.extents.x, 0), Vector2.up * (m_collider2D.bounds.extents.y + extraHeightCeilCheck), rayColor);
        Debug.DrawRay(m_collider2D.bounds.center - new Vector3(m_collider2D.bounds.extents.x, 0), Vector2.up * (m_collider2D.bounds.extents.y + extraHeightCeilCheck), rayColor);
        Debug.DrawRay(m_collider2D.bounds.center - new Vector3(m_collider2D.bounds.extents.x, m_collider2D.bounds.extents.y), Vector2.right * (m_collider2D.bounds.extents.x), rayColor);
        return raycasthit.collider;
    }
}
