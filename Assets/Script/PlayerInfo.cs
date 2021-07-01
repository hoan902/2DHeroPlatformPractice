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

    public void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        m_animator = GetComponent<Animator>();
        m_rb2D = GetComponent<Rigidbody2D>();
        m_boxCollider2D = GetComponent<BoxCollider2D>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        m_animator.SetTrigger("Hurt");
        m_rb2D.velocity = new Vector2(0, m_rb2D.velocity.y);
        if(!m_boxCollider2D.enabled)
        {
            m_boxCollider2D.enabled = true;
        }    
        healthBar.SetCurrentHealth(currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Update is called once per frame
    void Die()
    {
        m_animator.SetTrigger("Death");
        PlayerMovement.Instance.GetComponent<Rigidbody2D>().simulated = false;
        PlayerMovement.Instance.enabled = false;
        enabled = false;
        //Instantiate(deathEffect, transform.position, Quaternion.identity);
        //Destroy(gameObject);
    }
}
