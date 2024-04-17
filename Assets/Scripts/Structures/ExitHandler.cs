using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitHandler : MonoBehaviour, IStructure
{
    [Header("References")]
    [SerializeField] private SpriteRenderer intentRenderer;
    [SerializeField] private Collider2D collider2d;

    public void Use(UnitData _)
    {
        GameManager.instance.GameWin();
    }

    private void Start()
    {
        GameEvents.instance.OnEnterStructure += EventEnter;
        GameEvents.instance.OnExitStructure += EventExit;
        GameEvents.instance.OnStateExpand += EventStateExpand;
        GameEvents.instance.OnStatePrepare += EventStatePrepare;
        GameEvents.instance.OnStateDefend += EventStateExpand;
    }

    private void OnDestroy()
    {
        GameEvents.instance.OnEnterStructure -= EventEnter;
        GameEvents.instance.OnExitStructure -= EventExit;
        GameEvents.instance.OnStateExpand -= EventStateExpand;
        GameEvents.instance.OnStatePrepare -= EventStatePrepare;
        GameEvents.instance.OnStateDefend -= EventStateExpand;
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

    private void EventStateExpand()
    {
        // Allow interaction
        collider2d.enabled = true;
    }

    private void EventStatePrepare(WaveData _)
    {
        // Disable interaction
        collider2d.enabled = false;
    }

    #endregion
}
