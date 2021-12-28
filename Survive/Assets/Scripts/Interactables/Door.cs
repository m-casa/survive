using UnityEngine;

public class Door : MonoBehaviour, IInteractable<GameObject>
{
    [SerializeField] private GameObject exitPoint;

    public void Interact(GameObject character)
    {
        character.transform.position = exitPoint.transform.position;
        character.transform.rotation = exitPoint.transform.rotation;
    }
}
