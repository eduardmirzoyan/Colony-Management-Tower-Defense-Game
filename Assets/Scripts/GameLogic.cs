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
}
