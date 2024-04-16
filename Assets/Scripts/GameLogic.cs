using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameLogic
{
    public static void AttackUnit(UnitData attacker, UnitData victim)
    {
        victim.currentHP -= attacker.attackDamage;
        GameEvents.instance.TriggerOnUnitTakeDamage(victim);

        if (victim.currentHP <= 0)
        {
            victim.currentHP = 0;
            GameEvents.instance.TriggerOnUnitDie(victim);
        }
    }

    public static void AssignUnitToRoom(UnitData unitData, RoomData roomData)
    {
        if (roomData != null)
        {
            // Add to new room
            roomData.AddUnit(unitData);
            GameEvents.instance.TriggerOnUnitAssign(unitData, roomData);
        }
        else
        {
            // Remove self from current room
            unitData.roomData?.RemoveUnit(unitData);
            GameEvents.instance.TriggerOnUnitAssign(unitData, unitData.roomData);
        }

        unitData.roomData = roomData;
    }
}
