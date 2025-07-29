using System;
using Random = System.Random;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[System.Serializable]
public class GameState
{
    public bool isSolved;
    public int score;
    public List<CardState> cardsState = new List<CardState>();
    [System.Serializable]
    public class CardState
    {
        public bool isSolved;
        public string spriteName;
        public bool isOpen = false;
    }
}

public class GameManager : MonoBehaviour
{
    [Header("OngoingGameState")]
    public GameState ongoingGameState;

    [SerializeField] LayoutSpawner layoutSpawner;
    [SerializeField] List<LayoutSO> levelsLayoutSO = new List<LayoutSO>();
    LayoutSO ongoingLayoutSO;
    [SerializeField] List<Card> clickedSequenceOfCards = new List<Card>();
    bool levelStarted = false;
    private void OnEnable()
    {
        GameEvents.OnCheckForLevelCompletion += CheckForLevelCompletion;
        GameEvents.OnLayoutReady += LayoutSpawned;
        GameEvents.OnPlayerClickedShownCard += PlayerClickedShownCard;
        GameEvents.OnPlayerClickedHiddenCard += PlayerClickedHiddenCard;
        GameEvents.OnCardFlipFinished += CardFlipFinished;
    }

    private void OnDisable()
    {
        GameEvents.OnCheckForLevelCompletion -= CheckForLevelCompletion;
        GameEvents.OnLayoutReady -= LayoutSpawned;
        GameEvents.OnPlayerClickedShownCard -= PlayerClickedShownCard;
        GameEvents.OnPlayerClickedHiddenCard -= PlayerClickedHiddenCard;
        GameEvents.OnCardFlipFinished -= CardFlipFinished;
    }

    void Start()
    {
        Spawn();
    }

    public void Spawn()
    {
        if (ongoingLayoutSO == null)
        {
            ongoingLayoutSO = levelsLayoutSO[UnityEngine.Random.Range(0, levelsLayoutSO.Count)];
        }
        levelStarted = false;
        clickedSequenceOfCards.Clear();
        layoutSpawner.SpawnLayout(ongoingLayoutSO);
    }

    public void LayoutSpawned()
    {
        levelStarted = true;
        foreach (var x in layoutSpawner.InsLayoutHorizontals)
        {
            foreach (var y in x.InsCards)
            {
                y.ToggleAllowClick(true);
            }
        }
    }

    public void PlayerClickedShownCard(Card c)
    {

    }

    public void PlayerClickedHiddenCard(Card c)
    {
        if (clickedSequenceOfCards.Contains(c))
        {
            Debug.LogError(c.transform.name + " already in clickedSequenceOfCards");
            return;
        }
        c.Show();

        List<Card> tempCorrectCardsSequence = new List<Card>();

        bool isIncorrectClick = true;
        foreach (var x in clickedSequenceOfCards)
        {
            if (x.CardSprite == c.CardSprite)
            {
                if (tempCorrectCardsSequence.Contains(x) == false)
                {
                    tempCorrectCardsSequence.Add(x);
                }
                if (tempCorrectCardsSequence.Contains(c) == false)
                {
                    tempCorrectCardsSequence.Add(c);
                }
                isIncorrectClick = false;
            }
        }

        if (tempCorrectCardsSequence.Count == ongoingLayoutSO.NumOfCopiesInGrid)
        {
            c.CallEscapedTheGrid(tempCorrectCardsSequence);
            clickedSequenceOfCards.Clear();
        }
        else if (isIncorrectClick == false)
        {
            if (clickedSequenceOfCards.Contains(c) == false)
            {
                clickedSequenceOfCards.Add(c);
            }
        }
        else if (clickedSequenceOfCards.Count == 0)
        {
            clickedSequenceOfCards.Add(c);
        }
        else
        {
            List<Card> incorrectCardsSequence = new List<Card>(clickedSequenceOfCards);
            c.HideASAP(incorrectCardsSequence);

            clickedSequenceOfCards.Clear();
        }
    }

    public void CardFlipFinished(Card c)
    {
        if (c.IsSolved)
        {
            return;
        }
        if (levelStarted)
        {
            if (clickedSequenceOfCards.Contains(c) == false)
            {
                c.ToggleAllowClick(true);
            }
        }
    }

    public void CheckForLevelCompletion()
    {
        if (IsLevelSolved())
        {
            ongoingGameState.isSolved = true;
            if (ienumRestartLevel != null)
            {
                StopCoroutine(ienumRestartLevel);
            }
            StartCoroutine(ienumRestartLevel = RestartLevel());
            //level finished
        }
    }

    IEnumerator ienumRestartLevel;
    IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(1f);
        ongoingGameState = new GameState();
        ongoingLayoutSO = null;
        Spawn();
    }

    bool IsLevelSolved()
    {
        List<Card> remainingCardsToSolve = new List<Card>();
        foreach (var x in layoutSpawner.InsLayoutHorizontals)
        {
            foreach (var y in x.InsCards)
            {
                if (y.IsSolved == false)
                {
                    remainingCardsToSolve.Add(y);
                }
            }
        }
        if (remainingCardsToSolve.Count <= 1)
        {
            return true;
        }
        else
        {
            foreach (var x in remainingCardsToSolve)
            {
                List<Card> group = new List<Card>();
                group.Add(x);
                foreach (var y in remainingCardsToSolve)
                {
                    if (x.CardSprite == y.CardSprite)
                    {
                        if (group.Contains(y) == false)
                        {
                            group.Add(y);
                        }
                    }
                }
                if (group.Count >= ongoingLayoutSO.NumOfCopiesInGrid)
                {
                    return false;
                }
            }
        }
        return false;
    }

    public void SaveGameState()
    {

    }
}
