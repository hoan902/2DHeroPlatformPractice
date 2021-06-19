using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageDeal : MonoBehaviour
{
    public int damage = 0;

    [SerializeField]
    private PlayerMovement m_playerMoment;
    private Transform m_playerTranform;

    private void OnTriggerEnter2D(Collider2D Target)
    {
        PlayerInfo player = Target.GetComponent<PlayerInfo>();
        m_playerMoment = Target.GetComponent<PlayerMovement>();
        m_playerTranform = Target.GetComponent<Transform>();
        if (player != null)
        {
            if(!m_playerMoment.m_isBlocking || m_playerMoment.m_isBlocking && m_playerTranform.eulerAngles == transform.eulerAngles)
            {
                player.TakeDamage(damage);
            }
        }
        Destroy(gameObject);
    }
}
