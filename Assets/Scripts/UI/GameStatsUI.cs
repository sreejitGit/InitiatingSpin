using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStatsUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI currentScoreText;

    private void OnEnable()
    {
        GameEvents.OnUpdatedCurrentScore += UpdatedCurrentScore;
    }

    private void OnDisable()
    {
        GameEvents.OnUpdatedCurrentScore -= UpdatedCurrentScore;
    }

    void UpdatedCurrentScore(int currentScore)
    {
        currentScoreText.text = $"{currentScore}";
    }

}
