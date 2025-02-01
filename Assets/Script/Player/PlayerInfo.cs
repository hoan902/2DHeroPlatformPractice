using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public int maxHealth = 300;
    public int currentHealth;
    public PlayerHealthBar healthBar;
    //public GameObject deathEffect;

    private Rigidbody2D m_rb2D;
    private Animator m_animator;
    private BoxCollider2D m_boxCollider2D;
    private PlayerMovement m_playerMovement;
    private static readonly int Hurt = Animator.StringToHash("Hurt");
    private static readonly int Death = Animator.StringToHash("Death");

    public void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        m_animator = GetComponent<Animator>();
        m_rb2D = GetComponent<Rigidbody2D>();
        m_boxCollider2D = GetComponent<BoxCollider2D>();
        m_playerMovement = GetComponent<PlayerMovement>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        m_animator.SetTrigger(Hurt);
        m_rb2D.velocity = new Vector2(0, m_rb2D.velocity.y);
        healthBar.SetCurrentHealth(currentHealth);
        if (currentHealth <= 0)
            Die();
    }

    // Update is called once per frame
    void Die()
    {
        m_animator.SetTrigger(Death);
        m_rb2D.simulated = false;
        m_playerMovement.enabled = false;
        enabled = false;
        //Instantiate(deathEffect, transform.position, Quaternion.identity);
        //Destroy(gameObject);
    }
}
