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
    }
}
