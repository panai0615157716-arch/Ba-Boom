
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Score UI")]
    public TMP_Text scoreText;

    private int currentScore = 0;
    public int CurrentScore => currentScore;
    private void Awake()
    {
        Instance = this;
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = ": " + currentScore;
        }
    }
}
