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
        if (Input.GetKeyDown(KeyCode.G))
        {
            EnemyManager.instance.Spawn(worldData.rooms[0]);
        }
    }
}
