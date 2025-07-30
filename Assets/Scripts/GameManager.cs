using System;
using Random = System.Random;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Experimental.AI;

[System.Serializable]
public class GameState
{
    public LayoutState layoutState = new LayoutState();

    public void SetEmptyLayoutState()
    {
        layoutState = new LayoutState();
    }

    [System.Serializable]
    public class LayoutState
    {
        public string layoutID;
        public bool isSolved;
        public int score;
        public List<CardState> cardsState = new List<CardState>();

        public LayoutState Clone()
        {
            LayoutState x = new LayoutState();
            x.layoutID = layoutID;
            x.isSolved = isSolved;
            x.score = score;
            foreach (var y in cardsState)
            {
                x.cardsState.Add(y.Clone());
            }

            return x;
        }
    }
   
    [System.Serializable]
    public class CardState
    {
        public bool isSolved;
        public string spriteName;
        public bool isOpen = false;

        public CardState Clone()
        {
            CardState x = new CardState();
            x.isSolved = isSolved;
            x.spriteName = spriteName;
            x.isOpen = isOpen;
            return x;
        }
    }

    public GameState Clone()
    {
        GameState x = new GameState();
        x.layoutState = layoutState.Clone();
        return x;
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
        SaveGameState(true);
        GameEvents.OnCheckForLevelCompletion -= CheckForLevelCompletion;
        GameEvents.OnLayoutReady -= LayoutSpawned;
        GameEvents.OnPlayerClickedShownCard -= PlayerClickedShownCard;
        GameEvents.OnPlayerClickedHiddenCard -= PlayerClickedHiddenCard;
        GameEvents.OnCardFlipFinished -= CardFlipFinished;
    }

    void Awake()
    {
        ongoingGameState = LoadSavedData();
    }

    void Start()
    {
        bool foundLayoutFromLastGame = false;
        if (ongoingGameState.layoutState.isSolved == false)
        {
            foreach (var x in levelsLayoutSO)
            {
                if (x.layoutID == ongoingGameState.layoutState.layoutID)
                {
                    ongoingLayoutSO = x;
                    currentScore = ongoingGameState.layoutState.score;
                    foundLayoutFromLastGame = true;
                    break;
                }
            }
        }
        if (foundLayoutFromLastGame == false)
        {
            LoadRandomLayout();
            SaveGameState(true);
        }
        Spawn();
    }

    void LoadRandomLayout()
    {
        ongoingLayoutSO = levelsLayoutSO[UnityEngine.Random.Range(0, levelsLayoutSO.Count)];
        ongoingGameState.SetEmptyLayoutState();
    }

    void Spawn()
    {
        levelStarted = false;
        clickedSequenceOfCards.Clear();
        layoutSpawner.SpawnLayout(ongoingLayoutSO);
    }

    void LayoutSpawned()
    {
        foreach (var x in layoutSpawner.InsLayoutHorizontals)
        {
            foreach (var y in x.InsCards)
            {
                y.ToggleAllowClick(true);
            }
        }
        if (ongoingGameState.layoutState?.layoutID == ongoingLayoutSO.layoutID)
        {
            //reload from saved state.
            if (ongoingGameState.layoutState?.cardsState.Count > 0)
            {
                if (CheckIfSavedStateIsSameLayout())
                {
                    int index = 0;
                    foreach (var x in layoutSpawner.InsLayoutHorizontals)
                    {
                        foreach (var y in x.InsCards)
                        {
                            if (ongoingGameState.layoutState.cardsState[index].isSolved)
                            {
                                y.SetAsSolvedFromSavedState();
                            }
                            else if (ongoingGameState.layoutState.cardsState[index].isOpen)
                            {
                                y.SetAsOpenFromSavedState();
                                clickedSequenceOfCards.Add(y);
                            }
                            index++;
                        }
                    }
                }
                else
                {
                    ongoingGameState.SetEmptyLayoutState();
                }

                bool CheckIfSavedStateIsSameLayout()
                {
                    int index = 0;
                    foreach (var x in layoutSpawner.InsLayoutHorizontals)
                    {
                        foreach (var y in x.InsCards)
                        {
                            if (index >= ongoingGameState.layoutState.cardsState.Count)
                            {
                                Debug.LogError("index bounds layout saved state for index " + index + " name " + y.CardSprite.name);
                                return false;
                            }
                            else
                            {
                                if (y.CardSprite.name != ongoingGameState.layoutState.cardsState[index].spriteName)
                                {
                                    Debug.LogError("incorrect layout saved state for index " + index + " name " + y.CardSprite.name);
                                    return false;
                                }
                            }
                            index++;
                        }
                    }
                    return true;
                }
            }
        
        }
        SaveGameState(true);
        levelStarted = true;
    }

    void PlayerClickedShownCard(Card c)
    {
        SaveGameState(true);
    }

    void PlayerClickedHiddenCard(Card c)
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
        SaveGameState(true);
    }

    void CardFlipFinished(Card c)
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
            SaveGameState(true);
        }
    }

    void CheckForLevelCompletion()
    {
        if (levelStarted == false)
        {
            return;
        }
        if (IsLevelSolved())
        {
            ongoingGameState.layoutState.isSolved = true;
            SaveGameState(true);
            if (ienumRestartLevel != null)
            {
                StopCoroutine(ienumRestartLevel);
            }
            StartCoroutine(ienumRestartLevel = RestartLevel());
            //level finished
        }
        else
        {
            SaveGameState(true);
        }
    }

    int currentScore = 0;
    void SaveGameState(bool writeToDisk = false)
    {
        ongoingGameState.layoutState.layoutID = ongoingLayoutSO.layoutID;
        if (IsLevelSolved())
        {
            ongoingGameState.layoutState.isSolved = true;
        }
        else
        {
            ongoingGameState.layoutState.isSolved = false;
        }

        ongoingGameState.layoutState.score = currentScore;
        ongoingGameState.layoutState.cardsState = new List<GameState.CardState>();
        foreach (var x in layoutSpawner.InsLayoutHorizontals)
        {
            foreach (var y in x.InsCards)
            {
                GameState.CardState newCardState = new GameState.CardState();
                newCardState.isSolved = y.IsSolved;
                newCardState.isOpen = y.IsOpen;
                newCardState.spriteName = y.CardSprite.name;
                ongoingGameState.layoutState.cardsState.Add(newCardState);
            }
        }

        if (writeToDisk)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/" + nameFile);
            bf.Serialize(file, ongoingGameState);
            file.Close();
        }
    }

    [ContextMenu("ResetSavedData")]
    void ResetSavedData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + nameFile);
        bf.Serialize(file, new GameState());
        file.Close();
    }

    const string nameFile = "InitSpinSavedData.dat";
    GameState LoadSavedData()
    {
        if (File.Exists(Application.persistentDataPath + "/" + nameFile))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + nameFile, FileMode.Open);

            GameState data = new GameState();
            GameState l = (GameState)bf.Deserialize(file);

            if (l != null)
            {
                data = l;
            }

            file.Close();
            Debug.Log(" InitSpinSavedData loaded!");
            return data.Clone();
        }
        else
        {
            Debug.LogError("There is no saved InitSpinSavedData data!");
            return new GameState();
        }
    }

    IEnumerator ienumRestartLevel;
    IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(1f);
        LoadRandomLayout();
        SaveGameState(true);
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
}
