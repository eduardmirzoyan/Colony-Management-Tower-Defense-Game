using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerSpawnerHandler : MonoBehaviour, ISpawner
{
    [Header("References")]
    [SerializeField] private GameObject followerPrefab;
    [SerializeField] private Vector3 spawnOffset;

    [Header("Data")]
    [SerializeField] private RoomData roomData;
    [SerializeField] private UnitData unitData;

    [Header("Settings")]
    [SerializeField] private int cost;

    public void Initialize(RoomData roomData)
    {
        this.roomData = roomData;
    }

    public void Spawn(UnitData player)
    {
        if (player.goldHeld < cost)
            return;

        // Spawn unit
        SpawnManager.instance.SpawnAlly(unitData, roomData, transform.position + spawnOffset);

        // Reduce hold
        player.goldHeld -= cost;
        GameEvents.instance.TriggerOnGoldLoss();
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 spawnPosition = transform.position + spawnOffset;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spawnPosition, 0.25f);
    }
}
