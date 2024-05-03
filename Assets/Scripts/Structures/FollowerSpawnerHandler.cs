using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FollowerSpawnerHandler : MonoBehaviour, IStructure
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI costLabel;
    [SerializeField] private Vector3 spawnOffset;
    [SerializeField] private SpriteRenderer spotRenderer;

    [Header("Data")]
    [SerializeField] private RoomData roomData;
    [SerializeField] private UnitData unitData;

    [Header("Settings")]
    [SerializeField] private int cost;
    [SerializeField] private bool isRanged;

    private void Start()
    {
        GameEvents.instance.OnEnterStructure += EventEnter;
        GameEvents.instance.OnExitStructure += EventExit;
        GameEvents.instance.OnGoldChange += EventGoldChange;
    }

    private void OnDestroy()
    {
        GameEvents.instance.OnEnterStructure -= EventEnter;
        GameEvents.instance.OnExitStructure -= EventExit;
        GameEvents.instance.OnGoldChange -= EventGoldChange;
    }

    public void Initialize(RoomData roomData)
    {
        this.roomData = roomData;
        costLabel.text = $"{cost}";
        canvasGroup.alpha = 0f;
        spotRenderer.color = new Color(0f, 0f, 0f, 0.25f);
    }

    public void Use(UnitData player)
    {
        if (player.goldHeld < cost)
            return;

        // Spawn unit
        SpawnManager.instance.SpawnAlly(unitData, roomData, transform.position + spawnOffset, isRanged);

        // Reduce hold
        player.goldHeld -= cost;
        GameEvents.instance.TriggerOnGoldChange(player);
    }

    #region Events

    private void EventEnter(IStructure structure)
    {
        if (!structure.Equals(this)) return;

        spotRenderer.color = new Color(1f, 1f, 1f, 0.25f);
        canvasGroup.alpha = 1f;
    }

    private void EventExit(IStructure structure)
    {
        if (!structure.Equals(this)) return;

        spotRenderer.color = new Color(0f, 0f, 0f, 0.25f);
        canvasGroup.alpha = 0f;
    }

    private void EventGoldChange(UnitData unitData)
    {
        costLabel.color = unitData.goldHeld >= cost ? Color.white : Color.red;
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Vector3 spawnPosition = transform.position + spawnOffset;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spawnPosition, 0.25f);
    }
}
