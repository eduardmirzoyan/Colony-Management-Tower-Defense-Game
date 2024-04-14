using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InteractionComponent : MonoBehaviour
{
    private IFollower follower;
    private ISpawner spawner;
    private IBarrier barrier;

    private List<IFollower> heldFollowers;

    private void Awake()
    {
        heldFollowers = new List<IFollower>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out RoomHandler roomHandler))
        {
            roomHandler.Enter();
        }

        if (other.gameObject.TryGetComponent(out IFollower follower))
        {
            this.follower = follower;

            GameEvents.instance.TriggerOnEnterFollower(follower);
        }

        if (other.gameObject.TryGetComponent(out ISpawner spawner))
        {
            this.spawner = spawner;

            GameEvents.instance.TriggerOnEnterSpawner(spawner);
        }

        if (other.gameObject.TryGetComponent(out IBarrier barrier))
        {
            barrier.HoverEnter();

            this.barrier = barrier;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out IFollower follower) && this.follower == follower)
        {
            this.follower = null;

            GameEvents.instance.TriggerOnExitFollower(follower);
        }

        if (other.gameObject.TryGetComponent(out ISpawner spawner) && this.spawner == spawner)
        {
            this.spawner = null;

            GameEvents.instance.TriggerOnExitSpawner(spawner);
        }

        if (other.gameObject.TryGetComponent(out IBarrier barrier) && this.barrier == barrier)
        {
            barrier.HoverExit();

            this.barrier = null;
        }
    }

    public void HandleInteractions(UnitData unitData)
    {
        // If you are currently able to interact, then recruit them
        if (follower != null)
        {
            follower.Follow(unitData);
            heldFollowers.Add(follower);
        }
        // Else release all recruits
        else
        {
            foreach (var follower in heldFollowers)
            {
                follower.Follow(null);
            }
            heldFollowers.Clear();
        }

        spawner?.Spawn(unitData);

        barrier?.Raise();
    }
}
