using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public void EnableAnimator()
    {
        animator.enabled = true;
    }
}
