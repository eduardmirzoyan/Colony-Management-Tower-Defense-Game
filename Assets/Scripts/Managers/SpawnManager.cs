using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject meleePrefab;
    [SerializeField] private GameObject rangedPrefab;
    [SerializeField] private GameObject corpsePrefab;

    [Header("Debug")]
    [SerializeField, ReadOnly] private WorldData worldData;

    public static SpawnManager instance;
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

    public void Initialize(WorldData worldData)
    {
        this.worldData = worldData;
    }

    public void SpawnAlly(UnitData unitData, RoomData roomData, Vector3 position, bool isRanged = false)
    {
        GameObject prefab = isRanged ? rangedPrefab : meleePrefab;

        UnitData copy = unitData.Copy();
        Vector3 spawnPosition = position;
        var follower = Instantiate(prefab, spawnPosition, Quaternion.identity).GetComponent<FollowerHandler>();
        copy.Initialize(follower.transform, roomData);

        follower.Initialize(copy);
    }

    public void SpawnEnemy(UnitData unitData, RoomData roomData)
    {
        UnitData copy = unitData.Copy();
        Vector3 spawnPosition = roomData.worldPosition;
        var enemyHandler = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity).GetComponent<EnemyHandler>();
        copy.Initialize(enemyHandler.transform, roomData);

        enemyHandler.Initialize(copy, worldData.baseData);
    }

    public void SpawnCorpse(UnitData unitData)
    {
        Animator animator = unitData.transform.GetComponent<Animator>();
        Instantiate(corpsePrefab, transform.position, Quaternion.identity).GetComponent<CorpseHandler>().Initialize(animator.runtimeAnimatorController);
    }
}
