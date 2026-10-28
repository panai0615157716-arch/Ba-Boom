
using UnityEngine;
using TMPro;

public class ResultSummaryUI : MonoBehaviour
{
    [Header("Result Title")]
    public TMP_Text titleText;

    [Header("Main Score")]
    public TMP_Text artScoreText;

    [Header("Statistic Cards")]
    public TMP_Text chainReactionText;
    public TMP_Text multiplierText;
    public TMP_Text destructionText;
    public TMP_Text bestScoreText;

    [Header("Score Breakdown")]
    public TMP_Text destructionScoreText;
    public TMP_Text chainBonusText;
    public TMP_Text totalArtScoreText;

    private void Start()
    {
        ShowResults();
    }

    // แสดงสถิติทั้งหมด
    public void ShowResults()
    {
        // แสดงชื่อผลลัพธ์
        if (titleText != null)
        {
            if (ResultDataManager.IsWin)
            {
                titleText.text = "";
            }
            else
            {
                titleText.text = "";
            }
        }

        // คะแนนรวมหลัก
        if (artScoreText != null)
        {
            artScoreText.text =
                ResultDataManager.TotalScore.ToString("N0");
        }

        // จำนวน Chain Reaction
        if (chainReactionText != null)
        {
            chainReactionText.text =
                "x" + ResultDataManager.ChainCount;
        }

        // ตัวคูณคะแนน
        if (multiplierText != null)
        {
            multiplierText.text =
                "x" + ResultDataManager.Multiplier;
        }

        // จำนวนวัตถุที่ถูกทำลาย
        if (destructionText != null)
        {
            destructionText.text =
                ResultDataManager.DestructionCount.ToString();
        }

        // คะแนนสูงสุด
        if (bestScoreText != null)
        {
            bestScoreText.text =
                ResultDataManager.BestScore.ToString("N0");
        }

        // คะแนนพื้นฐาน
        if (destructionScoreText != null)
        {
            destructionScoreText.text =
                ResultDataManager.DestructionScore.ToString("N0");
        }

        // คะแนนโบนัสจาก Combo
        if (chainBonusText != null)
        {
            chainBonusText.text =
                "+" + ResultDataManager.ChainBonus.ToString("N0");
        }

        // คะแนนรวมสุดท้าย
        if (totalArtScoreText != null)
        {
            totalArtScoreText.text =
                ResultDataManager.TotalScore.ToString("N0");
        }
    }
}
