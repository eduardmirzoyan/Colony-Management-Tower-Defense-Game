using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierHandler : MonoBehaviour, IStructure
{
    [SerializeField] private SpriteRenderer intentRenderer;

    [SerializeField] private RoomData roomData;

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
    }

    private void OnDestroy()
    {
        GameEvents.instance.OnEnterStructure -= EventEnter;
        GameEvents.instance.OnExitStructure -= EventExit;
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
