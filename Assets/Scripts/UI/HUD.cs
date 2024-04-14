using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI healthLabel;
    [SerializeField] private TextMeshProUGUI goldLabel;
    [SerializeField] private TextMeshProUGUI unitsLabel;

    [Header("Data")]
    [SerializeField] private WorldData worldData;

    private void Start()
    {
        GameEvents.instance.OnGenerateWorld += EventGenerateWorld;
        GameEvents.instance.OnUnitTakeDamage += EventUpdateHealth;
        GameEvents.instance.OnGoldGain += EventUpdateGold;
        GameEvents.instance.OnGoldLoss += EventUpdateGold;
    }

    private void OnDestroy()
    {
        GameEvents.instance.OnGenerateWorld -= EventGenerateWorld;
        GameEvents.instance.OnUnitTakeDamage -= EventUpdateHealth;
        GameEvents.instance.OnGoldGain -= EventUpdateGold;
        GameEvents.instance.OnGoldLoss -= EventUpdateGold;
    }

    private void EventGenerateWorld(WorldData worldData)
    {
        this.worldData = worldData;

        EventUpdateHealth(worldData.baseData);
        EventUpdateGold(worldData.playerData);
        EventUpdateUnits(worldData.playerData);
    }

    private void EventUpdateHealth(UnitData unitData)
    {
        if (worldData.baseData != unitData) return;

        healthLabel.text = $"{unitData.currentHP}";
    }

    private void EventUpdateGold(UnitData unitData)
    {
        if (worldData.playerData != unitData) return;

        goldLabel.text = $"{unitData.goldHeld}";
    }

    private void EventUpdateUnits(UnitData unitData)
    {
        if (worldData.playerData != unitData) return;

        unitsLabel.text = "0";
    }
}
