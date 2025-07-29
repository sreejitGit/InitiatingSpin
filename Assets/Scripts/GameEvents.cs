using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents
{

    public static event System.Action OnCheckForLevelCompletion;
    public static event System.Action OnLayoutReady;
    public static event System.Action<Card> OnPlayerClickedShownCard;
    public static event System.Action<Card> OnPlayerClickedHiddenCard;
    public static event System.Action<Card> OnCardFlipFinished;

    public static void ClearEvents()
    {
        OnCheckForLevelCompletion = null;
        OnLayoutReady = null;
        OnPlayerClickedShownCard = null;
        OnPlayerClickedHiddenCard = null;
        OnCardFlipFinished = null;
    }

    public static void CheckForLevelCompletion()
    {
        OnCheckForLevelCompletion?.Invoke();
    }

    public static void LayoutSetupDone()
    {
        OnLayoutReady?.Invoke();
    }

    public static void PlayerClickedShownCard(Card c)
    {
        OnPlayerClickedShownCard?.Invoke(c);
    }

    public static void PlayerClickedHiddenCard(Card c)
    {
        OnPlayerClickedHiddenCard?.Invoke(c);
    }

    public static void CardFlipFinished(Card c)
    {
        OnCardFlipFinished?.Invoke(c);
    }
}
