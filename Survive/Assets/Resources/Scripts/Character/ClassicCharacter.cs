using Animancer;
using EasyCharacterMovement;
using Micosmo.SensorToolkit;
using Mirror;
using System.Collections;
using System.Collections.Generic;
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

    [Header("Animation")]
    [Tooltip("The animancer component.")]
    [SerializeField]
    private NamedAnimancerComponent animancer;

    [Tooltip("The script for networking animations.")]
    [SerializeField]
    private NetworkAnimations networkAnimations;

    [Header("Sensors")]
    [Tooltip("The Trigger Sensor.")]
    [SerializeField]
    private TriggerSensor triggerSensor;

    [Header("Materials")]
    [Tooltip("The Character's mesh.")]
    [SerializeField]
    private SkinnedMeshRenderer characterMesh;

    [Tooltip("The Character's transparent material.")]
    [SerializeField]
    private Material transparentMaterial;

    [Tooltip("The Character's shadow.")]
    [SerializeField]
    private GameObject blobShadow;

    [Header("To Disable")]
    [Tooltip("A list of game objects to disable for the local player.")]
    [SerializeField]
    private List<GameObject> disableList;

    [Header("To Enable")]
    [Tooltip("A list of game objects to enable for the local player.")]
    [SerializeField]
    private List<GameObject> enableList;

    #endregion

    #region FIELDS

    protected Material defaultMaterial;
    protected string currentAnimationClip;

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
    /// Called when the script instance is being loaded (Awake).
    /// If overriden, must call base method in order to fully initialize the class.
    /// </summary>

    protected override void OnAwake()
    {
        base.OnAwake();

        // Cache the Character's default material
        defaultMaterial = characterMesh.material;

        // Cache the Character's starting animation clip
        currentAnimationClip = "Idle";
    }

    /// <summary>
    /// Overrides SetupPlayerInput method.
    /// Sets up InputActions for tank controls.
    /// </summary>

    protected override void InitPlayerInput()
    {
        // Check if there are any InputActions cached
        if (inputActions == null)
            return;

        // Movement is polled
        movementInputAction = inputActions.FindAction("Movement");
        movementInputAction?.Enable();

        sprintInputAction = inputActions.FindAction("Sprint");
        if (sprintInputAction != null)
        {
            sprintInputAction.started += OnSprint;
            sprintInputAction.performed += OnSprint;
            sprintInputAction.canceled += OnSprint;

            sprintInputAction?.Enable();
        }

        interactInputAction = inputActions.FindAction("Interact");
        if (interactInputAction != null)
        {
            interactInputAction.started += OnInteract;
            interactInputAction.performed += OnInteract;
            interactInputAction.canceled += OnInteract;

            interactInputAction?.Enable();
        }

        quickTurnInputAction = inputActions.FindAction("Quick Turn");
        if (quickTurnInputAction != null)
        {
            quickTurnInputAction.started += OnQuickTurn;
            quickTurnInputAction.performed += OnQuickTurn;

            quickTurnInputAction?.Enable();
        }
    }

    /// <summary>
    /// Unsubscribe from input action events and disable input actions.
    /// </summary>

    protected override void DeinitPlayerInput()
    {
        if (movementInputAction != null)
        {
            movementInputAction.Disable();
            movementInputAction = null;
        }

        if (sprintInputAction != null)
        {
            sprintInputAction.started -= OnSprint;
            sprintInputAction.performed -= OnSprint;
            sprintInputAction.canceled -= OnSprint;

            sprintInputAction.Disable();
            sprintInputAction = null;
        }

        if (interactInputAction != null)
        {
            interactInputAction.started -= OnInteract;
            interactInputAction.performed -= OnInteract;
            interactInputAction.canceled -= OnInteract;

            interactInputAction?.Disable();
            interactInputAction = null;
        }

        if (quickTurnInputAction != null)
        {
            quickTurnInputAction.started -= OnQuickTurn;
            quickTurnInputAction.performed -= OnQuickTurn;

            quickTurnInputAction?.Disable();
            quickTurnInputAction = null;
        }
    }

    /// <summary>
    /// Overrides OnStartLocalPlayer.
    /// Setup the local player's authority.
    /// </summary>

    public override void OnStartLocalPlayer()
    {
        // Disable game objects not used by the local player
        foreach (GameObject item in disableList)
        {
            item.SetActive(false);
        }

        // Enable game objects used by the local player
        foreach (GameObject item in enableList)
        {
            item.SetActive(true);
        }
    }

    /// <summary>
    /// Extends OnStart.
    /// Only allow inputs on the local player's Character.
    /// </summary>

    protected override void OnStart()
    {
        base.OnStart();

        if (!isLocalPlayer)
            UnsubFromInputActions();
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
        quickTurnSpeed = 380;

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
    /// Overrides OnUpdate.
    /// Only actively update the local player's state;
    /// </summary>

    protected override void OnUpdate()
    {
        if (isLocalPlayer)
            base.OnUpdate();
    }

    /// <summary>
    /// Overrides HandleInput method.
    /// Rotate the Character and adjust their speed accordingly.
    /// </summary>

    protected override void HandleInput()
    {
        if (inputActions == null)
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
            SetSprintState(movementInput);
        }

        // Perform quick turn rotation
        if (IsQuickTurning())
        {
            // Don't allow the player to move while quick turning
            SetMovementDirection(Vector3.zero);

            Quaternion quickTurnRotation = GetQuickTurnRotation();

            SetRotation(Quaternion.RotateTowards(
                GetRotation(), quickTurnRotation, quickTurnSpeed * Time.deltaTime));

            // Stop turning after reaching close to 180 degrees
            if (Quaternion.Angle(GetRotation(), quickTurnRotation) < 0.1f)
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
    /// Only allow the Character to sprint forward.
    /// </summary>

    protected virtual void SetSprintState(Vector2 movementInput)
    {
        if (movementInput.y > 0.0f)
        {
            canEverSprint = true;
        }
        else if (movementInput.y <= 0.0f)
        {
            canEverSprint = false;
        }
    }

    /// <summary>
    /// Set the Character's rotation rate based on the their input.
    /// </summary>

    protected virtual void SetRotationRate(Vector2 movementInput)
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

    protected virtual void SetQuickTurnRotation()
    {
        // The quick turn will always be 180 degrees from the current rotation
        _quickTurnRotation = GetRotation() * Quaternion.AngleAxis(180f, Vector3.up);
    }

    /// <summary>
    /// Updates the Character's animation to quick turn.
    /// </summary>

    protected virtual void PlayQuickTurnAnimation()
    {
        if (currentAnimationClip != "Quick Turn")
        {
            CmdChangeAnimationClip("Quick Turn");
            currentAnimationClip = "Quick Turn";
        }

        animancer.TryPlay("Quick Turn", 0.25f);
    }

    /// <summary>
    /// Updates the Character's movement animation.
    /// </summary>

    protected virtual void PlayMovementAnimation()
    {
        Vector2 movementInput = GetMovementInput();

        // We're not moving
        if (movementInput == Vector2.zero)
        {
            if (currentAnimationClip != "Idle")
            {
                CmdChangeAnimationClip("Idle");
                currentAnimationClip = "Idle";
            }

            animancer.TryPlay("Idle", 0.25f);
        }

        // Moving forward
        else if (movementInput.y > 0f) //&& (movementInput.x > -0.5f && movementInput.x < 0.5f))
        {
            if (IsSprinting())
            {
                if (currentAnimationClip != "Run")
                {
                    CmdChangeAnimationClip("Run");
                    currentAnimationClip = "Run";
                }

                animancer.TryPlay("Run", 0.25f);
            }
            else
            {
                if (currentAnimationClip != "Walk")
                {
                    CmdChangeAnimationClip("Walk");
                    currentAnimationClip = "Walk";
                }

                animancer.TryPlay("Walk", 0.25f);
            }
        }

        // Moving backwards
        else if (movementInput.y < 0f) //&& (movementInput.x > -0.5f && movementInput.x < 0.5f))
        {
            if (currentAnimationClip != "Walk Backwards")
            {
                CmdChangeAnimationClip("Walk Backwards");
                currentAnimationClip = "Walk Backwards";
            }

            animancer.TryPlay("Walk Backwards", 0.25f);
        }

        // Turning left
        else if (movementInput.x < 0)
        {
            if (currentAnimationClip != "Turn Left")
            {
                CmdChangeAnimationClip("Turn Left");
                currentAnimationClip = "Turn Left";
            }

            animancer.TryPlay("Turn Left", 0.25f);
        }

        // Turning right
        else if (movementInput.x > 0f)
        {
            if (currentAnimationClip != "Turn Right")
            {
                CmdChangeAnimationClip("Turn Right");
                currentAnimationClip = "Turn Right";
            }

            animancer.TryPlay("Turn Right", 0.25f);
        }
    }

    /// <summary>
    /// Unsub from all input action handlers.
    /// </summary>

    protected virtual void UnsubFromInputActions()
    {
        movementInputAction = null;

        if (sprintInputAction != null)
        {
            sprintInputAction.started -= OnSprint;
            sprintInputAction.performed -= OnSprint;
            sprintInputAction.canceled -= OnSprint;

            sprintInputAction = null;
        }

        if (interactInputAction != null)
        {
            interactInputAction.started -= OnInteract;
            interactInputAction.performed -= OnInteract;
            interactInputAction.canceled -= OnInteract;

            interactInputAction = null;
        }

        if (quickTurnInputAction != null)
        {
            quickTurnInputAction.started -= OnQuickTurn;
            quickTurnInputAction.performed -= OnQuickTurn;

            quickTurnInputAction = null;
        }
    }

    /// <summary>
    /// Sends a command to the server, giving it
    ///  the updated animation clip of this Character.
    /// </summary>

    [Command]
    protected virtual void CmdChangeAnimationClip(string animationClip)
    {
        networkAnimations.SetAnimationClip(animationClip);
    }

    /// <summary>
    /// Sends a command to the server, telling it
    ///  to change this Character's transperency.
    /// </summary>

    [Command]
    protected virtual void CmdChangeTransparency()
    {
        RpcChangeTransparency();
    }

    /// <summary>
    /// Changes the transparency of the Character 
    ///  for the player that made this call.
    /// </summary>
    
    [ClientRpc(includeOwner = false)]
    protected virtual void RpcChangeTransparency()
    {
        ChangeTransparency();
    }

    /// <summary>
    /// Sends a command to the server, telling it
    ///  to start fading this Character.
    /// </summary>

    [Command]
    protected virtual void CmdCharacterFade(float start, float end, float duration)
    {
        RpcCharacterFade(start, end, duration);
    }

    /// <summary>
    /// Fades the Character for the player that made this call.
    /// </summary>
    
    [ClientRpc(includeOwner = false)]
    protected virtual void RpcCharacterFade(float start, float end, float duration)
    {
        StartCoroutine(CharacterFade(start, end, duration));
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

        if (triggerSensor.GetDetections().Count != 0)
        {
            List<GameObject> detections = triggerSensor.GetDetectionsByDistance();
            GameObject nearestGameObject = detections[0];
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
    /// Enables the player's controls.
    /// </summary>

    public virtual void EnableControls()
    {
        movementInputAction.Enable();
        interactInputAction.Enable();
        quickTurnInputAction.Enable();
    }

    /// <summary>
    /// Disables the player's controls.
    /// </summary>

    public virtual void DisableControls()
    {
        movementInputAction.Disable();
        interactInputAction.Disable();
        quickTurnInputAction.Disable();
    }

    /// <summary>
    /// Syncs this Character's current animation accross all clients
    /// </summary>

    public virtual void SyncAnimationClip(string newAnimationClip)
    {
        animancer.TryPlay(newAnimationClip, 0.25f);
    }

    /// <summary>
    /// Change the Character's material to solid or transparent.
    /// </summary>

    public virtual void ChangeTransparency()
    {
        // The local player needs to let other clients know
        //  to change the transparency for this character
        if (isLocalPlayer)
            CmdChangeTransparency();

        if (characterMesh.material == defaultMaterial)
        {
            characterMesh.material = transparentMaterial;
        }
        else
        {
            characterMesh.material = defaultMaterial;
        }
    }

    /// <summary>
    /// Fade the Character in or out.
    /// </summary>

    public virtual IEnumerator CharacterFade(float start, float end, float duration)
    {
        // The local player needs to let other clients know
        //  to fade this character in or out
        if (isLocalPlayer)
            CmdCharacterFade(start, end, duration);

        if (end != 0.0f)
        {
            characterMesh.enabled = true;
            blobShadow.SetActive(true);
        }

        Color tempColor = characterMesh.material.color;

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;

            // Right here, you can now use normalizedTime as the third parameter in any Lerp from start to end
            tempColor.a = Mathf.Lerp(start, end, normalizedTime);
            characterMesh.material.color = tempColor;

            yield return null;
        }

        // Without this, the value will end at something like 0.9992367
        tempColor.a = end;
        characterMesh.material.color = tempColor;

        if (end == 0.0f)
        {
            characterMesh.enabled = false;
            blobShadow.SetActive(false);
        }
    }

    #endregion
}
