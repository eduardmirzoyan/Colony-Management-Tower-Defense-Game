using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Upon spawning, chases target, for now...
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyHandler : MonoBehaviour
{
    private enum EnemyState { Chasing, Attacking, Dying }

    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private AnimationComponent animationn;

    [Header("Data")]
    [SerializeField, ReadOnly] private UnitData unitData;

    [Header("Debug")]
    [SerializeField] private UnitData player;
    [SerializeField] private UnitData target;
    [SerializeField] private EnemyState state;
    [SerializeField] private bool printLogs;

    public UnitData UnitData { get { return unitData; } }

    public void Initialize(UnitData unitData, UnitData player)
    {
        this.unitData = unitData;
        this.player = player;
        state = EnemyState.Chasing;

        gameObject.name = unitData.ToString();
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.avoidancePriority = Random.Range(0, 1000);
    }

    private void Start()
    {
        GameEvents.instance.OnUnitTakeDamage += EventTakeDamage;
        GameEvents.instance.OnUnitDie += EventDie;
    }

    private void OnDestroy()
    {
        GameEvents.instance.OnUnitTakeDamage -= EventTakeDamage;
        GameEvents.instance.OnUnitDie -= EventDie;
    }

    private void Update()
    {
        switch (state)
        {
            case EnemyState.Chasing:

                ChasePlayer();

                if (unitData.IsDead)
                {
                    GameEvents.instance.TriggerOnUnitDie(unitData);

                    Destroy(gameObject, 2f);
                    animationn.Die();
                    state = EnemyState.Dying;
                }

                break;
            case EnemyState.Attacking:
                // TODO
                break;
            case EnemyState.Dying:
                // Do nothing until gone...
                break;
        }
    }

    #region Helpers

    private void ChasePlayer()
    {
        // Chase target
        if (player != null)
            agent.SetDestination(player.transform.position);
        else // Go back to spawn
            agent.SetDestination(unitData.roomData.worldPosition);

        animationn.Movement(agent.velocity);
    }

    #endregion

    #region Events

    public void EventTakeDamage(UnitData unitData)
    {
        // TODO
    }

    public void EventDie(UnitData unitData)
    {
        // 
    }

    #endregion
}
