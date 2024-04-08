using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordsmanSpawnerHandler : MonoBehaviour, ISpawner
{
    [Header("References")]
    [SerializeField] private GameObject swordsmanPrefab;
    [SerializeField] private Vector3 spawnOffset;

    public void Spawn()
    {
        Vector3 spawnPosition = transform.position + spawnOffset;
        Instantiate(swordsmanPrefab, spawnPosition, Quaternion.identity);
    }
}
