
using UnityEngine;

public static class ResultDataManager
{
    private const string BestScoreKey = "ChainArcadeBestScore";

    // จำนวนวัตถุที่ถูกทำลาย
    public static int DestructionCount { get; private set; }

    // จำนวน Chain Reaction
    public static int ChainCount { get; private set; }

    // คะแนนพื้นฐานจากการทำลาย
    public static int DestructionScore { get; private set; }

    // ตัวคูณคะแนน
    public static int Multiplier { get; private set; } = 1;

    // คะแนนโบนัส
    public static int ChainBonus { get; private set; }

    // คะแนนรวมทั้งหมด
    public static int TotalScore { get; private set; }

    // คะแนนสูงสุด
    public static int BestScore { get; private set; }

    // สถานะชนะหรือแพ้
    public static bool IsWin { get; set; }

    // รีเซ็ตข้อมูลเมื่อเริ่มด่านใหม่
    public static void ResetRun()
    {
        DestructionCount = 0;
        ChainCount = 0;
        DestructionScore = 0;

        Multiplier = 1;
        ChainBonus = 0;
        TotalScore = 0;

        IsWin = false;

        BestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
    }

    // บันทึกคะแนนทุกครั้งที่วัตถุระเบิด
    public static void RecordDestruction(int score)
    {
        DestructionCount++;

        // นับวัตถุทุกชิ้นเป็น Chain
        ChainCount++;

        DestructionScore += Mathf.Max(0, score);
    }

    // คำนวณคะแนนเมื่อ Chain Reaction จบ
    public static void CalculateFinalScore()
    {
        // ระบบ Combo Multiplier
        if (ChainCount >= 10)
        {
            Multiplier = 4;
        }
        else if (ChainCount >= 5)
        {
            Multiplier = 2;
        }
        else
        {
            Multiplier = 1;
        }

        // คำนวณโบนัส
        ChainBonus = DestructionScore * (Multiplier - 1);

        // คำนวณคะแนนรวม
        TotalScore = DestructionScore + ChainBonus;

        // โหลดคะแนนสูงสุดที่เคยบันทึก
        BestScore = PlayerPrefs.GetInt(BestScoreKey, 0);

        // บันทึกคะแนนสูงสุดใหม่
        if (TotalScore > BestScore)
        {
            BestScore = TotalScore;

            PlayerPrefs.SetInt(BestScoreKey, BestScore);
            PlayerPrefs.Save();
        }

        Debug.Log("===== RESULT =====");
        Debug.Log("Destruction: " + DestructionCount);
        Debug.Log("Chain: " + ChainCount);
        Debug.Log("Multiplier: x" + Multiplier);
        Debug.Log("Base Score: " + DestructionScore);
        Debug.Log("Chain Bonus: " + ChainBonus);
        Debug.Log("Total Score: " + TotalScore);
        Debug.Log("Best Score: " + BestScore);
    }
}
