using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageDeal : MonoBehaviour
{
    public int damage = 0;
    [SerializeField] private Collider2D m_atkCollider;
    private void OnTriggerEnter2D(Collider2D Target)
    {
        EnemyInfo enemy = Target.GetComponent<EnemyInfo>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
        m_atkCollider.enabled = false;
    }
    
    public void DisableAttack()
    {
        m_atkCollider.enabled = false;
    }

    public void EnableAttack()
    {
        m_atkCollider.enabled = true;
    }
}
