using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { Normal, Fast, Boss }

public class EnemyManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private UnitData normalEnemy;
    [SerializeField] private UnitData fastEnemy;
    [SerializeField] private UnitData bossEnemy;

    public Dictionary<EnemyType, int> EnemyValueTable = new()
    {
        [EnemyType.Normal] = 1,
        [EnemyType.Fast] = 1,
        [EnemyType.Boss] = 5,
    };

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

    public void Spawn(EnemyType enemyType, RoomData roomData)
    {
        switch (enemyType)
        {
            case EnemyType.Normal:
                SpawnNormal(roomData);
                break;

            case EnemyType.Fast:
                SpawnFast(roomData);
                break;

            default: // Spawn normal for now
                SpawnNormal(roomData);
                break;
        }
    }

    public void SpawnNormal(RoomData roomData)
    {
        SpawnManager.instance.SpawnEnemy(normalEnemy, roomData);
    }

    public void SpawnFast(RoomData roomData)
    {
        SpawnManager.instance.SpawnEnemy(fastEnemy, roomData);
    }

    public void SpawnBoss(RoomData roomData)
    {
        SpawnManager.instance.SpawnEnemy(bossEnemy, roomData);
    }
}
