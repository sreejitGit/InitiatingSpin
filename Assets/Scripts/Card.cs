using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [Header("Card data")]
    [SerializeField] CardData myCardData;

    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Transform contentAreaTransform;
    [SerializeField] GameObject hiddenObj;
    [SerializeField] GameObject shownObj;
    [SerializeField] Image cardContentImage;

    [Header("UI scale to fit")]
    [SerializeField] RectTransform parentToFitTo;
    [SerializeField] List<RectTransform> targetRectsToScaleToFit;

    public void InitData(CardData cardData)
    {
        myCardData = cardData;
        if (myCardData.name == "")
        {
            myCardData.name = myCardData.sprite.name;
        }
        transform.name = transform.name + " " + myCardData.name;
    }

    public void InitUI()
    {
        cardContentImage.sprite = myCardData.sprite;
        RescaleUI();
        Hide();
        Show();
    }

    void RescaleUI()
    {
        foreach (var x in targetRectsToScaleToFit)
        {
            Utils.Rescale(parentToFitTo, x);
        }
    }

    void Show()
    {
        canvasGroup.alpha = 1f;
        hiddenObj.SetActive(false);
        shownObj.SetActive(true);
    }

    void Hide()
    {
        hiddenObj.SetActive(true);
        shownObj.SetActive(false);
    }
}
