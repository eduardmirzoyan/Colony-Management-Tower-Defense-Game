using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class FollowerHandler : MonoBehaviour, IFollower
{
    private enum FollowerState { Idle, Guarding, Following, Aggravated, Attacking }
    private enum Intent { Select, Follow, Defend, Attack }

    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private SpriteRenderer intentRenderer;
    [SerializeField] private SpriteRenderer outlineRenderer;
    [SerializeField] private AnimationComponent animationn;
    [SerializeField] private DamageFlash damageFlash;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Sprite[] intentSprites;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Data")]
    [SerializeField, ReadOnly] private UnitData unitData;
    [SerializeField] private float intentDuration;
    [SerializeField] private float attackTimestamp;
    [SerializeField] private bool isRanged;

    [Header("Debug")]
    [SerializeField, ReadOnly] private UnitData leader;
    [SerializeField, ReadOnly] private UnitData target;
    [SerializeField, ReadOnly] private FollowerState state;
    [SerializeField, ReadOnly] private float attackTimer;
    [SerializeField, ReadOnly] private bool hasAttacked;

    public UnitData UnitData { get { return unitData; } }

    public void Initialize(UnitData unitData)
    {
        this.unitData = unitData;

        StartCoroutine(ShowIntent(Intent.Defend, intentDuration));
        state = FollowerState.Guarding;
        intentRenderer.sprite = null;
        agent.speed = unitData.moveSpeed;

        gameObject.name = unitData.ToString();
    }

    public void Follow(UnitData leader)
    {
        if (leader != null) // Start following leader
        {
            GameLogic.AssignUnitToRoom(unitData, null);
        }
        else if (this.leader != null) // Null means assign yourself to current room
        {
            GameLogic.AssignUnitToRoom(unitData, this.leader.roomData);
        }

        this.leader = leader;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = 0f;
        agent.avoidancePriority = Random.Range(0, 1000);
        intentRenderer.sprite = null;
        outlineRenderer.enabled = false;
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

                var t = SearchForValidTarget(unitData.aggroRange, enemyLayer);
                if (t != null)
                {
                    target = t;

                    StopAllCoroutines();
                    StartCoroutine(ShowIntent(Intent.Attack, intentDuration));
                    lineRenderer.positionCount = 0;

                    attackTimer = unitData.attackSpeed;
                    state = FollowerState.Aggravated;
                    return;
                }

                agent.SetDestination(unitData.roomData.worldPosition);
                animationn.Movement(agent.velocity);
                UpdateMoveLine(transform.position, unitData.roomData.worldPosition, agent.stoppingDistance);

                break;
            case FollowerState.Following:

                if (leader == null)
                {
                    StopAllCoroutines();
                    StartCoroutine(ShowIntent(Intent.Defend, intentDuration));

                    state = FollowerState.Guarding;
                    return;
                }

                var tt = SearchForValidTarget(unitData.aggroRange, enemyLayer);
                if (tt != null)
                {
                    target = tt;

                    StopAllCoroutines();
                    StartCoroutine(ShowIntent(Intent.Attack, intentDuration));
                    lineRenderer.positionCount = 0;

                    attackTimer = unitData.attackSpeed;
                    state = FollowerState.Aggravated;
                    return;
                }

                agent.SetDestination(leader.transform.position);
                animationn.Movement(agent.velocity);
                UpdateMoveLine(transform.position, leader.transform.position, agent.stoppingDistance);

                break;
            case FollowerState.Aggravated:

                // Reduce attack timer 
                if (attackTimer > 0)
                    attackTimer -= Time.deltaTime;

                // Check if target is dead or left room
                if (target.IsDead || TargetOutsideRoom(unitData.roomData, target))
                {
                    target = null;
                    state = FollowerState.Guarding;
                    return;
                }

                // Check if started an attack
                bool canAttack = ChaseAndAttackTarget();
                if (canAttack)
                {
                    attackTimer = unitData.attackSpeed;
                    animationn.Attack();
                    hasAttacked = false;
                    agent.isStopped = true;
                    state = FollowerState.Attacking;
                    return;
                }

                break;
            case FollowerState.Attacking:

                // Reduce attack timer 
                if (attackTimer > 0)
                    attackTimer -= Time.deltaTime;

                // Attack part-way in animation
                float ratio = animationn.CurrentAnimationRatio();
                if (ratio >= attackTimestamp && !hasAttacked && !target.IsDead)
                {
                    if (isRanged)
                        SpawnManager.instance.SpawnProjectile(unitData, target);
                    else
                        GameLogic.AttackUnit(unitData, target);
                    //SpawnManager.instance.SpawnSlash(unitData, target);

                    hasAttacked = true;
                }

                // Wait until animation is over
                if (ratio >= 0.95f)
                {
                    agent.isStopped = false;
                    state = FollowerState.Aggravated;
                    return;
                }

                break;
            default:
                print("Unimplemented state.");
                break;
        }
    }

    #region Helpers

    private UnitData SearchForValidTarget(float range, LayerMask layer)
    {
        var hit = Physics2D.OverlapCircle(transform.position, range, layer);
        if (hit && hit.gameObject.TryGetComponent(out EnemyHandler enemyHandler))
            if (!enemyHandler.UnitData.IsDead && !TargetOutsideRoom(unitData.roomData, enemyHandler.UnitData)) // Make sure enemy is alive and within room
                return enemyHandler.UnitData;

        return null;
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

    private IEnumerator ShowIntent(Intent intent, float duration)
    {
        intentRenderer.sprite = intentSprites[(int)intent];
        intentRenderer.color = Color.white;

        // Hold
        yield return new WaitForSeconds(duration);

        // Fade out
        float elapsed = 0;
        while (elapsed < 0.5f)
        {
            // Lerp color
            intentRenderer.color = Color.Lerp(Color.white, Color.clear, elapsed / duration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        intentRenderer.sprite = null;
        intentRenderer.color = Color.white;
    }

    private void UpdateMoveLine(Vector3 startPosition, Vector3 endPosition, float minRadius)
    {
        lineRenderer.positionCount = 0;

        if (Vector3.Distance(startPosition, endPosition) > minRadius)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startPosition);
            lineRenderer.SetPosition(1, endPosition);
        }
    }

    private bool TargetOutsideRoom(RoomData roomData, UnitData target) => Vector2.Distance(roomData.worldPosition, target.transform.position) > roomData.size / 2f;

    #endregion

    #region Events

    private void EventEnterFollow(IFollower follower)
    {
        if (follower.Equals(this))
            outlineRenderer.enabled = true;
        else
            outlineRenderer.enabled = false;
    }

    private void EventExitFollow(IFollower follower)
    {
        if (follower.Equals(this))
            outlineRenderer.enabled = false;
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

        if (unitData.roomData == null) return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(unitData.roomData.worldPosition, unitData.roomData.size / 2f);

        if (target?.transform == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, target.transform.position);
        Gizmos.DrawWireSphere(target.transform.position, 0.25f);
    }
}
