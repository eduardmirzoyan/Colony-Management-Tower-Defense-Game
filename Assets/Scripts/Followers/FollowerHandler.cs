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
        intentRenderer.sprite = null;

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
                    agent.isStopped = true;
                    state = FollowerState.Attacking;
                }
                else if (target.transform == null)
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

                    agent.isStopped = false;
                    if (target.transform == null)
                    {
                        target = null;
                        state = FollowerState.Guarding;
                    }
                    else
                        state = FollowerState.Aggravated;
                }

                break;
            default:
                print("Unimplemented state.");
                break;
        }
    }

    #region Helpers

    private bool SearchForTarget()
    {
        var hit = Physics2D.OverlapCircle(transform.position, unitData.aggroRange, enemyLayer);
        if (hit && hit.gameObject.TryGetComponent(out EnemyHandler enemyHandler))
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
            agent.isStopped = false;
            agent.SetDestination(target.transform.position);
            animationn.Movement(agent.velocity);
        }
        else
        {
            agent.isStopped = true;
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
        Gizmos.DrawWireSphere(target.transform.position, 0.25f);
    }
}
