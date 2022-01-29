using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable<GameObject>
{
    [SerializeField] private GameObject exitPoint;

    private Transition transition;

    void Start()
    {
        transition = GetComponentInParent<MansionSetup>().GetTransition();
    }

    public void Interact(GameObject player)
    {
        StartCoroutine(TraverseDoor(player));
    }
    
    private IEnumerator TraverseDoor(GameObject player)
    {
        ClassicCharacter character = player.GetComponent<ClassicCharacter>();

        // Disable the player's controls
        character.DisableControls();
        character.ChangeTransparency();

        // Start the door transition
        StartCoroutine(character.CharacterFade(1.0f, 0.0f, 0.5f));
        StartCoroutine(transition.ScreenFade(0.0f, 1.0f, 0.5f));
        yield return new WaitForSeconds(.75f);

        // Move the Character to the next room
        character.transform.position = exitPoint.transform.position;
        character.transform.rotation = exitPoint.transform.rotation;
        yield return new WaitForSeconds(.75f);

        // End the door transition
        StartCoroutine(character.CharacterFade(0.0f, 1.0f, 0.5f));
        StartCoroutine(transition.ScreenFade(1.0f, 0.0f, 0.5f));
        yield return new WaitForSeconds(0.5f);

        // Re-enable the player's controls
        character.ChangeTransparency();
        character.EnableControls();
    }
}
