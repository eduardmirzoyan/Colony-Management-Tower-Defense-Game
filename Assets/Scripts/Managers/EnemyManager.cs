using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private UnitData normalEnemy;
    [SerializeField] private UnitData fastEnemy;
    [SerializeField] private UnitData bossEnemy;

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
