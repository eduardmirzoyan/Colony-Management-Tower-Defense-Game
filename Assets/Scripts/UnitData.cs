using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class UnitData : ScriptableObject
{
    public new string name;

    [Header("Health")]
    public int currentHP;
    public int maxHP;

    [Header("Offensive")]
    public float aggroRange;
    public float attackRange;
    public float attackSpeed;
    public int attackDamage;

    [Header("Other")]
    public float moveSpeed;
    public int goldHeld;

    public bool IsDead { get { return currentHP <= 0; } }

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
