using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Transform contentAreaTransform;
    [SerializeField] GameObject hiddenObj;
    [SerializeField] GameObject shownObj;
    [SerializeField] Image cardContentImage;

    [Header("UI scale to fit")]
    [SerializeField] RectTransform parentToFitTo;
    [SerializeField] List<RectTransform> targetRectsToScaleToFit;

    public void InitData()
    {
    
    }

    public void InitUI()
    {
        foreach (var x in targetRectsToScaleToFit)
        {
            Utils.Rescale(parentToFitTo,x);
        }

    }
}
