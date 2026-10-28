
using UnityEngine;

public static class ResultDataManager
{
    private const string BestScoreKey =
        "ChainArcadeBestScore";

    // สถิติการทำลาย
    public static int DestructionCount { get; private set; }
    public static int ChainCount { get; private set; }
    public static int DestructionScore { get; private set; }

    // Combo และคะแนนรวม
    public static int Multiplier { get; private set; } = 1;
    public static int ChainBonus { get; private set; }
    public static int TotalScore { get; private set; }

    // คะแนนสูงสุด
    public static int BestScore { get; private set; }

    // ผลชนะหรือแพ้
    public static bool IsWin { get; set; }

    // จำนวนวัตถุแต่ละชนิด
    public static int CarCount { get; private set; }

    public static int SmallBuildingCount
    {
        get;
        private set;
    }

    public static int LargeBuildingCount
    {
        get;
        private set;
    }

    // ชื่อด่านล่าสุด
    public static string LastPlayedScene
    {
        get;
        set;
    } = "GamePlay";

    // คะแนนประมาณการระหว่างเล่น
    public static int PreviewTotalScore
    {
        get
        {
            int multiplier = 1;

            if (ChainCount >= 10)
            {
                multiplier = 4;
            }
            else if (ChainCount >= 5)
            {
                multiplier = 2;
            }

            return DestructionScore * multiplier;
        }
    }

    // เริ่มเล่นด่านใหม่
    public static void ResetRun()
    {
        DestructionCount = 0;
        ChainCount = 0;
        DestructionScore = 0;

        CarCount = 0;
        SmallBuildingCount = 0;
        LargeBuildingCount = 0;

        Multiplier = 1;
        ChainBonus = 0;
        TotalScore = 0;

        IsWin = false;

        BestScore = PlayerPrefs.GetInt(
            BestScoreKey,
            0
        );
    }

    // รองรับการเรียกใช้รูปแบบเดิม
    public static void RecordDestruction(int score)
    {
        RecordDestruction(
            score,
            DestructibleType.Barrel
        );
    }

    // บันทึกการทำลายแต่ละวัตถุ
    public static void RecordDestruction(
        int score,
        DestructibleType type)
    {
        // ไม่นับระเบิดที่ผู้เล่นวาง
        if (type == DestructibleType.PlayerBomb)
            return;

        DestructionCount++;
        ChainCount++;

        DestructionScore += Mathf.Max(0, score);

        // แยกนับรถและตึก
        if (type == DestructibleType.Car)
        {
            CarCount++;
        }
        else if (type == DestructibleType.SmallBuilding)
        {
            SmallBuildingCount++;
        }
        else if (type == DestructibleType.LargeBuilding)
        {
            LargeBuildingCount++;
        }
    }

    // คำนวณคะแนนสุดท้าย
    public static void CalculateFinalScore()
    {
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

        ChainBonus =
            DestructionScore * (Multiplier - 1);

        TotalScore =
            DestructionScore + ChainBonus;

        BestScore = PlayerPrefs.GetInt(
            BestScoreKey,
            0
        );

        if (TotalScore > BestScore)
        {
            BestScore = TotalScore;

            PlayerPrefs.SetInt(
                BestScoreKey,
                BestScore
            );

            PlayerPrefs.Save();
        }
    }
}
