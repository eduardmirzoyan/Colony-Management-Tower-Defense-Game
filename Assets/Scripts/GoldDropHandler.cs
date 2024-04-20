using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldDropHandler : MonoBehaviour
{
    private const int GOLD_VALUE = 1;

    [Header("References")]
    [SerializeField] private CircleCollider2D collider2d;

    [Header("Settings")]
    [SerializeField] private float vaccumRange;
    [SerializeField] private float pickupRange;
    [SerializeField] private float vaccumSpeed;
    [SerializeField] private float dropDuration;

    private Coroutine coroutine;

    public void Initialize()
    {
        collider2d.radius = vaccumRange;
        StartCoroutine(DropOverTime(transform.position, transform.position + (Vector3)Random.insideUnitCircle, dropDuration));
    }

    private IEnumerator DropOverTime(Vector3 startPosition, Vector3 endPosition, float duration)
    {
        float elasped = 0f;
        while (elasped < duration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elasped / duration);

            elasped += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (coroutine != null) return;

        // If collides with player, start moving towards him
        if (other.gameObject.TryGetComponent(out PlayerHandler playerHandler))
        {
            StopAllCoroutines();
            coroutine = StartCoroutine(VaccumOverTime(playerHandler.UnitData, vaccumSpeed));
        }
    }

    private IEnumerator VaccumOverTime(UnitData unitData, float speed)
    {
        Transform target = unitData.transform;

        float elasped = 0f;
        while (true)
        {
            // Move towards target (faster over time)
            transform.position = Vector3.MoveTowards(transform.position, target.position, elasped);

            if (Vector3.Distance(transform.position, target.position) <= pickupRange)
            {
                // Collect
                Pickup(unitData);

                // Destroy this
                Destroy(gameObject);

                break;
            }

            elasped += Time.deltaTime;
            yield return null;
        }
    }

    private void Pickup(UnitData unitData)
    {
        // Add gold to unit
        unitData.goldHeld += GOLD_VALUE;

        // Event
        GameEvents.instance.TriggerOnGoldChange(unitData);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, pickupRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, vaccumRange);
    }
}
