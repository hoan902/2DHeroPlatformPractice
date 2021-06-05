using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageDeal : MonoBehaviour
{
    public int damage = 0;
    private void OnTriggerEnter2D(Collider2D Target)
    {
        PlayerInfo player = Target.GetComponent<PlayerInfo>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Debug.LogWarning(Target.name);
        }
        Destroy(gameObject);
    }
}
