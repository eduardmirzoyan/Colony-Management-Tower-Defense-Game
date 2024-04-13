using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Vector3 spawnOffset;

    [Header("Data")]
    [SerializeField] private WorldData worldData;
    [SerializeField] private RoomData roomData;
    [SerializeField] private UnitData unitData;

    public void Initialize(RoomData roomData, WorldData worldData)
    {
        this.worldData = worldData;
        this.roomData = roomData;
    }

    public void Spawn()
    {
        UnitData copy = unitData.Copy();
        Vector3 spawnPosition = transform.position + spawnOffset;
        var enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity).GetComponent<EnemyHandler>();
        copy.Initialize(enemy.transform, roomData);

        enemy.Initialize(copy, worldData.playerData);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 spawnPosition = transform.position + spawnOffset;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spawnPosition, 0.25f);
    }
}
