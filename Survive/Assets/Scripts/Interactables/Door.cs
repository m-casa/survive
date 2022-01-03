using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable<GameObject>
{
    [SerializeField] private GameObject exitPoint;

    private CanvasGroup crossfadeGroup;
    private float crossfadeDuration = 0.6f;

    void Start()
    {
        crossfadeGroup = GetComponentInParent<Map>().GetCrossfadeGroup();
    }

    public void Interact(GameObject player)
    {
        StartCoroutine(TraverseDoor(player));
    }
    
    private IEnumerator TraverseDoor(GameObject player)
    {
        // The player will not be allowed to move while transitioning
        ClassicCharacter character = player.GetComponent<ClassicCharacter>();

        character.actions.Disable();
        character.ChangeTransparency();

        // Start the door transition
        StartCoroutine(character.CharacterFade(1f, 0f));
        StartCoroutine(ScreenFade(0f, 1f));
        yield return new WaitForSeconds(1.15f);

        character.transform.position = exitPoint.transform.position;
        character.transform.rotation = exitPoint.transform.rotation;

        // End the door transition
        StartCoroutine(character.CharacterFade(0, 1f));
        StartCoroutine(ScreenFade(1f, 0f));
        yield return new WaitForSeconds(0.6f);

        character.ChangeTransparency();
        character.actions.Enable();
    }

    // Wait for the screen to fade over time
    private IEnumerator ScreenFade(float start, float end)
    {
        for (float t = 0f; t < crossfadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / crossfadeDuration;

            // Right here, you can now use normalizedTime as the third parameter in any Lerp from start to end
            crossfadeGroup.alpha = Mathf.Lerp(start, end, normalizedTime);

            yield return null;
        }

        // Without this, the value will end at something like 0.9992367
        crossfadeGroup.alpha = end;
    }
}
