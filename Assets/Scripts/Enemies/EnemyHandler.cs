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
    [SerializeField] private float attackTimestamp;

    [Header("Debug")]
    [SerializeField, ReadOnly] private UnitData baseData;
    [SerializeField, ReadOnly] private UnitData target;
    [SerializeField, ReadOnly] private EnemyState state;
    [SerializeField, ReadOnly] private float attackTimer;
    [SerializeField, ReadOnly] private bool hasAttacked;

    public UnitData UnitData { get { return unitData; } }

    public void Initialize(UnitData unitData, UnitData baseData)
    {
        this.unitData = unitData;
        this.baseData = baseData;
        state = EnemyState.Chasing;
        agent.speed = unitData.moveSpeed;

        gameObject.name = unitData.ToString();
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = 0f;
        agent.avoidancePriority = Random.Range(0, MAX_PRIORITY);

        // Small offset to fix bug?
        transform.position += new Vector3(0.1f, 0.1f);
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

                // Reduce attack timer 
                if (attackTimer > 0)
                    attackTimer -= Time.deltaTime;

                if (target.transform == null)
                {
                    target = null;
                    state = EnemyState.Chasing;
                    return;
                }

                bool canAttack = ChaseAndAttackTarget();
                if (canAttack)
                {
                    attackTimer = unitData.attackSpeed;
                    animationn.Attack();
                    hasAttacked = false;
                    agent.isStopped = true;
                    state = EnemyState.Attacking;
                }

                break;
            case EnemyState.Attacking:

                // Reduce attack timer 
                if (attackTimer > 0)
                    attackTimer -= Time.deltaTime;

                // Attack part-way in animation
                float ratio = animationn.CurrentAnimationRatio();
                if (ratio >= attackTimestamp && !hasAttacked && target.transform != null)
                {
                    GameLogic.AttackUnit(unitData, target);
                    hasAttacked = true;
                }

                // Wait until animation is over
                if (ratio >= 0.95f)
                {
                    agent.isStopped = false;
                    state = EnemyState.Aggravated;
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
        // Get closer if far
        if (Vector2.Distance(transform.position, target.transform.position) > unitData.attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(target.transform.position);

        }
        // Stop moving and wait
        else
        {
            agent.isStopped = true;
            if (attackTimer <= 0)
                return true;
        }

        animationn.Movement(agent.velocity);

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
        for (int i = 0; i < unitData.goldHeld; i++)
        {
            SpawnManager.instance.SpawnGold(transform.position);
        }

        // Create corpse
        SpawnManager.instance.SpawnCorpse(transform);

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
