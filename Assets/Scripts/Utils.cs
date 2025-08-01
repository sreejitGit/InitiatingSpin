using System;
using Random = System.Random;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    private static Random rng = new Random();

    public static void Randomize<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);  // Random index
            (list[k], list[n]) = (list[n], list[k]);  // Swap
        }
    }

    public static Coroutine DelayExecute(this MonoBehaviour monoBehaviour, Action action, float delayTime)
    {
        return monoBehaviour.StartCoroutine(DelayedAction(monoBehaviour, action, delayTime));
    }

    private static IEnumerator DelayedAction(MonoBehaviour monoBehaviour, Action action, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        if (monoBehaviour != null && monoBehaviour.gameObject != null && monoBehaviour.gameObject.activeInHierarchy)
        {
            action?.Invoke();
        }
    }

    public static void Rescale(RectTransform parentToFitTo, RectTransform t, System.Action onComplete = null)
    {
        if (parentToFitTo == null || t == null)
        {
            return;
        }

        Vector2 availableAreaSize = parentToFitTo.rect.size;

        RectTransform targetRect = t;
        while (targetRect.sizeDelta.x * targetRect.localScale.x > availableAreaSize.x || targetRect.sizeDelta.y * targetRect.transform.localScale.y > availableAreaSize.y)
        {
            targetRect.transform.localScale -= targetRect.transform.localScale * 1 / 100f;
        }

        while (targetRect.sizeDelta.x * targetRect.transform.localScale.x < availableAreaSize.x && targetRect.sizeDelta.y * targetRect.transform.localScale.y < availableAreaSize.y)
        {
            targetRect.transform.localScale += targetRect.transform.localScale * 1 / 100f;

        }
        if (onComplete != null)
        {
            onComplete.Invoke();
        }
    }

    public static IEnumerator BounceUpEffect(Transform t, Vector3 originalScale, float bounceScale = 1.1f, float bounceDuration = 0.1f)
    {
        yield return BounceEffect(t, originalScale, originalScale * bounceScale, bounceDuration);
    }

    public static IEnumerator BounceDownEffect(Transform t, Vector3 originalScale, float bounceScale = 1.1f, float bounceDuration = 0.1f)
    {
        yield return BounceEffect(t, originalScale * bounceScale, originalScale, bounceDuration);
    }

    public static IEnumerator BounceEffect(Transform t, Vector3 originalScale, Vector3 targetScale, float bounceDuration = 0.1f)
    {
        float time = 0f;
        while (time < bounceDuration)
        {
            t.transform.localScale = Vector3.Lerp(originalScale, targetScale, time / bounceDuration);
            time += Time.deltaTime;
            yield return null;
        }
        t.transform.localScale = targetScale;
    }

    public static IEnumerator FadeCanvas(CanvasGroup canvasGroup, float from, float to, float duration)
    {
        float time = 0f;
        canvasGroup.alpha = from;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }


    public static IEnumerator RotateSlerp(Transform t,Quaternion startRotation, Quaternion endRotation, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            t.rotation = Quaternion.Slerp(startRotation, endRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
    }

    public static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        char[] stringChars = new char[length];

        for (int i = 0; i < length; i++)
        {
            stringChars[i] = chars[rng.Next(chars.Length)];
        }

        return new string(stringChars);
    }
}
