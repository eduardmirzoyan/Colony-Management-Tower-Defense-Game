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

    public event Action<WorldData> OnGenerateWorld;
    public void TriggerOnGenrateWorld(WorldData worldData) => OnGenerateWorld?.Invoke(worldData);

    #region Followers

    public event Action<IFollower> OnEnterFollower;
    public event Action<IFollower> OnExitFollower;

    public event Action OnRallyStart;
    public event Action OnRallyEnd;

    public void TriggerOnEnterFollower(IFollower follower) => OnEnterFollower?.Invoke(follower);
    public void TriggerOnExitFollower(IFollower follower) => OnExitFollower?.Invoke(follower);

    public void TriggerOnRallyStart() => OnRallyStart?.Invoke();
    public void TriggerOnRallyEnd() => OnRallyEnd?.Invoke();

    #endregion

    #region Combat

    public event Action<UnitData> OnUnitSpawn;
    public event Action<UnitData> OnUnitTakeDamage;
    public event Action<UnitData> OnUnitDie;
    public event Action<UnitData, RoomData> OnUnitAssign;

    public void TriggerOnUnitSpawn(UnitData unitData) => OnUnitSpawn?.Invoke(unitData);
    public void TriggerOnUnitTakeDamage(UnitData unitData) => OnUnitTakeDamage?.Invoke(unitData);
    public void TriggerOnUnitDie(UnitData unitData) => OnUnitDie?.Invoke(unitData);
    public void TriggerOnUnitAssign(UnitData unitData, RoomData roomData) => OnUnitAssign?.Invoke(unitData, roomData);

    #endregion

    #region Structures

    public event Action<IStructure> OnEnterStructure;
    public event Action<IStructure> OnExitStructure;

    public void TriggerOnEnterStructure(IStructure structure) => OnEnterStructure?.Invoke(structure);
    public void TriggerOnExitStructure(IStructure structure) => OnExitStructure?.Invoke(structure);

    #endregion

    #region Rooms

    public event Action<RoomData> OnEnterRoom;
    public event Action<RoomData> OnExitRoom;
    public event Action<RoomData> OnDiscoverRoom;

    public void TriggerOnEnterRoom(RoomData roomData) => OnEnterRoom?.Invoke(roomData);
    public void TriggerOnExitRoom(RoomData roomData) => OnExitRoom?.Invoke(roomData);
    public void TriggerOnDiscoverRoom(RoomData roomData) => OnDiscoverRoom?.Invoke(roomData);

    #endregion

    #region Game States

    public event Action OnStateExpand;
    public event Action<WaveData> OnStatePrepare;
    public event Action OnStateDefend;

    public void TriggerOnStateExpand() => OnStateExpand?.Invoke();
    public void TriggerOnStatePrepare(WaveData waveData) => OnStatePrepare?.Invoke(waveData);
    public void TriggerOnStateDefend() => OnStateDefend?.Invoke();

    #endregion

    #region Gold

    public event Action<UnitData> OnGoldChange;
    public void TriggerOnGoldChange(UnitData unitData) => OnGoldChange?.Invoke(unitData);

    #endregion
}
