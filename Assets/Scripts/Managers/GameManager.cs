using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private WorldData worldData;

    [SerializeField]
    private EnemySpawnerHandler[] enemySpawners;

    public static GameManager instance;
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

    private void Start()
    {
        // Create world
        WorldManager.instance.GenerateWorld(out worldData);
        InitializeSpawners();

        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        TransitionManager.instance.Initialize();

        yield return new WaitForSeconds(0.1f);

        DiscoverRoom(worldData.playerData.roomData);
        EnterRoom(worldData.playerData.roomData);

        yield return new WaitForSeconds(1f);

        TransitionManager.instance.OpenScene();
    }

    public void EnterRoom(RoomData roomData)
    {
        // Leave previous room
        if (worldData.playerData.roomData != null)
            GameEvents.instance.TriggerOnExitRoom(worldData.playerData.roomData);

        // Check if room was already discovered
        if (!roomData.isDiscovered)
            roomData.isDiscovered = true;

        // Enter room
        worldData.playerData.roomData = roomData;
        GameEvents.instance.TriggerOnEnterRoom(roomData);
    }

    public void DiscoverRoom(RoomData roomData)
    {
        // Discover room
        if (!roomData.isDiscovered)
            roomData.isDiscovered = true;

        GameEvents.instance.TriggerOnDiscoverRoom(roomData);
    }

    public void ExitMap()
    {
        // Win game!
        print("You win!");

        // Load new level
        TransitionManager.instance.ReloadScene();
    }

    public void AttackUnit(UnitData attacker, UnitData victim)
    {
        victim.currentHP -= attacker.attackDamage;
        if (victim.currentHP < 0) victim.currentHP = 0;
    }

    // TESTING
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            SpawnEnemy();
        }
    }

    #region Helpers

    private void InitializeSpawners()
    {
        FollowerSpawnerHandler[] followerSpawners = (FollowerSpawnerHandler[])FindObjectsOfType(typeof(FollowerSpawnerHandler));
        foreach (var spawner in followerSpawners)
        {
            spawner.Initialize(worldData.playerData.roomData);
        }

        enemySpawners = (EnemySpawnerHandler[])FindObjectsOfType(typeof(EnemySpawnerHandler));
        foreach (var spawner in enemySpawners)
        {
            spawner.Initialize(worldData.playerData.roomData, worldData);
        }
    }

    private void SpawnEnemy()
    {
        foreach (var spawner in enemySpawners)
        {
            spawner.Spawn();
            break;
        }
    }

    #endregion
}
