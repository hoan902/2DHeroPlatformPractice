using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageDeal : MonoBehaviour
{
    public int damage = 0;

    private void OnTriggerEnter2D(Collider2D target)
    {
        PlayerInfo player = target.GetComponent<PlayerInfo>();
        PlayerMovement playerMovement = target.GetComponent<PlayerMovement>();
        if (!player)
            return;
        if (player.currentHealth <= 0)
            return;
        if (!playerMovement.IsBlocking())
            player.TakeDamage(damage);
    }
}
