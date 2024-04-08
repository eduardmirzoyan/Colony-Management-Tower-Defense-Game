using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement), typeof(AnimationHandler))]
public class PlayerHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Movement movement;
    [SerializeField] private AnimationHandler animationHandler;
    [SerializeField] private PlayerInteraction interaction;

    [Header("Settings")]
    [SerializeField] private KeyCode interactKey;

    private void Update()
    {
        HandleMovementInput();
        HandleInteractInput();
        HanldeAnimation();
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
            interaction.HandleInteractions();
        }
    }

    private void HanldeAnimation()
    {
        animationHandler.HandleAnimation(movement.Velocity);
    }
}
