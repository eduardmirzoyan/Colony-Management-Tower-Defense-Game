using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerSpawnerHandler : MonoBehaviour, ISpawner
{
    [Header("References")]
    [SerializeField] private SpriteRenderer intentRenderer;
    [SerializeField] private Vector3 spawnOffset;

    [Header("Data")]
    [SerializeField] private RoomData roomData;
    [SerializeField] private UnitData unitData;

    [Header("Settings")]
    [SerializeField] private int cost;
    [SerializeField] private bool isRanged;

    private void Start()
    {
        GameEvents.instance.OnEnterSpawner += EventEnter;
        GameEvents.instance.OnExitSpawner += EventExit;
    }

    private void OnDestroy()
    {
        GameEvents.instance.OnEnterSpawner -= EventEnter;
        GameEvents.instance.OnExitSpawner -= EventExit;
    }

    public void Initialize(RoomData roomData)
    {
        this.roomData = roomData;
        intentRenderer.enabled = false;
    }

    public void Spawn(UnitData player)
    {
        if (player.goldHeld < cost)
            return;

        // Spawn unit
        SpawnManager.instance.SpawnAlly(unitData, roomData, transform.position + spawnOffset, isRanged);

        // Reduce hold
        player.goldHeld -= cost;
        GameEvents.instance.TriggerOnGoldLoss(player);
    }

    #region Events

    private void EventEnter(ISpawner spawner)
    {
        if (!spawner.Equals(this)) return;

        intentRenderer.enabled = true;
    }

    private void EventExit(ISpawner spawner)
    {
        if (!spawner.Equals(this)) return;

        intentRenderer.enabled = false;
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Vector3 spawnPosition = transform.position + spawnOffset;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spawnPosition, 0.25f);
    }
}
