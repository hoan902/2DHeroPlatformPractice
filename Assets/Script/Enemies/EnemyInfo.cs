using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : MonoBehaviour
{
    public int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private EnemyHealthBar healthBar;
    [SerializeField] private GameObject deathEffect;
    
    [Header("---- Audio Sounds Config")]
    [SerializeField] private AudioClip m_hurtAudio;
    [SerializeField] private AudioClip m_dieAudio;

    private Rigidbody2D m_rb2D;
    private Animator m_animator;
    
    private static readonly int Hurt = Animator.StringToHash("Hurt");

    public void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        m_animator = GetComponent<Animator>();
        m_rb2D = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        m_animator.SetTrigger(Hurt);
        m_rb2D.velocity = new Vector2(0, m_rb2D.velocity.y);
        healthBar.SetCurrentHealth(currentHealth);
        SoundManager.PlaySound3D(m_hurtAudio, 20f, false, transform.position);
        if (currentHealth <= 0)
            Die();
    }

    // Update is called once per frame
    void Die()
    {
        SoundManager.PlaySound3D(m_dieAudio, 20f, false, transform.position);
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
