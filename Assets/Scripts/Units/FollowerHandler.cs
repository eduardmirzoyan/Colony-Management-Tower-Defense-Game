using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class FollowerHandler : MonoBehaviour, IFollower
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Image hoverIcon;
    [SerializeField] private AnimationComponent animationHandler;

    [Header("Data")]
    [SerializeField, ReadOnly] private UnitData unitData;

    [Header("Debug")]
    [SerializeField] private UnitData leader;
    [SerializeField] private bool printLogs;

    public void Initialize(UnitData unitData)
    {
        this.unitData = unitData;

        gameObject.name = unitData.ToString();
    }

    public void Follow(UnitData leader)
    {
        if (leader != null) // Start following leader
        {
            unitData.roomData = null;

            if (printLogs) print($"{name} is now following {leader}");
        }
        else if (this.leader != null) // Null means assign yourself to current room
        {
            unitData.roomData = this.leader.roomData;

            if (printLogs) print($"{name} assign to {this.leader.roomData}");
        }
        else if (printLogs) print($"{name} is lost.");

        this.leader = leader;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.avoidancePriority = Random.Range(0, 1000);

        hoverIcon.enabled = false;
    }

    private void Start()
    {
        GameEvents.instance.OnEnterFollower += ShowIndicator;
        GameEvents.instance.OnExitFollower += HideIndicator;
    }

    private void OnDestroy()
    {
        GameEvents.instance.OnEnterFollower -= ShowIndicator;
        GameEvents.instance.OnExitFollower -= HideIndicator;
    }

    private void Update()
    {
        if (leader != null) // Follow leader
            agent.SetDestination(leader.transform.position);
        else // Go to center of room
            agent.SetDestination(unitData.roomData.worldPosition);

        animationHandler.HandleAnimation(agent.velocity);
    }

    private void ShowIndicator(IFollower follower)
    {
        if (follower.Equals(this))
            hoverIcon.enabled = true;
        else
            hoverIcon.enabled = false;
    }

    private void HideIndicator(IFollower follower)
    {
        if (follower.Equals(this))
            hoverIcon.enabled = false;
    }
}
