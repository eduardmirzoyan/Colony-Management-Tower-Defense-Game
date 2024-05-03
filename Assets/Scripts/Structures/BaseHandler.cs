using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BaseHandler : MonoBehaviour, IStructure
{
    [Header("References")]
    [SerializeField] private DamageFlash damageFlash;
    [SerializeField] private TextMeshProUGUI instructionLabel;
    [SerializeField] private SpriteRenderer spotRenderer;

    [Header("Data")]
    [SerializeField] private UnitData unitData;

    private bool isInteractable;

    public void Initialize(UnitData unitData)
    {
        this.unitData = unitData;
        isInteractable = false;
        instructionLabel.text = string.Empty;
        instructionLabel.enabled = false;
        spotRenderer.color = new Color(0f, 0f, 0f, 0.25f);
    }

    public void Use(UnitData _)
    {
        // Start wave
        if (isInteractable)
            GameManager.instance.StartWave();
    }

    private void Start()
    {
        GameEvents.instance.OnEnterStructure += EventEnter;
        GameEvents.instance.OnExitStructure += EventExit;
        GameEvents.instance.OnUnitTakeDamage += EventTakeDamage;
        GameEvents.instance.OnUnitDie += EventDie;
        GameEvents.instance.OnStateExpand += EventStateExpand;
        GameEvents.instance.OnStatePrepare += EventStatePrepare;
        GameEvents.instance.OnStateDefend += EventStateExpand;
    }

    private void OnDestroy()
    {
        GameEvents.instance.OnEnterStructure -= EventEnter;
        GameEvents.instance.OnExitStructure -= EventExit;
        GameEvents.instance.OnUnitTakeDamage -= EventTakeDamage;
        GameEvents.instance.OnUnitDie -= EventDie;
        GameEvents.instance.OnStateExpand -= EventStateExpand;
        GameEvents.instance.OnStatePrepare -= EventStatePrepare;
        GameEvents.instance.OnStateDefend -= EventStateExpand;
    }

    #region Events

    private void EventEnter(IStructure structure)
    {
        if (!structure.Equals(this)) return;

        spotRenderer.color = new Color(1f, 1f, 1f, 0.25f);
        instructionLabel.enabled = true;
    }

    private void EventExit(IStructure structure)
    {
        if (!structure.Equals(this)) return;

        spotRenderer.color = new Color(0f, 0f, 0f, 0.25f);
        instructionLabel.enabled = false;
    }

    private void EventTakeDamage(UnitData unitData)
    {
        if (this.unitData != unitData) return;

        damageFlash.Flash();
    }

    private void EventDie(UnitData unitData)
    {
        if (this.unitData != unitData) return;

        // Game over!
        GameManager.instance.GameLose();
    }

    private void EventStateExpand()
    {
        // Disable interaction
        isInteractable = false;
        instructionLabel.text = "";
    }

    private void EventStatePrepare(WaveData _)
    {
        // Enable interaction
        isInteractable = true;
        instructionLabel.text = "Start?";
    }

    #endregion
}
