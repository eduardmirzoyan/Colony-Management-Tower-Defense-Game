using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierHandler : MonoBehaviour, IBarrier
{
    [SerializeField] private RoomData roomData;

    public void Initialize(RoomData roomData, Vector2 direction)
    {
        this.roomData = roomData;

        transform.position = roomData.worldPosition + direction * (roomData.size / 2 + 0.5f);
    }

    public void Raise()
    {
        GameManager.instance.DiscoverRoom(roomData);
    }
}
