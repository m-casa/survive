using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable<GameObject>
{
    [SerializeField] private GameObject exitPoint;

    public void Interact(GameObject player)
    {
        StartCoroutine(TraverseDoor(player));
    }

    private IEnumerator TraverseDoor(GameObject player)
    {
        ClassicCharacter character = player.GetComponent<ClassicCharacter>();

        character.actions.Disable();

        UIManager.Instance.StartCrossfade();
        yield return new WaitForSeconds(1f);

        character.transform.position = exitPoint.transform.position;
        character.transform.rotation = exitPoint.transform.rotation;

        UIManager.Instance.EndCrossfade();
        yield return new WaitForSeconds(1f);

        character.actions.Enable();
    }
}
