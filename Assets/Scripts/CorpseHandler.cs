using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer shadowRenderer;
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
            Color color = Color.Lerp(Color.white, Color.clear, elapsed / lingerDuration);
            spriteRenderer.color = color;
            shadowRenderer.color = color;

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
