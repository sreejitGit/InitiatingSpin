using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] bool allowClick = false;

    [SerializeField] bool isEmpty = true;
    public bool IsEmpty => isEmpty;

    [SerializeField] bool isOpen = false;
    public bool IsOpen => isOpen;

    [SerializeField] bool isSolved = false;
    public bool IsSolved => isSolved;

    [Header("Card data")]
    [SerializeField] CardData myCardData;
    public Sprite CardSprite => myCardData.sprite;

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
        isEmpty = false;
        myCardData = cardData;
        if (myCardData.name == "")
        {
            myCardData.name = myCardData.sprite.name;
        }
        transform.name = transform.name + " " + myCardData.name;
    }

    public void ToggleAllowClick(bool target)
    {
        allowClick = target;
    }

    public void InitUI()
    {
        cardContentImage.sprite = myCardData.sprite;
        RescaleUI();
    }

    void OnRescaleDone()
    {
        if (isEmpty)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            Hide(0f);
            StartCoroutine(PlayWelcomeAnimation());
        }
    }

    void RescaleUI()
    {
        for (int i = 0; i < targetRectsToScaleToFit.Count; i++)
        {
            if (i == targetRectsToScaleToFit.Count - 1)
            {
                Utils.Rescale(parentToFitTo, targetRectsToScaleToFit[i], OnRescaleDone);
            }
            else
            {
                Utils.Rescale(parentToFitTo, targetRectsToScaleToFit[i]);
            }
        }
    }

    public void Show(float duration = 0.5f)
    {
        StopFlippingAnim();
        StartCoroutine(flippingAnim = FlipToShowRoutine(duration));
    }

    public void Hide(float duration)
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
        if (isSolved)
        {
            return;
        }
        if (allowClick)
        {
            GameEvents.PlayerClickedShownCard(this);
        }
    }

    public void ClickedOnHidden()
    {
        if (isSolved)
        {
            return;
        }
        if (allowClick)
        {
            GameEvents.PlayerClickedHiddenCard(this);
        }
    }

    IEnumerator PlayWelcomeAnimation(float duration = 0.125f,float bounceScale = 1.2f)
    {
        float time = 0f;

        Vector3 initialScale = transform.localScale;
        Vector3 overshootScale = initialScale * bounceScale;

        StartCoroutine(Utils.FadeCanvas(canvasGroup, 0f, 1f, duration));
        yield return new WaitForSeconds(duration * 0.25f);
        while (time < duration)
        {
            float t = time / duration;
            transform.localScale = Vector3.Lerp(initialScale, overshootScale, t);
            time += Time.deltaTime;
            yield return null;
        }

        time = 0f;
        while (time < 0.1f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, initialScale, time / 0.1f);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = initialScale;
    }

    IEnumerator flippingAnim;
    private IEnumerator FlipToShowRoutine(float duration)
    {
        isOpen = true;
        ToggleAllowClick(false);
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

        if (halfTime > 0f)
        {
            yield return Utils.BounceUpEffect(hiddenObj.transform, hiddenObj.transform.localScale);
            yield return Utils.RotateSlerp(hiddenObj.transform, startRotation, endRotation, halfTime);
        }
        hiddenObj.transform.rotation = endRotation;
        hiddenObj.SetActive(false);

        shownObj.transform.localScale = hiddenObj.transform.localScale;
        if (halfTime > 0f)
        {
            yield return Utils.RotateSlerp(shownObj.transform, endRotation, startRotation, halfTime);
        }
        shownObj.transform.rotation = startRotation;
        if (halfTime > 0f)
        {
             yield return Utils.BounceDownEffect(shownObj.transform, Vector2.one);
        }

        if (hideOnCorrectCardsSequence.Count > 0)
        {
            SFXManager.instance.PlaySFXOnce(SFXManager.GameplaySFXType.CorrectMatch);
            foreach (var x in hideOnCorrectCardsSequence)
            {
                x.EscapedTheGrid();
            }
            hideOnCorrectCardsSequence.Clear();
        }
        else if (isSolved)
        {
            EscapedTheGrid();
        }
        else if (hideOnIncorrectCardsSequence.Count > 0)
        {
            this.DelayExecute(() =>
            { 
                SFXManager.instance.PlaySFXOnce(SFXManager.GameplaySFXType.IncorrectMatch);
            }, 0.5f);
            foreach (var x in hideOnIncorrectCardsSequence)
            {
                x.ToggleAllowClick(true);
                x.Hide(0.3f);
            }
            ToggleAllowClick(true);
            Hide(0.3f);
            hideOnIncorrectCardsSequence.Clear();
        }
        else
        {
            GameEvents.CardFlipFinished(this);
        }
    }

    private IEnumerator FlipToHideRoutine(float duration)
    {
        isOpen = false;
        ToggleAllowClick(false);
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

            if (halfTime > 0f)
            {
                yield return Utils.BounceUpEffect(shownObj.transform, shownObj.transform.localScale);
                SFXManager.instance.PlaySFXOnce(SFXManager.GameplaySFXType.CardClose);
                yield return Utils.RotateSlerp(shownObj.transform, startRotation, endRotation, halfTime);
            }   
            shownObj.transform.rotation = endRotation;
            shownObj.SetActive(false);

            hiddenObj.transform.localScale = shownObj.transform.localScale;
            if (halfTime > 0f)
            {
                yield return Utils.RotateSlerp(hiddenObj.transform, endRotation, startRotation, halfTime);
            }
            hiddenObj.transform.rotation = startRotation;
            if (halfTime > 0f)
            {
                yield return Utils.BounceDownEffect(hiddenObj.transform, Vector2.one);
            }
        }
        GameEvents.CardFlipFinished(this);
    }

    void EscapedTheGrid()
    {
        ChangeSolvedStateToTrue();
        StartCoroutine(Utils.FadeCanvas(canvasGroup, 1f, 0.5f, 0.25f));
        GameEvents.CheckForLevelCompletion();
    }

    List<Card> hideOnIncorrectCardsSequence = new List<Card>();
    public void HideASAP(List<Card> incorrectCardsSequence)
    {
        foreach (var x in incorrectCardsSequence)
        {
            x.ChangeOpenStateToFalse();
        }
        ChangeOpenStateToFalse();
        hideOnIncorrectCardsSequence = incorrectCardsSequence;
    }

    List<Card> hideOnCorrectCardsSequence = new List<Card>();
    public void CallEscapedTheGrid(List<Card> correctCardsSequence)
    {
        foreach (var x in correctCardsSequence)
        {
            x.ChangeSolvedStateToTrue();
        }
        hideOnCorrectCardsSequence = correctCardsSequence;
    }

    public void ChangeSolvedStateToTrue()
    {
        isSolved = true;
    }

    public void ChangeOpenStateToFalse()
    {
        isSolved = false;
        isOpen = false;
    }

    public void SetAsSolvedFromSavedState()
    {
        isOpen = true;
        ChangeSolvedStateToTrue();
        isEmpty = false;
        Show(0.25f);
    }

    public void SetAsOpenFromSavedState()
    {
        isOpen = true;
        isSolved = false;
        isEmpty = false;
        Show(0.25f);
    }

    public void SetAsEmpty()
    {
        isOpen = false;
        isSolved = false;
        isEmpty = true;
    }
}
