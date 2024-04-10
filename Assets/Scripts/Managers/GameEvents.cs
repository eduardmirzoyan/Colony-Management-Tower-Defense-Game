using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents instance;
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

    #region Followers

    public event Action<IFollower> OnEnterFollower;
    public event Action<IFollower> OnExitFollower;

    public void TriggerOnEnterFollower(IFollower follower) => OnEnterFollower?.Invoke(follower);
    public void TriggerOnExitFollower(IFollower follower) => OnExitFollower?.Invoke(follower);

    #endregion

    #region Combat

    public event Action<UnitData> OnUnitTakeDamage;
    public event Action<UnitData> OnUnitDie;

    public void TriggerOnUnitTakeDamage(UnitData unitData) => OnUnitTakeDamage?.Invoke(unitData);
    public void TriggerOnUnitDie(UnitData unitData) => OnUnitDie?.Invoke(unitData);

    #endregion

    #region Spawners

    public event Action<ISpawner> OnEnterSpawner;
    public event Action<ISpawner> OnExitSpawner;

    public void TriggerOnEnterSpawner(ISpawner spawner) => OnEnterSpawner?.Invoke(spawner);
    public void TriggerOnExitSpawner(ISpawner spawner) => OnExitSpawner?.Invoke(spawner);

    #endregion

    #region Rooms

    public event Action<RoomData> OnEnterRoom;
    public event Action<RoomData> OnExitRoom;
    public event Action<RoomData> OnDiscoverRoom;

    public void TriggerOnEnterRoom(RoomData roomData) => OnEnterRoom?.Invoke(roomData);
    public void TriggerOnExitRoom(RoomData roomData) => OnExitRoom?.Invoke(roomData);
    public void TriggerOnDiscoverRoom(RoomData roomData) => OnDiscoverRoom?.Invoke(roomData);

    #endregion
}
