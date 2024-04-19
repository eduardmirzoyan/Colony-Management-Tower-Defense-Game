using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Data")]
    [SerializeField, ReadOnly] private UnitData unitData;

    [Header("Settings")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float radius;

    public void Initialize(UnitData source, UnitData target)
    {
        unitData = source;

        // Relocate in front of source
        Vector3 direction = (target.transform.position - source.transform.position).normalized;
        transform.position = source.transform.position + direction;

        // Face towards motion
        FaceDirection(direction);

        // Attack area
        DamageArea(transform.position, radius, enemyLayer);

        // Visuals
        StartCoroutine(DisperseOverTime());
    }

    private void DamageArea(Vector3 position, float radius, LayerMask layer)
    {
        var hits = Physics2D.OverlapCircleAll(position, radius, layer);
        foreach (var hit in hits)
            if (hit.gameObject.TryGetComponent(out EnemyHandler enemyHandler))
                GameLogic.AttackUnit(unitData, enemyHandler.UnitData);
    }

    private IEnumerator DisperseOverTime()
    {
        // Play animation
        animator.Play("Idle");

        // Wait until animation is over
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95f)
            yield return null;

        // Finish
        Destroy(gameObject);
    }

    private void FaceDirection(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
