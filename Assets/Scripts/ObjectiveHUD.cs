
using UnityEngine;
using TMPro;

public class ObjectiveHUD : MonoBehaviour
{
    [Header("Objective UI")]
    public TMP_Text scoreObjectiveText;
    public TMP_Text largeBuildingObjectiveText;
    public TMP_Text carObjectiveText;

    [Header("Text Colors")]
    public Color normalColor = Color.white;
    public Color completedColor = Color.green;

    private void Update()
    {
        // ตรวจว่ามี GameResultManager หรือไม่
        if (GameResultManager.Instance == null)
        {
            return;
        }

        // อ่านเป้าหมายของด่าน
        int targetScore =
            GameResultManager.Instance.targetScore;

        int targetBuildings =
            GameResultManager.Instance.requiredLargeBuildings;

        int targetCars =
            GameResultManager.Instance.requiredCars;

        // อ่านผลการเล่นปัจจุบัน
        int currentScore =
            ResultDataManager.PreviewTotalScore;

        int currentBuildings =
            ResultDataManager.LargeBuildingCount;

        int currentCars =
            ResultDataManager.CarCount;

        // แสดงคะแนน
        if (scoreObjectiveText != null)
        {
            scoreObjectiveText.text =
                " " +
                currentScore.ToString("N0") +
                " / " +
                targetScore.ToString("N0");

            scoreObjectiveText.color =
                currentScore >= targetScore
                ? completedColor
                : normalColor;
        }

        // แสดงจำนวนตึกใหญ่
        if (largeBuildingObjectiveText != null)
        {
            largeBuildingObjectiveText.text =
                " " +
                currentBuildings +
                " / " +
                targetBuildings;

            largeBuildingObjectiveText.color =
                currentBuildings >= targetBuildings
                ? completedColor
                : normalColor;
        }

        // แสดงจำนวนรถ
        if (carObjectiveText != null)
        {
            carObjectiveText.text =
                " " +
                currentCars +
                " / " +
                targetCars;

            carObjectiveText.color =
                currentCars >= targetCars
                ? completedColor
                : normalColor;
        }
    }
}
