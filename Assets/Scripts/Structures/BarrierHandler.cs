using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierHandler : MonoBehaviour, IStructure
{
    [Header("References")]
    [SerializeField] private SpriteRenderer intentRenderer;
    [SerializeField] private Collider2D collider2d;
    [SerializeField] private Sprite enabledSprite;
    [SerializeField] private Sprite disabledSprite;
    [SerializeField] private Color disabledColor;

    [Header("Debug")]
    [SerializeField, ReadOnly] private RoomData roomData;

    public void Initialize(RoomData roomData, Vector2 direction)
    {
        this.roomData = roomData;

        transform.position = roomData.worldPosition + (Vector3)direction * (roomData.size / 2 + 0.5f);
    }

    public void Use(UnitData _)
    {
        GameManager.instance.DiscoverRoom(roomData);
    }

    private void Start()
    {
        GameEvents.instance.OnEnterStructure += EventEnter;
        GameEvents.instance.OnExitStructure += EventExit;
        GameEvents.instance.OnStateExpand += EventStateExpand;
        GameEvents.instance.OnStatePrepare += EventStatePrepare;
    }

    private void OnDestroy()
    {
        GameEvents.instance.OnEnterStructure -= EventEnter;
        GameEvents.instance.OnExitStructure -= EventExit;
        GameEvents.instance.OnStateExpand -= EventStateExpand;
        GameEvents.instance.OnStatePrepare -= EventStatePrepare;
    }

    #region Events

    private void EventEnter(IStructure structure)
    {
        if (!structure.Equals(this)) return;

        intentRenderer.color = Color.yellow;
    }

    private void EventExit(IStructure structure)
    {
        if (!structure.Equals(this)) return;

        intentRenderer.color = Color.white;
    }

    private void EventStateExpand()
    {
        // Allow interaction
        intentRenderer.sprite = enabledSprite;
        intentRenderer.color = Color.white;
        collider2d.enabled = true;
    }

    private void EventStatePrepare(WaveData _)
    {
        // Disable itneraction
        intentRenderer.sprite = disabledSprite;
        intentRenderer.color = disabledColor;
        collider2d.enabled = false;
    }

    #endregion

    public void HoverEnter()
    {
        intentRenderer.color = Color.yellow;
    }

    public void HoverExit()
    {
        intentRenderer.color = Color.white;
    }
}
