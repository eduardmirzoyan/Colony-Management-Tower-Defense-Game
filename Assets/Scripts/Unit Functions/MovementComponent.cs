using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementComponent : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rigidbody2d;

    [Header("Settings")]
    [SerializeField] private MovementSettings settings;

    [Header("Debug")]
    [SerializeField, ReadOnly] private Vector2 velocity;

    public Vector2 Velocity { get { return velocity; } }

    public void Move(Vector2 direction)
    {
        if (direction.magnitude > 1f) throw new System.Exception($"Direction magnitude too larger: {direction.magnitude}");

        if (direction == Vector2.zero)
            velocity = Vector2.MoveTowards(velocity, Vector2.zero, settings.deceleration * Time.deltaTime);
        else
            velocity = Vector2.MoveTowards(velocity, direction * settings.maxSpeed, settings.acceleration * Time.deltaTime);

        rigidbody2d.velocity = velocity;
    }
}
