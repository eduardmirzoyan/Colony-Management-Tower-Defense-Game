using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float height;

    public void Initialize(UnitData source, UnitData target)
    {
        StartCoroutine(FlyOverTime(source, target, speed, height));
    }

    private IEnumerator FlyOverTime(UnitData source, UnitData target, float speed, float height)
    {
        Vector3 startPosition = source.transform.position;
        Vector3 endPosition = target.transform.position;
        Transform endTransform = target.transform;
        transform.position = startPosition;

        float elapsed = 0f;
        float duration = Vector3.Distance(startPosition, endPosition) / speed;

        Vector3 previousPosition = startPosition;
        Vector3 currentPosition;
        Vector3 control = (startPosition + endPosition) / 2 + Vector3.up * height;
        while (elapsed < duration)
        {
            // Travel in a projectile motion
            float ratio = elapsed / duration;
            Vector3 ac = Vector3.Lerp(startPosition, control, ratio);
            Vector3 cb = Vector3.Lerp(control, endTransform.position, ratio);
            currentPosition = Vector3.Lerp(ac, cb, ratio);
            transform.position = currentPosition;

            // Rotate to face direction of motion
            Vector3 direction = (currentPosition - previousPosition).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            previousPosition = currentPosition;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endTransform.position;
        GameLogic.AttackUnit(source, target);
        Destroy(gameObject);
    }

    public IEnumerator Jump(Vector3Int startLocation, Vector3Int endLocation)
    {
        Vector3 startPosition = startLocation;
        Vector3 endPosition = endLocation;

        float elapsed = 0;
        float duration = 0.5f;
        float jumpHeight = 3f;

        Vector3 currentPosition;
        Vector3 control = (startPosition + endPosition) / 2 + Vector3.up * jumpHeight;
        while (elapsed < duration)
        {
            // Projectile motion
            float ratio = elapsed / duration;
            Vector3 ac = Vector3.Lerp(startPosition, control, ratio);
            Vector3 cb = Vector3.Lerp(control, endPosition, ratio);
            currentPosition = Vector3.Lerp(ac, cb, ratio);

            transform.position = currentPosition;

            // Increment time
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Set to final destination
        transform.position = endPosition;
    }
}
