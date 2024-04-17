using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHandler : MonoBehaviour, IStructure
{
    [Header("References")]
    [SerializeField] private DamageFlash damageFlash;
    [SerializeField] private SpriteRenderer intentRenderer;
    [SerializeField] private Collider2D collider2d;

    [Header("Data")]
    [SerializeField] private UnitData unitData;

    public void Initialize(UnitData unitData)
    {
        this.unitData = unitData;
        intentRenderer.color = Color.clear;
    }

    public void Use(UnitData _)
    {
        // Start wave
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

        intentRenderer.color = Color.white;
    }

    private void EventExit(IStructure structure)
    {
        if (!structure.Equals(this)) return;

        intentRenderer.color = Color.clear;
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
        collider2d.enabled = false;
    }

    private void EventStatePrepare(WaveData _)
    {
        // Enable interaction
        collider2d.enabled = true;
    }

    #endregion
}
