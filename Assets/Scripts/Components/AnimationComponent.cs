using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationComponent : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Debug")]
    [SerializeField, ReadOnly] private bool isFacingRight;

    private void Start()
    {
        isFacingRight = true;
        animator.Play("Idle");
        spriteRenderer.flipX = !isFacingRight;
    }

    public void Movement(Vector2 velocity)
    {
        if (velocity.magnitude > 0.1f)
        {
            animator.Play("Run");
            FlipModel(velocity.normalized);
        }
        else
        {
            animator.Play("Idle");
        }
    }

    public void Attack()
    {
        animator.Play("Attack");
    }

    public void Die()
    {
        animator.Play("Die");
    }

    public float CurrentAnimationRatio() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

    private void FlipModel(Vector3 direction)
    {
        // If you are moving right and facing left, then flip
        if (direction.x > 0 && !isFacingRight)
        {
            isFacingRight = true;
            spriteRenderer.flipX = !isFacingRight;
        }
        // Else if you are moving left and facing right, also flip
        else if (direction.x < 0 && isFacingRight)
        {
            isFacingRight = false;
            spriteRenderer.flipX = !isFacingRight;
        }
    }
}
