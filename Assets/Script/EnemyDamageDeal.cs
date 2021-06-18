using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageDeal : MonoBehaviour
{
    public int damage = 0;

    [SerializeField]
    private PlayerMovement m_playerMoment;

    private void OnTriggerEnter2D(Collider2D Target)
    {
        PlayerInfo player = Target.GetComponent<PlayerInfo>();
        m_playerMoment = Target.GetComponent<PlayerMovement>();
        if (player != null)
        {
            if(!m_playerMoment.m_isBlocking)
            {
                player.TakeDamage(damage);
            }
            else if (m_playerMoment.m_isBlocking)
            {

            }
        }
        Destroy(gameObject);
    }
}
