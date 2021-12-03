using ECM2.Characters;
using UnityEngine;

public class ClassicCharacter : Character
{
    #region EDITOR EXPOSED FIELDS

    #endregion

    #region FIELDS

    #endregion

    #region PROPERTIES

    #endregion

    #region METHODS

    /// <summary>
    /// Overrides SetupPlayerInput method.
    /// Sets up InputActions for tank controls.
    /// </summary>

    protected override void SetupPlayerInput()
    {
        // Check if there are any InputActions cached

        if (actions == null)
            return;

        // Movement input action (This is polled)

        movementInputAction = actions.FindAction("Movement");

        // Setup Sprint input action handlers

        sprintInputAction = actions.FindAction("Sprint");
        if (sprintInputAction != null)
        {
            sprintInputAction.started += OnSprint;
            sprintInputAction.performed += OnSprint;
            sprintInputAction.canceled += OnSprint;
        }
    }

    /// <summary>
    /// Overrides HandleInput method.
    /// Rotate the player and adjust their speed accordingly.
    /// </summary>

    protected override void HandleInput()
    {
        if (actions == null)
            return;

        // Poll Movement InputAction

        Vector2 movementInput = GetMovementInput();

        // Add movement input relative to the player

        Vector3 movementDirection = Vector3.zero;
        Vector3 rotationDirection = Vector3.zero;

        movementDirection += GetForwardVector() * movementInput.y;
        rotationDirection += Vector3.Normalize(Vector3.right * movementInput.x);

        SetMovementDirection(movementDirection);
        SetRotationRate(movementInput);

        if (_sprintButtonPressed)
        {
            ChangeSprintState(movementInput);
        }

        // Perform rotation

        if (rotationDirection.x != 0f)
        {
            AddYawInput(rotationDirection.x * rotationRate * Time.deltaTime);
        }
    }

    /// <summary>
    /// Only allow the player to sprint forward
    /// </summary>

    private void ChangeSprintState(Vector2 movementInput)
    {
        if (movementInput.y > 0f)
        {
            Sprint();
        }
        else if (movementInput.y <= 0f)
        {
            StopSprinting();
        }
    }

    /// <summary>
    /// Set the player's rotation rate based on the their input.
    /// </summary>

    private void SetRotationRate(Vector2 movementInput)
    {
        if (movementInput.y != 0f && IsSprinting())
        {
            rotationRate = 165f;
        }
        else if (movementInput.y != 0f)
        {
            rotationRate = 95f;
        }
        else
        {
            rotationRate = 130f;
        }
    }

    #endregion
}
