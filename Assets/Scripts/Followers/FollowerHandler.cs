using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class FollowerHandler : MonoBehaviour, IFollower
{
    private enum FollowerState { Idle, Guarding, Following, Attacking, Dying }

    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Image hoverIcon;
    [SerializeField] private AnimationComponent animationn;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Data")]
    [SerializeField, ReadOnly] private UnitData unitData;

    [Header("Debug")]
    [SerializeField, ReadOnly] private UnitData leader;
    [SerializeField, ReadOnly] private UnitData target;
    [SerializeField, ReadOnly] private FollowerState state;
    [SerializeField, ReadOnly] private float attackTimer;

    public UnitData UnitData { get { return unitData; } }

    public void Initialize(UnitData unitData)
    {
        this.unitData = unitData;

        state = FollowerState.Guarding;

        gameObject.name = unitData.ToString();
    }

    public void Follow(UnitData leader)
    {
        if (leader != null) // Start following leader
        {
            unitData.roomData = null;
        }
        else if (this.leader != null) // Null means assign yourself to current room
        {
            unitData.roomData = this.leader.roomData;
        }

        this.leader = leader;
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
        GameEvents.instance.OnEnterFollower += EventEnterFollow;
        GameEvents.instance.OnExitFollower += EventExitFollow;
        GameEvents.instance.OnUnitTakeDamage += EventTakeDamage;
        GameEvents.instance.OnUnitDie += EventDie;
    }

    private void OnDestroy()
    {
        GameEvents.instance.OnEnterFollower -= EventEnterFollow;
        GameEvents.instance.OnExitFollower -= EventExitFollow;
        GameEvents.instance.OnUnitTakeDamage -= EventTakeDamage;
        GameEvents.instance.OnUnitDie -= EventDie;
    }

    private void Update()
    {
        switch (state)
        {
            case FollowerState.Guarding:

                FollowLeader();

                bool found = SearchForTarget();
                if (found)
                {
                    attackTimer = unitData.attackSpeed;
                    state = FollowerState.Attacking;
                }

                break;
            case FollowerState.Following:

                // TODO?

                break;
            case FollowerState.Attacking:

                AttackTarget();
                if (target == null || target.IsDead)
                {
                    target = null;
                    state = FollowerState.Guarding;
                }

                break;
            case FollowerState.Dying:
                // Do nothing...
                break;
            default:
                print("Unimplemented state.");
                break;
        }
    }

    #region Helpers

    private void FollowLeader()
    {
        if (leader != null)
            agent.SetDestination(leader.transform.position);
        else // Go to center of room
            agent.SetDestination(unitData.roomData.worldPosition);

        animationn.Movement(agent.velocity);
    }

    private bool SearchForTarget()
    {
        var hit = Physics2D.OverlapCircle(transform.position, unitData.aggroRange, enemyLayer);
        if (hit && hit.gameObject.TryGetComponent(out EnemyHandler enemyHandler) && !enemyHandler.UnitData.IsDead)
        {
            target = enemyHandler.UnitData;
            return true;
        }

        return false;
    }

    private void AttackTarget()
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
                print("attack!");

                GameManager.instance.AttackUnit(unitData, target);

                animationn.Attack();

                attackTimer = unitData.attackSpeed;
            }
            else
            {
                attackTimer -= Time.deltaTime;
            }
        }
    }

    #endregion

    #region Events

    private void EventEnterFollow(IFollower follower)
    {
        if (follower.Equals(this))
            hoverIcon.enabled = true;
        else
            hoverIcon.enabled = false;
    }

    private void EventExitFollow(IFollower follower)
    {
        if (follower.Equals(this))
            hoverIcon.enabled = false;
    }

    public void EventTakeDamage(UnitData unitData)
    {
        // TODO
    }

    public void EventDie(UnitData unitData)
    {
        // TODO
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        if (unitData == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, unitData.aggroRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, unitData.attackRange);
    }
}
