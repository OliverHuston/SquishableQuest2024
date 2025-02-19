using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = this.gameObject.GetComponentInChildren<Animator>();
    }

    public void PlayMoveAnim()
    {
        animator.SetTrigger("move");
    }
}