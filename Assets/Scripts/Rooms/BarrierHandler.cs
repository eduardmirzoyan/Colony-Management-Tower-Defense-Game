using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierHandler : MonoBehaviour, IBarrier
{
    [SerializeField] private SpriteRenderer intentRenderer;

    [SerializeField] private RoomData roomData;

    public void Initialize(RoomData roomData, Vector2 direction)
    {
        this.roomData = roomData;

        transform.position = roomData.worldPosition + (Vector3)direction * (roomData.size / 2 + 0.5f);
    }

    public void Raise()
    {
        GameManager.instance.DiscoverRoom(roomData);
    }

    public void HoverEnter()
    {
        intentRenderer.color = Color.yellow;
    }

    public void HoverExit()
    {
        intentRenderer.color = Color.white;
    }
}
