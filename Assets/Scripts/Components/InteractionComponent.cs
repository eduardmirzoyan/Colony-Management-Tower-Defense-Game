using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InteractionComponent : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool autoRecruit;

    private IFollower follower;
    private List<IStructure> structures;

    private List<IFollower> heldFollowers;

    private UnitData unitData;

    private void Awake()
    {
        heldFollowers = new List<IFollower>();
        structures = new List<IStructure>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out RoomHandler roomHandler))
        {
            roomHandler.Enter();
        }

        if (other.gameObject.TryGetComponent(out IStructure structure))
        {
            structures.Add(structure);
            GameEvents.instance.TriggerOnEnterStructure(structure);

            // Ditch follower if possible
            if (this.follower != null)
            {
                GameEvents.instance.TriggerOnExitFollower(this.follower);
                this.follower = null;
            }
        }

        if (other.gameObject.TryGetComponent(out IFollower follower)) // Skip if in range of struct
        {
            // Immediately recruit if auto
            if (autoRecruit && !heldFollowers.Contains(follower))
            {
                follower.Follow(unitData);
                heldFollowers.Add(follower);
                return;
            }

            if (structures.Count == 0)
            {
                this.follower = follower;
                GameEvents.instance.TriggerOnEnterFollower(follower);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out IStructure structure) && structures.Contains(structure))
        {
            structures.Remove(structure);
            GameEvents.instance.TriggerOnExitStructure(structure);
        }

        if (other.gameObject.TryGetComponent(out IFollower follower) && this.follower == follower)
        {
            this.follower = null;
            GameEvents.instance.TriggerOnExitFollower(follower);
        }
    }

    public void HandleInteractions(UnitData unitData)
    {
        // First check for any structures
        if (structures.Count > 0)
        {
            // Interact with most recent structure
            structures[^1].Use(unitData);
            return;
        }

        // If you are currently able to interact, then recruit them
        if (follower != null)
        {
            // If follower is already following you, release him
            if (heldFollowers.Contains(follower))
            {
                follower.Follow(null);
                heldFollowers.Remove(follower);
            }
            else // Recruit him 
            {
                follower.Follow(unitData);
                heldFollowers.Add(follower);
            }
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
    }

    public void SetAutoRecruit(UnitData unitData)
    {
        this.unitData = unitData;
        autoRecruit = unitData != null;
    }
}
