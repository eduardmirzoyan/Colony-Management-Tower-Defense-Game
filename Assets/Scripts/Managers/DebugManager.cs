using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private WorldData worldData;

    public static DebugManager instance;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            EnemyManager.instance.SpawnNormal(worldData.rooms[0]);
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            EnemyManager.instance.SpawnFast(worldData.rooms[0]);
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            EnemyManager.instance.SpawnBoss(worldData.rooms[0]);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            SpawnManager.instance.SpawnGold(worldData.baseData.roomData.worldPosition);
        }
    }
}
