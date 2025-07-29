using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] bool allowClick = false;
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
        Hide(0f);
        StartCoroutine(Utils.FadeCanvas(canvasGroup, 0f, 1f, 0.5f));
    }

    void RescaleUI()
    {
        foreach (var x in targetRectsToScaleToFit)
        {
            Utils.Rescale(parentToFitTo, x);
        }
    }

    void Show(float duration = 0.5f)
    {
        StopFlippingAnim();
        StartCoroutine(flippingAnim = FlipToShowRoutine(duration));
    }

    void Hide(float duration)
    {
        StopFlippingAnim();
        StartCoroutine(flippingAnim = FlipToHideRoutine(duration));
    }

    void StopFlippingAnim()
    {
        if (flippingAnim != null)
        {
            StopCoroutine(flippingAnim);
        }
    }

    public void ClickedOnShown()
    {
        if (allowClick)
        {
            Hide(0.5f);
        }
    }

    public void ClickedOnHidden()
    {
        if (allowClick)
        {
            Show();
        }
    }

    IEnumerator flippingAnim;
    private IEnumerator FlipToShowRoutine(float duration)
    {
        allowClick = false;
        float halfTime = duration / 2f;

        hiddenObj.SetActive(true);
        hiddenObj.transform.localScale = Vector2.one;
        hiddenObj.transform.localEulerAngles = Vector3.zero;

        shownObj.SetActive(true);
        shownObj.transform.localScale = Vector2.zero;

        Quaternion startRotation = hiddenObj.transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, 270, 0);
        shownObj.transform.rotation = endRotation;
        hiddenObj.transform.rotation = startRotation;

        yield return Utils.BounceUpEffect(hiddenObj.transform, hiddenObj.transform.localScale);
        yield return Utils.RotateSlerp(hiddenObj.transform, startRotation, endRotation, halfTime);
        hiddenObj.transform.rotation = endRotation;
        hiddenObj.SetActive(false);

        shownObj.transform.localScale = hiddenObj.transform.localScale;
        yield return Utils.RotateSlerp(shownObj.transform, endRotation, startRotation, halfTime);
        shownObj.transform.rotation = startRotation;
        yield return Utils.BounceDownEffect(shownObj.transform, Vector2.one);

        allowClick = true;
    }

    private IEnumerator FlipToHideRoutine(float duration)
    {
        allowClick = false;
        if (duration <= 0f)
        {
            hiddenObj.SetActive(true);
            shownObj.SetActive(false);
            hiddenObj.transform.localEulerAngles = Vector3.zero;
        }
        else
        {
            float halfTime = duration / 2f;

            hiddenObj.SetActive(true);
            hiddenObj.transform.localScale = Vector2.zero;

            shownObj.SetActive(true);
            shownObj.transform.localScale = Vector2.one;
            shownObj.transform.localEulerAngles = Vector3.zero;

            Quaternion startRotation = shownObj.transform.rotation;
            Quaternion endRotation = startRotation * Quaternion.Euler(0, 270, 0);

            shownObj.transform.rotation = startRotation;
            hiddenObj.transform.rotation = endRotation;

            yield return Utils.BounceUpEffect(shownObj.transform, shownObj.transform.localScale);
            yield return Utils.RotateSlerp(shownObj.transform, startRotation, endRotation,halfTime);
            shownObj.transform.rotation = endRotation;
            shownObj.SetActive(false);

            hiddenObj.transform.localScale = shownObj.transform.localScale;
            yield return Utils.RotateSlerp(hiddenObj.transform, endRotation, startRotation,halfTime);
            hiddenObj.transform.rotation = startRotation;
            yield return Utils.BounceDownEffect(hiddenObj.transform, Vector2.one);
        }
        allowClick = true;
    }
    

}
