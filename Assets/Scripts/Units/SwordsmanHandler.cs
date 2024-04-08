using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class SwordsmanHandler : MonoBehaviour, IFollower
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Image hoverIcon;
    [SerializeField] private AnimationHandler animationHandler;

    [Header("Debug")]
    [SerializeField] private Transform target;
    [SerializeField] private bool printLogs;

    public void Follow(Transform source)
    {
        if (source != null)
        {
            agent.isStopped = false;

            if (printLogs) print($"[{name}] is now following [{source.name}]");
        }
        else
        {
            agent.isStopped = true;

            if (printLogs) print($"[{name}] stopped following [{target.name}]");
        }

        target = source;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.avoidancePriority = Random.Range(0, 100);

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
        if (target != null)
            agent.SetDestination(target.position);

        animationHandler.HandleAnimation(agent.velocity);
    }

    private void ShowIndicator(IFollower follower)
    {
        if (follower.Equals(this))
            hoverIcon.enabled = true;
    }

    private void HideIndicator(IFollower follower)
    {
        if (follower.Equals(this))
            hoverIcon.enabled = false;
    }
}
