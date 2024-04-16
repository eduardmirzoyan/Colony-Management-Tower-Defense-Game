using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHandler : MonoBehaviour, IStructure
{
    [Header("References")]
    [SerializeField] private DamageFlash damageFlash;
    [SerializeField] private SpriteRenderer intentRenderer;

    [Header("Data")]
    [SerializeField] private UnitData unitData;

    public UnitData UnitData { get { return unitData; } }

    public void Initialize(UnitData unitData)
    {
        this.unitData = unitData;
        intentRenderer.enabled = false;
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
    }

    private void OnDestroy()
    {
        GameEvents.instance.OnEnterStructure -= EventEnter;
        GameEvents.instance.OnExitStructure -= EventExit;
        GameEvents.instance.OnUnitTakeDamage -= EventTakeDamage;
        GameEvents.instance.OnUnitDie -= EventDie;
    }

    #region Events

    private void EventEnter(IStructure structure)
    {
        if (!structure.Equals(this)) return;

        intentRenderer.enabled = true;
    }

    private void EventExit(IStructure structure)
    {
        if (!structure.Equals(this)) return;

        intentRenderer.enabled = false;
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

    #endregion
}
