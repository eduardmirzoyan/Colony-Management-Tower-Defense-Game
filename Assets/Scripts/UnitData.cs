using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class UnitData : ScriptableObject
{
    public new string name;
    public int currentHP;
    public int maxHP;


    /// <summary>
    /// The transform of unit in real world.
    /// </summary>
    public Transform transform;

    /// <summary>
    /// The room this unit is assigned to.
    /// </summary>
    public RoomData roomData;

    public void Initialize(Transform transform, RoomData roomData)
    {
        this.transform = transform;
        this.roomData = roomData;
    }

    public UnitData Copy()
    {
        return Instantiate(this);
    }

    public override string ToString() => $"{name} [{currentHP}/{maxHP}]";
}
