using System.Collections;
using UnityEngine;

public class Transition : MonoBehaviour
{
    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private CanvasGroup flashCanvasGroup;
    [SerializeField] private CanvasGroup fadeCanvasGroup;

    void Awake()
    {
        StartFade(1.0f, 0.0f, 1.5f);
    }

    public void StartFade(float start, float end, float duration)
    {
        StartCoroutine(ScreenFade(start, end, duration));
    }

    public void StartHostTransition()
    {
        StartCoroutine(HostTransition());
    }

    public void StartJoinTransition()
    {
        StartCoroutine(JoinTransition());
    }

    private IEnumerator ScreenFlash(float start, float end, float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;

            flashCanvasGroup.alpha = Mathf.Lerp(start, end, normalizedTime);

            yield return null;
        }

        flashCanvasGroup.alpha = end;

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;

            flashCanvasGroup.alpha = Mathf.Lerp(end, start, normalizedTime);

            yield return null;
        }

        flashCanvasGroup.alpha = start;
    }

    private IEnumerator ScreenFade(float start, float end, float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;

            fadeCanvasGroup.alpha = Mathf.Lerp(start, end, normalizedTime);

            yield return null;
        }

        fadeCanvasGroup.alpha = end;
    }

    private IEnumerator HostTransition()
    {
        yield return StartCoroutine(ScreenFlash(0.0f, 1.0f, 0.15f));
        yield return StartCoroutine(ScreenFade(0.0f, 1.0f, 1.5f));

        StartCoroutine(ScreenFade(1.0f, 0.0f, 1.5f));
        mainMenu.HostLobby();
    }

    private IEnumerator JoinTransition()
    {
        yield return StartCoroutine(ScreenFlash(0.0f, 1.0f, 0.15f));
        yield return StartCoroutine(ScreenFade(0.0f, 1.0f, 1.5f));

        StartCoroutine(ScreenFade(1.0f, 0.0f, 1.5f));
        mainMenu.JoinLocally();
    }
}
