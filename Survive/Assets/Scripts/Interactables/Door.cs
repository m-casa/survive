using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable<GameObject>
{
    [SerializeField] private GameObject exitPoint;

    private Transition transition;

    void Awake()
    {
        transition = GetComponentInParent<Map>().GetTransition();
    }

    public void Interact(GameObject player)
    {
        StartCoroutine(TraverseDoor(player));
    }
    
    private IEnumerator TraverseDoor(GameObject player)
    {
        ClassicCharacter character = player.GetComponent<ClassicCharacter>();

        // Start the door transition
        character.DisableControls();
        character.ChangeTransparency();
        character.StartFade(1.0f, 0.0f, 0.5f);
        transition.StartFade(0.0f, 1.0f, 0.5f);
        yield return new WaitForSeconds(.75f);

        character.transform.position = exitPoint.transform.position;
        character.transform.rotation = exitPoint.transform.rotation;

        // End the door transition
        yield return new WaitForSeconds(.75f);
        character.StartFade(0.0f, 1.0f, 0.5f);
        transition.StartFade(1.0f, 0.0f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        character.ChangeTransparency();
        character.EnableControls();
    }
}
