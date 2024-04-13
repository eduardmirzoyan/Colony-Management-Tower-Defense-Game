using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class FollowerHandler : MonoBehaviour, IFollower
{
    private enum FollowerState { Idle, Guarding, Following, Aggravated, Attacking, Dying }
    private enum Intent { Select, Follow, Defend, Attack }

    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private SpriteRenderer intentIcon;
    [SerializeField] private AnimationComponent animationn;
    [SerializeField] private DamageFlash damageFlash;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Sprite[] intentSprites;

    [Header("Data")]
    [SerializeField, ReadOnly] private UnitData unitData;
    [SerializeField] private float intentDuration;

    [Header("Debug")]
    [SerializeField, ReadOnly] private UnitData leader;
    [SerializeField, ReadOnly] private UnitData target;
    [SerializeField, ReadOnly] private FollowerState state;
    [SerializeField, ReadOnly] private float attackTimer;

    public UnitData UnitData { get { return unitData; } }

    public void Initialize(UnitData unitData)
    {
        this.unitData = unitData;

        StartCoroutine(ShowIntent(Intent.Defend, intentDuration));
        state = FollowerState.Guarding;
        intentIcon.sprite = null;

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
        intentIcon.sprite = null;
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

                if (leader != null)
                {
                    StopAllCoroutines();
                    StartCoroutine(ShowIntent(Intent.Follow, intentDuration));

                    state = FollowerState.Following;
                    return;
                }

                bool found = SearchForTarget();
                if (found)
                {
                    StopAllCoroutines();
                    StartCoroutine(ShowIntent(Intent.Attack, intentDuration));

                    attackTimer = unitData.attackSpeed;
                    state = FollowerState.Aggravated;
                    return;
                }

                agent.SetDestination(unitData.roomData.worldPosition);
                animationn.Movement(agent.velocity);

                break;
            case FollowerState.Following:

                if (leader == null)
                {
                    StopAllCoroutines();
                    StartCoroutine(ShowIntent(Intent.Defend, intentDuration));

                    state = FollowerState.Guarding;
                    return;
                }

                agent.SetDestination(leader.transform.position);
                animationn.Movement(agent.velocity);

                break;
            case FollowerState.Aggravated:

                bool attacked = ChaseAndAttackTarget();

                if (attacked)
                {
                    state = FollowerState.Attacking;
                }
                else if (target == null || target.IsDead)
                {
                    target = null;
                    state = FollowerState.Guarding;
                }

                break;
            case FollowerState.Attacking:

                // Wait until animation is over
                if (animationn.CurrentAnimationOver())
                {
                    attackTimer = unitData.attackSpeed;
                    state = target.IsDead ? FollowerState.Guarding : FollowerState.Aggravated;
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

    private IEnumerator ShowIntent(Intent intent, float duration)
    {
        intentIcon.sprite = intentSprites[(int)intent];
        intentIcon.color = Color.white;

        // Hold
        yield return new WaitForSeconds(duration);

        // Fade out
        float elapsed = 0;
        while (elapsed < 0.5f)
        {
            // Lerp color
            intentIcon.color = Color.Lerp(Color.white, Color.clear, elapsed / duration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        intentIcon.sprite = null;
        intentIcon.color = Color.white;
    }

    #endregion

    #region Events

    private void EventEnterFollow(IFollower follower)
    {
        if (follower.Equals(this))
            intentIcon.sprite = intentSprites[(int)Intent.Select];
        else
            intentIcon.sprite = null;
    }

    private void EventExitFollow(IFollower follower)
    {
        if (follower.Equals(this))
            intentIcon.sprite = null;
    }

    public void EventTakeDamage(UnitData unitData)
    {
        if (this.unitData != unitData) return;

        damageFlash.Flash();
    }

    public void EventDie(UnitData unitData)
    {
        if (this.unitData != unitData) return;

        agent.isStopped = true;
        animationn.Die();

        Destroy(gameObject, 2f);
        state = FollowerState.Dying;
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
