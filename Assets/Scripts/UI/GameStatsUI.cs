using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStatsUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI currentScoreText;
    [SerializeField] TextMeshProUGUI currentMatchesText;
    [SerializeField] TextMeshProUGUI currentTriesText;

    private void OnEnable()
    {
        GameEvents.OnUpdatedCurrentScore += UpdatedCurrentScore;
        GameEvents.OnUpdatedCurrentMatches += UpdatedCurrentMatches;
        GameEvents.OnUpdatedCurrentTries += UpdatedCurrentTries;
    }

    private void OnDisable()
    {
        GameEvents.OnUpdatedCurrentScore -= UpdatedCurrentScore;
        GameEvents.OnUpdatedCurrentMatches -= UpdatedCurrentMatches;
        GameEvents.OnUpdatedCurrentTries -= UpdatedCurrentTries;
    }

    void UpdatedCurrentScore(int currentScore)
    {
        currentScoreText.text = $"{currentScore}";
    }

    void UpdatedCurrentMatches(int matches)
    {
        currentMatchesText.text = $"{matches}";
    }

    void UpdatedCurrentTries(int tries)
    {
        currentTriesText.text = $"{tries}";
    }

}
