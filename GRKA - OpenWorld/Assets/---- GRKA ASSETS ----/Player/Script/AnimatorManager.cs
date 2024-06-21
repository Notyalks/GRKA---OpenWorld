    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class AnimatorManager : MonoBehaviour
{
    public Animator animator;

    private int movementState;

    private bool isWalkingKeyPressed;
    private bool isSprintingKeyPressed;
    private bool isWalking;
    private bool isSprinting;
    private float firstKeyPressTime;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        movementState = Animator.StringToHash("MovementState");
    }

    public void PlayTargetAnimation(string targetAnimation, bool isInteracting)
    {
        animator.SetBool("isInteracting", isInteracting);
        animator.CrossFade(targetAnimation, 0.2f);
    }

    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprintingInput, bool isWalkingInput)
    {
        // Detect key press states
        bool walkingKeyPressed = isWalkingInput;
        bool sprintingKeyPressed = isSprintingInput;

        // Handle key press priority
        if (walkingKeyPressed && !isWalkingKeyPressed)
        {
            isWalkingKeyPressed = true;
            isWalking = true;
            isSprinting = false;
            firstKeyPressTime = Time.time;
        }
        else if (sprintingKeyPressed && !isSprintingKeyPressed)
        {
            isSprintingKeyPressed = true;
            isSprinting = true;
            isWalking = false;
            firstKeyPressTime = Time.time;
        }

        // Update the key release states
        if (!walkingKeyPressed)
        {
            isWalkingKeyPressed = false;
        }
        if (!sprintingKeyPressed)
        {
            isSprintingKeyPressed = false;
        }

        // Set movement state
        int currentMovementState;
        if (isSprinting && !isWalking)
        {
            currentMovementState = 3; // Fast Run
        }
        else if (isWalking && !isSprinting)
        {
            currentMovementState = 1; // Walk
        }
        else if (Mathf.Abs(horizontalMovement) > 0.1f || Mathf.Abs(verticalMovement) > 0.1f)
        {
            currentMovementState = 2; // Slow Run
        }
        else
        {
            currentMovementState = 0; // Idle
        }

        // Reset the priority when both keys are released
        if (!isWalkingKeyPressed && !isSprintingKeyPressed)
        {
            isWalking = false;
            isSprinting = false;
        }

        animator.SetInteger(movementState, currentMovementState);
    }
}