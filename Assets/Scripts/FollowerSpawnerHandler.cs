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

    public void Initialize(RoomData roomData)
    {
        this.roomData = roomData;
    }

    public void Spawn()
    {
        UnitData copy = unitData.Copy();
        Vector3 spawnPosition = transform.position + spawnOffset;
        var swordsman = Instantiate(followerPrefab, spawnPosition, Quaternion.identity).GetComponent<FollowerHandler>();
        copy.Initialize(swordsman.transform, roomData);

        swordsman.Initialize(copy);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 spawnPosition = transform.position + spawnOffset;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spawnPosition, 0.25f);
    }
}
