using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Vector3 spawnOffset;

    [Header("Data")]
    [SerializeField] private UnitData unitData;
    [SerializeField] private UnitData playerData;

    public static EnemyManager instance;
    private void Awake()
    {
        // Singleton Logic
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void Initialize(UnitData playerData)
    {
        this.playerData = playerData;
    }

    public void Spawn(RoomData roomData)
    {
        UnitData copy = unitData.Copy();
        Vector3 spawnPosition = (Vector3)roomData.worldPosition + spawnOffset;
        var enemyHandler = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity).GetComponent<EnemyHandler>();
        copy.Initialize(enemyHandler.transform, roomData);

        enemyHandler.Initialize(copy, playerData);
    }
}
