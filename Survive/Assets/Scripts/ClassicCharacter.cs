using Animancer;
using ECM2.Characters;
using UnityEngine;

public class ClassicCharacter : Character
{
    #region EDITOR EXPOSED FIELDS

    [Header("Speed Multipliers")]
    [Tooltip("The speed multiplier while Character is walking forward.")]
    [SerializeField]
    private float _forwardSpeedMultiplier;

    [Tooltip("The speed multiplier while Character is walking backward.")]
    [SerializeField]
    private float _backwardSpeedMultiplier;

    [Tooltip("The speed multiplier while Character is walking sideways.")]
    [SerializeField]
    private float _strafeSpeedMultiplier;

    [Header("Animation Objects")]
    [Tooltip("The animancer component.")]
    [SerializeField]
    private AnimancerComponent animancer;

    #endregion

    #region FIELDS

    #endregion

    #region PROPERTIES

    /// <summary>
    /// The speed multiplier while Character is walking forward.
    /// </summary>

    public float forwardSpeedMultiplier
    {
        get => _forwardSpeedMultiplier;
        set => _forwardSpeedMultiplier = Mathf.Max(0.0f, value);
    }

    /// <summary>
    /// The speed multiplier while Character is walking backwards.
    /// </summary>

    public float backwardSpeedMultiplier
    {
        get => _backwardSpeedMultiplier;
        set => _backwardSpeedMultiplier = Mathf.Max(0.0f, value);
    }

    /// <summary>
    /// The speed multiplier while Character is strafing.
    /// </summary>

    public float strafeSpeedMultiplier
    {
        get => _strafeSpeedMultiplier;
        set => _strafeSpeedMultiplier = Mathf.Max(0.0f, value);
    }

    #endregion

    #region METHODS

    /// <summary>
    /// Overrides GetMaxSpeed.
    /// The maximum speed for current movement mode, factoring walking movement direction.
    /// </summary>

    public override float GetMaxSpeed()
    {
        return base.GetMaxSpeed() * GetSpeedMultiplier();
    }

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

        movementDirection += Vector3.Normalize(GetForwardVector() * movementInput.y);
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
    /// Overrides Animate.
    /// Update the animation state of the character.
    /// </summary>

    protected override void Animate()
    {
        ChangeMovementAnimation();
    }

    /// <summary>
    /// Overrides OnReset.
    /// Resets speed multipliers.
    /// </summary>

    protected override void OnReset()
    {
        // Character defaults

        base.OnReset();

        // Speed multiplier defaults

        forwardSpeedMultiplier = 1.0f;
        backwardSpeedMultiplier = 0.5f;
        strafeSpeedMultiplier = 0.75f;

        SetRotationMode(RotationMode.None);
    }

    /// <summary>
    /// Overrides OnOnValidate.
    /// Validates speed multipliers.
    /// </summary>

    protected override void OnOnValidate()
    {
        // Validates Character fields

        base.OnOnValidate();

        // Validate speed multipliers

        forwardSpeedMultiplier = _forwardSpeedMultiplier;
        backwardSpeedMultiplier = _backwardSpeedMultiplier;
        strafeSpeedMultiplier = _strafeSpeedMultiplier;
    }

    /// <summary>
    /// The current speed multiplier based on movement direction,
    /// eg: walking forward, walking backwards or strafing side to side.
    /// </summary>

    protected virtual float GetSpeedMultiplier()
    {
        // Compute planar move direction

        Vector3 up = GetUpVector();
        Vector3 planarMoveDirection = Vector3.ProjectOnPlane(GetMovementDirection(), up);

        // Compute actual walk speed factoring movement direction

        Vector3 characterForward = Vector3.ProjectOnPlane(GetForwardVector(), up);

        float dot = Vector3.Dot(planarMoveDirection.normalized, characterForward.normalized);

        float speedMultiplier = dot >= 0.0f
            ? Mathf.Lerp(strafeSpeedMultiplier, forwardSpeedMultiplier, dot)
            : Mathf.Lerp(strafeSpeedMultiplier, backwardSpeedMultiplier, -dot);

        return speedMultiplier;
    }

    /// <summary>
    /// Updates the character's movement animation.
    /// </summary>

    private void ChangeMovementAnimation()
    {
        Vector2 movementInput = GetMovementInput();

        if (movementInput != Vector2.zero)
        {
            // We're moving by direction
            ChangeDirectionalAnimation(movementInput);
        }
        else
        {
            // We're not moving, so use an idle animation
            ChangeIdleAnimation();
        }
    }

    /// <summary>
    /// Updates the character's directional animation.
    /// </summary>

    private void ChangeDirectionalAnimation(Vector2 movementInput)
    {
        // Moving forward
        if (movementInput.y > 0f )//&& (movementInput.x > -0.5f && movementInput.x < 0.5f))
        {
            if (IsSprinting())
            {
                animancer.TryPlay("Run", 0.25f);
            }
            else
            {
                animancer.TryPlay("Walk", 0.25f);
            }
        }

        // Moving backwards
        else if (movementInput.y < 0f)//&& (movementInput.x > -0.5f && movementInput.x < 0.5f))
        {
            animancer.TryPlay("Walk Backwards", 0.25f);
        }

        // Turning left
        else if (movementInput.x < 0)
        {
            animancer.TryPlay("Turn Left", 0.25f);
        }

        // Turning right
        else if (movementInput.x > 0f)
        {
            animancer.TryPlay("Turn Right", 0.25f);
        }
    }

    /// <summary>
    /// Updates the character's idle animation.
    /// </summary>

    private void ChangeIdleAnimation()
    {
        animancer.TryPlay("Idle", 0.25f);
    }

    /// <summary>
    /// Set the player's rotation rate based on the their input.
    /// </summary>

    private void SetRotationRate(Vector2 movementInput)
    {
        if (movementInput.y != 0f)
        {
            if (IsSprinting())
            {
                rotationRate = 180f;
            }
            else
            {
                rotationRate = 90f;
            }
        }
        else
        {
            rotationRate = 130f;
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

    #endregion
}
