using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHandler : MonoBehaviour
{
    public void Initialize(UnitData source, UnitData target, float speed)
    {
        StartCoroutine(FlyOverTime(source, target, speed));
    }

    private IEnumerator FlyOverTime(UnitData source, UnitData target, float speed)
    {
        Vector3 start = source.transform.position;
        Vector3 end = target.transform.position;
        transform.position = start;

        float duration = Vector3.Distance(start, end) / speed;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / duration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        GameLogic.AttackUnit(source, target);
        Destroy(gameObject);
    }
}
