using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageDeal : MonoBehaviour
{
    public int damage = 0;
    [SerializeField] private Collider2D m_atkCollider;
    private void OnTriggerEnter2D(Collider2D target)
    {
        PlayerInfo player = target.GetComponent<PlayerInfo>();
        PlayerMovement playerMovement = target.GetComponent<PlayerMovement>();
        if (!player)
            return;
        m_atkCollider.enabled = false;
        if (player.currentHealth <= 0)
            return;
        if (!playerMovement.IsBlocking())
            player.TakeDamage(damage);
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
