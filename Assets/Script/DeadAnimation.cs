using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadAnimation : MonoBehaviour
{
    private Animator m_animator;
    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_animator.SetTrigger("Hurt");
        m_animator.SetBool("isDeath", true);
        Physics2D.IgnoreLayerCollision(3, 9, true);
        Physics2D.IgnoreLayerCollision(8, 9, true);
        Physics2D.IgnoreLayerCollision(9, 9, true);
    }
}
