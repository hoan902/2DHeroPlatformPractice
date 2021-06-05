using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageDeal : MonoBehaviour
{
    public int damage = 0;
    private void OnTriggerEnter2D(Collider2D Target)
    {
        EnemyInfo enemy = Target.GetComponent<EnemyInfo>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Debug.LogWarning(Target.name);
        }
        Destroy(gameObject);
    }
}
