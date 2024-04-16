using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitHandler : MonoBehaviour, IStructure
{
    [Header("References")]
    [SerializeField] private SpriteRenderer intentRenderer;

    public void Use(UnitData _)
    {
        GameManager.instance.GameWin();
    }

    private void Start()
    {
        GameEvents.instance.OnEnterStructure += EventEnter;
        GameEvents.instance.OnExitStructure += EventExit;
        intentRenderer.enabled = false;
    }

    private void OnDestroy()
    {
        GameEvents.instance.OnEnterStructure -= EventEnter;
        GameEvents.instance.OnExitStructure -= EventExit;
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

    #endregion
}
