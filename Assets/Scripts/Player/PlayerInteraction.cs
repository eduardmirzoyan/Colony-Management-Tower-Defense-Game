using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider2D collider2d;

    private IFollower follower;
    private List<IFollower> heldFollowers;

    private ISpawner spawner;

    private IBarrier barrier;

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

            print("Found spawner");
        }

        if (other.gameObject.TryGetComponent(out IBarrier barrier))
        {
            this.barrier = barrier;

            print("Found barrier");
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
            this.barrier = null;
        }
    }

    public void HandleInteractions()
    {
        // If you are currently able to interact, then recruit them
        if (follower != null)
        {
            follower.Follow(transform);
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

        spawner?.Spawn();

        barrier?.Raise();
    }
}
