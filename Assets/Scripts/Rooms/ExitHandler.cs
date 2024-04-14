using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitHandler : MonoBehaviour, ISpawner
{
    [Header("References")]
    [SerializeField] private SpriteRenderer intentRenderer;

    public void Spawn(UnitData _)
    {
        GameManager.instance.GameWin();
    }

    private void Start()
    {
        GameEvents.instance.OnEnterSpawner += EventEnter;
        GameEvents.instance.OnExitSpawner += EventExit;
        intentRenderer.enabled = false;
    }

    private void OnDestroy()
    {
        GameEvents.instance.OnEnterSpawner -= EventEnter;
        GameEvents.instance.OnExitSpawner -= EventExit;
    }

    #region Events

    private void EventEnter(ISpawner spawner)
    {
        if (!spawner.Equals(this)) return;

        intentRenderer.enabled = true;
    }

    private void EventExit(ISpawner spawner)
    {
        if (!spawner.Equals(this)) return;

        intentRenderer.enabled = false;
    }

    #endregion
}
