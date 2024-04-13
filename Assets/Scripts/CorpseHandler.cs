using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("Settings")]
    [SerializeField] private float lingerDuration;

    public void Initialize(RuntimeAnimatorController controller)
    {
        animator.runtimeAnimatorController = controller;
        StartCoroutine(DieOverTime(lingerDuration));
    }

    private IEnumerator DieOverTime(float lingerDuration)
    {
        animator.Play("Die");

        float elapsed = 0f;
        while (elapsed < lingerDuration)
        {
            spriteRenderer.color = Color.Lerp(Color.white, Color.clear, elapsed / lingerDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
