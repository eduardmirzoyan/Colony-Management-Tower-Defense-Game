using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Upon spawning, chases target, for now...
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyHandler : MonoBehaviour
{
    private enum EnemyState { Chasing, Aggravated, Attacking }
    private const int MAX_PRIORITY = 99;

    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private AnimationComponent animationn;
    [SerializeField] private DamageFlash damageFlash;
    [SerializeField] private LayerMask allyLayer;
    [SerializeField] private GameObject goldDropPrefab;

    [Header("Data")]
    [SerializeField, ReadOnly] private UnitData unitData;

    [Header("Debug")]
    [SerializeField, ReadOnly] private UnitData baseData;
    [SerializeField, ReadOnly] private UnitData target;
    [SerializeField, ReadOnly] private EnemyState state;
    [SerializeField, ReadOnly] private float attackTimer;

    public UnitData UnitData { get { return unitData; } }

    public void Initialize(UnitData unitData, UnitData baseData)
    {
        this.unitData = unitData;
        this.baseData = baseData;
        state = EnemyState.Chasing;

        gameObject.name = unitData.ToString();
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.avoidancePriority = Random.Range(0, MAX_PRIORITY);
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

                GoToBase();

                bool near = NearBase();
                if (near)
                {
                    attackTimer = unitData.attackSpeed;
                    state = EnemyState.Aggravated;
                }

                bool found = SearchForTarget();
                if (found)
                {
                    attackTimer = unitData.attackSpeed;
                    state = EnemyState.Aggravated;
                }

                break;
            case EnemyState.Aggravated:

                bool attacked = ChaseAndAttackTarget();
                if (attacked)
                {
                    state = EnemyState.Attacking;
                }
                else if (target.transform == null)
                {
                    target = null;
                    state = EnemyState.Chasing;
                }

                break;
            case EnemyState.Attacking:

                // Wait until animation is over
                if (animationn.CurrentAnimationOver())
                {
                    attackTimer = unitData.attackSpeed;
                    state = target.transform == null ? EnemyState.Chasing : EnemyState.Aggravated;
                }

                break;
        }
    }

    #region Helpers

    private bool SearchForTarget()
    {
        var hit = Physics2D.OverlapCircle(transform.position, unitData.aggroRange, allyLayer);
        if (hit && hit.gameObject.TryGetComponent(out FollowerHandler followerHandler))
        {
            target = followerHandler.UnitData;
            return true;
        }

        return false;
    }

    private bool NearBase()
    {
        if (Vector2.Distance(transform.position, baseData.transform.position) <= unitData.attackRange)
        {
            target = baseData;
            return true;
        }

        return false;
    }

    private void GoToBase()
    {
        // Chase target
        if (baseData != null)
            agent.SetDestination(baseData.transform.position);
        else // Go back to spawn
            agent.SetDestination(unitData.roomData.worldPosition);

        animationn.Movement(agent.velocity);
    }

    private bool ChaseAndAttackTarget()
    {
        if (Vector2.Distance(transform.position, target.transform.position) > unitData.attackRange)
        {
            // Get closer
            agent.SetDestination(target.transform.position);
            animationn.Movement(agent.velocity);
        }
        else
        {
            if (attackTimer <= 0)
            {
                GameLogic.AttackUnit(unitData, target);

                animationn.Attack();
                return true;
            }
            else
            {
                animationn.Movement(agent.velocity);
                attackTimer -= Time.deltaTime;
            }
        }

        return false;
    }

    #endregion

    #region Events

    public void EventTakeDamage(UnitData unitData)
    {
        if (this.unitData != unitData) return;

        damageFlash.Flash();
    }

    public void EventDie(UnitData unitData)
    {
        if (this.unitData != unitData) return;

        // Stop moving
        agent.isStopped = true;

        // Reduce wave
        GameManager.instance.WaveReduced();

        // Drop gold if possible
        if (unitData.goldHeld > 0)
            Instantiate(goldDropPrefab, transform.position, Quaternion.identity).GetComponent<GoldDropHandler>().Initialize(unitData.goldHeld);

        // Create corpse
        SpawnManager.instance.SpawnCorpse(unitData);

        // Destroy self
        Destroy(gameObject);
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        if (unitData == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, unitData.aggroRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, unitData.attackRange);

        if (target?.transform == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, target.transform.position);
    }
}
