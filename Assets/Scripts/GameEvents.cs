using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents
{
    public static event System.Action OnEnterHome;
    public static event System.Action OnResetGame;
    public static event System.Action OnEnterGame;
    public static event System.Action OnCheckForLevelCompletion;
    public static event System.Action OnLayoutReady;
    public static event System.Action<Card> OnPlayerClickedShownCard;
    public static event System.Action<Card> OnPlayerClickedHiddenCard;
    public static event System.Action<Card> OnCardFlipFinished;
    public static event System.Action<int> OnUpdatedCurrentScore;
    public static event System.Action<int> OnUpdatedCurrentMatches;
    public static event System.Action<int> OnUpdatedCurrentTries;

    public static void ClearEvents()
    {
        OnEnterHome = null;
        OnResetGame = null;
        OnEnterGame = null;
        OnCheckForLevelCompletion = null;
        OnLayoutReady = null;
        OnPlayerClickedShownCard = null;
        OnPlayerClickedHiddenCard = null;
        OnCardFlipFinished = null;
        OnUpdatedCurrentScore = null;
        OnUpdatedCurrentMatches = null;
        OnUpdatedCurrentTries = null;
    }

    public static void EnterHome()
    {
        OnEnterHome?.Invoke();
    }

    public static void ResetGameplay()
    {
        OnResetGame?.Invoke();
    }

    public static void EnterGame()
    {
        OnEnterGame?.Invoke();
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

    public static void UpdatedCurrentScore(int currentScore)
    {
        OnUpdatedCurrentScore?.Invoke(currentScore);
    }

    public static void UpdatedCurrentMatches(int currentMatches)
    {
        OnUpdatedCurrentMatches?.Invoke(currentMatches);
    }

    public static void UpdatedCurrentTries(int currentTries)
    {
        OnUpdatedCurrentTries?.Invoke(currentTries);
    }
}
