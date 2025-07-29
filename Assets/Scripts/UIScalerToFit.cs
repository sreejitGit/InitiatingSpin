using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIScalerToFit : MonoBehaviour
{
    [SerializeField] RectTransform parentToFitTo;
    [SerializeField] List<RectTransform> targetRectsToScaleToFit;
    [SerializeField] UnityEvent onComplete;

    [SerializeField] float delay = 0f;

    [ContextMenu("Start")]
    public void Start() {
        if (delay > 0f) {
            this.Execute(()=> {
                Begin();
            }, delay);
        }
        else {
            Begin();
        }
    }

    void Begin() {
        if (parentToFitTo != null) {
            if (parentToFitTo.GetComponentsInParent<Canvas>() != null) {
                foreach (var x in targetRectsToScaleToFit) {
                    if (x != null) {
                        Rescale(x);
                    }
                }
            }
        }
    }

    void Rescale(RectTransform t) {
        if(parentToFitTo == null) {
            return;
        }

        Vector2 availableAreaSize = parentToFitTo.rect.size;

        RectTransform targetRect = t;
        while (targetRect.sizeDelta.x * targetRect.localScale.x > availableAreaSize.x || targetRect.sizeDelta.y * targetRect.transform.localScale.y > availableAreaSize.y) {
            targetRect.transform.localScale -= targetRect.transform.localScale * 1 / 100f;
        }

        while (targetRect.sizeDelta.x * targetRect.transform.localScale.x < availableAreaSize.x && targetRect.sizeDelta.y * targetRect.transform.localScale.y < availableAreaSize.y) {
            targetRect.transform.localScale += targetRect.transform.localScale * 1 / 100f;
        }
        onComplete.Invoke();
    }
}
