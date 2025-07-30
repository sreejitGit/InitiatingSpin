using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStatsUI : BaseScreenUI
{
    [SerializeField] TextMeshProUGUI currentScoreText;
    [SerializeField] TextMeshProUGUI currentMatchesText;
    [SerializeField] TextMeshProUGUI currentTriesText;

    protected override void OnEnable()
    {
        GameEvents.OnUpdatedCurrentScore += UpdatedCurrentScore;
        GameEvents.OnUpdatedCurrentMatches += UpdatedCurrentMatches;
        GameEvents.OnUpdatedCurrentTries += UpdatedCurrentTries;
    }

    protected override void OnDisable()
    {
        GameEvents.OnUpdatedCurrentScore -= UpdatedCurrentScore;
        GameEvents.OnUpdatedCurrentMatches -= UpdatedCurrentMatches;
        GameEvents.OnUpdatedCurrentTries -= UpdatedCurrentTries;
    }

    public override void Show()
    {
        base.Show();
    }

    public override void Hide()
    {
        base.Hide();
    }

    public void EnterHomeClicked()
    {
        GameEvents.EnterHome();
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
