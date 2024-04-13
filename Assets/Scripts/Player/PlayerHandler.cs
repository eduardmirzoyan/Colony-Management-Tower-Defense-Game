using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementComponent), typeof(AnimationComponent), typeof(InteractionComponent))]
public class PlayerHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MovementComponent movement;
    [SerializeField] private AnimationComponent animationn;
    [SerializeField] private InteractionComponent interaction;

    [Header("Data")]
    [SerializeField, ReadOnly] private UnitData unitData;

    [Header("Settings")]
    [SerializeField] private KeyCode interactKey;

    public UnitData UnitData { get { return unitData; } }

    public void Initialize(UnitData unitData)
    {
        this.unitData = unitData;
    }

    private void Update()
    {
        HandleMovementInput();
        HandleInteractInput();
        HandleAnimation();
    }

    private void HandleMovementInput()
    {
        // Get input from player
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Normalize
        Vector2 direction = new Vector2(moveX, moveY).normalized;

        movement.Move(direction);
    }

    private void HandleInteractInput()
    {
        if (Input.GetKeyDown(interactKey))
        {
            interaction.HandleInteractions(unitData);
        }
    }

    private void HandleAnimation()
    {
        animationn.Movement(movement.Velocity);
    }
}
