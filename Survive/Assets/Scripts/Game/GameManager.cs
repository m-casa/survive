using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public InputActionAsset actions;

    private InputAction sprintInputAction { get; set; }

    void Awake()
    {
        // When our new scene loads, don't delete the game manager
        DontDestroyOnLoad(gameObject);

        // Check if we have an instance of the game manager
        if (Instance != null)
        {
            // If we already have a game manager, destroy this one
            Destroy(gameObject);
        }

        // Set this game manager as the primary instance since we don't have one
        Instance = this;

        //sprintInputAction = actions.FindAction("Sprint");
    }

    void OnEnable()
    {
        if (sprintInputAction != null)
        {
            sprintInputAction.Enable();

            sprintInputAction.started += TraceAction;
            sprintInputAction.canceled += TraceAction;
        }
    }

    void OnDisable()
    {
        if (sprintInputAction != null)
        {
            sprintInputAction.Disable();

            sprintInputAction.started -= TraceAction;
            sprintInputAction.canceled -= TraceAction;
        }
    }

    private void TraceAction(InputAction.CallbackContext ctx)
    {
        var trace = new InputActionTrace();

        // Subscribe trace to single Action.
        // (Use UnsubscribeFrom to unsubscribe)
        trace.SubscribeTo(sprintInputAction);

        // Subscribe trace to entire Action Map.
        // (Use UnsubscribeFrom to unsubscribe)
        trace.SubscribeTo(sprintInputAction);

        // Subscribe trace to all Actions in the system.
        trace.SubscribeToAll();

        Debug.Log("===============TRACE ACTION===============");

        // Record a single triggering of an Action.
        if (ctx.ReadValue<float>() > 0.5f)
        {
            trace.RecordAction(ctx);
            // Output trace to console.
            Debug.Log(string.Join(",\n", trace));
        }
        else
        {
            Debug.Log("Cancelled");
        }

        // Walk through all recorded Actions and then clear trace.
        foreach (var record in trace)
        {
            Debug.Log($"{record.action} was {record.phase} by control {record.control}");

            // To read out the value, you either have to know the value type or read the
            // value out as a generic byte buffer. Here, we assume that the value type is
            // float.

            Debug.Log("Value: " + record.ReadValue<float>());

            // If it's okay to accept a GC hit, you can also read out values as objects.
            // In this case, you don't have to know the value type.

            Debug.Log("Value: " + record.ReadValueAsObject());
        }

        trace.Clear();

        // Unsubscribe trace from everything.
        trace.UnsubscribeFromAll();

        // Release memory held by trace.
        trace.Dispose();
    }
}
