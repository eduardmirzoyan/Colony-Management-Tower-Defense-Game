using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private UnitData unitData;

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

    public void Spawn(RoomData roomData)
    {
        SpawnManager.instance.SpawnEnemy(unitData, roomData);
    }
}
