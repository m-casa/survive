using Animancer;
using ECM2.Characters;
using SensorToolkit;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClassicCharacter : Character
{
    #region EDITOR EXPOSED FIELDS

    [Header("Speed Multipliers")]
    [Tooltip("The speed multiplier while the Character is walking forward.")]
    [SerializeField]
    private float _forwardSpeedMultiplier;

    [Tooltip("The speed multiplier while the Character is walking backward.")]
    [SerializeField]
    private float _backwardSpeedMultiplier;

    [Tooltip("The speed multiplier while the Character is walking sideways.")]
    [SerializeField]
    private float _strafeSpeedMultiplier;

    [Tooltip("The speed at which the Character is quick turning.")]
    [SerializeField]
    private float _quickTurnSpeed;

    [Header("Animation Objects")]
    [Tooltip("The animancer component.")]
    [SerializeField]
    private NamedAnimancerComponent animancer;

    [Header("Sensors")]
    [Tooltip("The Trigger Sensor.")]
    [SerializeField]
    private TriggerSensor triggerSensor;

    #endregion

    #region FIELDS

    protected bool _interactButtonPressed;

    protected Quaternion _quickTurnRotation;
    protected bool _isQuickTurning;

    #endregion

    #region PROPERTIES

    /// <summary>
    /// The speed multiplier while the Character is walking forward.
    /// </summary>

    public float forwardSpeedMultiplier
    {
        get => _forwardSpeedMultiplier;
        set => _forwardSpeedMultiplier = Mathf.Max(0.0f, value);
    }

    /// <summary>
    /// The speed multiplier while the Character is walking backwards.
    /// </summary>

    public float backwardSpeedMultiplier
    {
        get => _backwardSpeedMultiplier;
        set => _backwardSpeedMultiplier = Mathf.Max(0.0f, value);
    }

    /// <summary>
    /// The speed multiplier while the Character is strafing.
    /// </summary>

    public float strafeSpeedMultiplier
    {
        get => _strafeSpeedMultiplier;
        set => _strafeSpeedMultiplier = Mathf.Max(0.0f, value);
    }

    /// <summary>
    /// The speed at which the Character is quick turning.
    /// </summary>

    public float quickTurnSpeed
    {
        get => _quickTurnSpeed;
        set => _quickTurnSpeed = Mathf.Max(0.0f, value);
    }

    #endregion

    #region INPUT ACTIONS

    /// <summary>
    /// Interact InputAction.
    /// </summary>

    protected InputAction interactInputAction { get; set; }

    /// <summary>
    /// Quick turn InputAction.
    /// </summary>

    protected InputAction quickTurnInputAction { get; set; }

    #endregion

    #region INPUT ACTION HANDLERS

    /// <summary>
    /// Interact input action handler.
    /// </summary>

    protected virtual void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
            Interact();
        else if (context.canceled)
            StopInteracting();
    }

    /// <summary>
    /// Quick turn input action handler.
    /// </summary>

    protected virtual void OnQuickTurn(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
            QuickTurn();
    }

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

        // Setup Interact input action handlers
        interactInputAction = actions.FindAction("Interact");
        if (sprintInputAction != null)
        {
            interactInputAction.started += OnInteract;
            interactInputAction.performed += OnInteract;
            interactInputAction.canceled += OnInteract;
        }

        // Setup Quick Turn input action handlers
        quickTurnInputAction = actions.FindAction("Quick Turn");
        if (quickTurnInputAction != null)
        {
            quickTurnInputAction.started += OnQuickTurn;
            quickTurnInputAction.performed += OnQuickTurn;
        }
    }

    /// <summary>
    /// Overrides HandleInput method.
    /// Rotate the Character and adjust their speed accordingly.
    /// </summary>

    protected override void HandleInput()
    {
        if (actions == null)
            return;

        // Poll Movement InputAction
        Vector2 movementInput = GetMovementInput();

        // Add movement input relative to the Character
        Vector3 movementDirection = Vector3.zero;
        Vector3 rotationDirection = Vector3.zero;

        movementDirection += Vector3.Normalize(GetForwardVector() * movementInput.y);
        rotationDirection += Vector3.Normalize(Vector3.right * movementInput.x);

        SetMovementDirection(movementDirection);
        SetRotationRate(movementInput);

        if (_sprintButtonPressed)
        {
            UpdateSprintState(movementInput);
        }

        // Perform quick turn rotation
        if (IsQuickTurning())
        {
            // Don't allow the player to move while quick turning
            SetMovementDirection(Vector3.zero);

            Quaternion quickTurnRotation = GetQuickTurnRotation();

            characterMovement.rotation = Quaternion.RotateTowards(
                characterMovement.rotation, quickTurnRotation, quickTurnSpeed * Time.deltaTime);

            // Stop turning after reaching close to 180 degrees
            if (Quaternion.Angle(characterMovement.rotation, quickTurnRotation) < 0.1f)
            {
                StopQuickTurning();
            }
        }

        // Perform regular turn rotation
        else if (rotationDirection.x != 0f)
        {
            AddYawInput(rotationDirection.x * rotationRate * Time.deltaTime);
        }
    }

    /// <summary>
    /// Overrides Animate.
    /// Update the animation state of the Character.
    /// </summary>

    protected override void Animate()
    {
        if (IsQuickTurning())
        {
            PlayQuickTurnAnimation();
        }
        else
        {
            PlayMovementAnimation();
        }
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
        backwardSpeedMultiplier = 0.75f;
        strafeSpeedMultiplier = 1.0f;
        quickTurnSpeed = 380f;

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
        quickTurnSpeed = _quickTurnSpeed;
    }

    /// <summary>
    /// Called when the object becomes enabled and active (OnEnabled).
    /// If overriden, must call base method in order to fully initialize the class.
    /// </summary>

    protected override void OnOnEnable()
    {
        // Init Character
        base.OnOnEnable();

        // Enable input actions (if any)
        interactInputAction?.Enable();
        quickTurnInputAction?.Enable();
    }

    /// <summary>
    /// Called when the behaviour becomes disabled (OnDisable).
    /// If overriden, must call base method in order to fully de-initialize the class.
    /// </summary>

    protected override void OnOnDisable()
    {
        // De-Init Character
        base.OnOnDisable();

        // Disable input actions (if any)
        interactInputAction?.Disable();
        quickTurnInputAction?.Disable();
    }

    /// <summary>
    /// Overrides GetMaxSpeed.
    /// The maximum speed for current movement mode, factoring walking movement direction.
    /// </summary>

    public override float GetMaxSpeed()
    {
        return base.GetMaxSpeed() * GetSpeedMultiplier();
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
    /// Return the Character's quick turn rotation.
    /// </summary>

    protected virtual Quaternion GetQuickTurnRotation()
    {
        return _quickTurnRotation;
    }

    /// <summary>
    /// Returns true if Character is interacting.
    /// </summary>

    public virtual bool IsInteracting()
    {
        return _interactButtonPressed;
    }

    /// <summary>
    /// Request the Character to start interacting.
    /// </summary>

    public virtual void Interact()
    {
        _interactButtonPressed = true;

        if (triggerSensor.DetectedObjects.Count != 0)
        {
            GameObject nearestGameObject = triggerSensor.DetectedObjectsOrderedByDistance[0];
            IInteractable<GameObject> interactable = nearestGameObject.GetComponent<IInteractable<GameObject>>();
            
            if (interactable != null)
                interactable.Interact(gameObject);
        }  
    }

    /// <summary>
    /// Request the Character to stop interacting.
    /// </summary>

    public virtual void StopInteracting()
    {
        _interactButtonPressed = false;
    }

    /// <summary>
    /// Returns true if Character is quick turning.
    /// </summary>

    public virtual bool IsQuickTurning()
    {
        return _isQuickTurning;
    }

    /// <summary>
    /// Request the Character to start quick turning.
    /// </summary>

    public virtual void QuickTurn()
    {
        if (!IsQuickTurning())
        {
            SetQuickTurnRotation();

            _isQuickTurning = true;
        }
    }

    /// <summary>
    /// Request the Character to stop quick turning.
    /// </summary>

    public virtual void StopQuickTurning()
    {
        _isQuickTurning = false;
    }

    /// <summary>
    /// Updates the Character's animation to quick turn.
    /// </summary>

    private void PlayQuickTurnAnimation()
    {
        animancer.TryPlay("Quick Turn", 0.25f);
    }

    /// <summary>
    /// Updates the Character's movement animation.
    /// </summary>

    private void PlayMovementAnimation()
    {
        Vector2 movementInput = GetMovementInput();

        // We're not moving
        if (movementInput == Vector2.zero)
        {
            animancer.TryPlay("Idle", 0.25f);
        }

        // Moving forward
        else if (movementInput.y > 0f ) //&& (movementInput.x > -0.5f && movementInput.x < 0.5f))
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
        else if (movementInput.y < 0f) //&& (movementInput.x > -0.5f && movementInput.x < 0.5f))
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
    /// Only allow the Character to sprint forward
    /// </summary>

    private void UpdateSprintState(Vector2 movementInput)
    {
        if (movementInput.y > 0.0f)
        {
            Sprint();
        }
        else if (movementInput.y <= 0.0f)
        {
            StopSprinting();
        }
    }

    /// <summary>
    /// Set the Character's rotation rate based on the their input.
    /// </summary>

    private void SetRotationRate(Vector2 movementInput)
    {
        if (movementInput.y != 0.0f)
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
    /// Set the Character's quick turn rotation.
    /// </summary>

    private void SetQuickTurnRotation()
    {
        // The quick turn will always be 180 degrees from the current rotation
        _quickTurnRotation = characterMovement.rotation * Quaternion.AngleAxis(180f, Vector3.up);
    }

    #endregion
}
