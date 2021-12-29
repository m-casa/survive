using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable<GameObject>
{
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private GameObject exitPoint;

    private CanvasGroup crossfadeGroup;

    private void Start()
    {
        crossfadeGroup = UIManager.Instance.GetCrossfadeGroup();
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

        yield return StartCoroutine(ScreenFade(0f, 1f));

        character.transform.position = exitPoint.transform.position;
        character.transform.rotation = exitPoint.transform.rotation;

        yield return StartCoroutine(ScreenFade(1f, 0f));

        character.actions.Enable();
    }

    // Wait for the screen to fade over time
    private IEnumerator ScreenFade(float start, float end)
    {
        for (float t = 0f; t < transitionDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / transitionDuration;

            // Right here, you can now use normalizedTime as the third parameter in any Lerp from start to end
            crossfadeGroup.alpha = Mathf.Lerp(start, end, normalizedTime);
            yield return null;
        }
        crossfadeGroup.alpha = end; // Without this, the value will end at something like 0.9992367
    }
}
