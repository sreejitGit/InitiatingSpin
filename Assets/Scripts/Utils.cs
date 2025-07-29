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

    public static Coroutine Execute(this MonoBehaviour monoBehaviour, Action action, float time)
    {
        return monoBehaviour.StartCoroutine(DelayedAction(monoBehaviour, action, time));
    }
    
    private static IEnumerator DelayedAction(MonoBehaviour monoBehaviour, Action action, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        if (monoBehaviour != null && monoBehaviour.gameObject != null && monoBehaviour.gameObject.activeInHierarchy)
        {
            action?.Invoke();
        }
    }

    public static void Rescale(RectTransform parentToFitTo, RectTransform t,System.Action onComplete = null) {
        if(parentToFitTo == null || t == null) {
            return;
        }

        Vector2 availableAreaSize = parentToFitTo.rect.size;

        RectTransform targetRect = t;
        while (targetRect.sizeDelta.x * targetRect.localScale.x > availableAreaSize.x || targetRect.sizeDelta.y * targetRect.transform.localScale.y > availableAreaSize.y) {
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
}
