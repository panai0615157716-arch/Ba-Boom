
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameResultManager : MonoBehaviour
{
    public static GameResultManager Instance { get; private set; }

    [Header("Win Settings")]
    public int targetScore = 600;

    [Header("Result Settings")]
    public float resultDelay = 1.5f;

    private int activeExplosions = 0;
    private bool chainStarted = false;
    private bool resultStarted = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // รีเซ็ตคะแนนของด่านใหม่
        ResultDataManager.ResetRun();
    }

    // เรียกเมื่อมีวัตถุเริ่มระเบิด
    public void RegisterExplosion()
    {
        if (resultStarted)
            return;

        chainStarted = true;
        activeExplosions++;
    }

    // เรียกเมื่อวัตถุระเบิดเสร็จ
    public void ExplosionFinished()
    {
        if (!chainStarted || resultStarted)
            return;

        activeExplosions = Mathf.Max(0, activeExplosions - 1);

        // ระเบิดทั้งหมดจบแล้ว
        if (activeExplosions == 0)
        {
            resultStarted = true;
            StartCoroutine(FinishRound());
        }
    }

    private IEnumerator FinishRound()
    {
        // รอเล็กน้อยก่อนแสดงผลลัพธ์
        yield return new WaitForSeconds(resultDelay);

        // คำนวณ Chain และ Multiplier
        ResultDataManager.CalculateFinalScore();

        int totalScore = ResultDataManager.TotalScore;

        // ตรวจผลชนะหรือแพ้จากคะแนนรวม
        bool win = totalScore >= targetScore;

        ResultDataManager.IsWin = win;

        Debug.Log(
            "Destruction: " +
            ResultDataManager.DestructionScore +
            " | Chain: " +
            ResultDataManager.ChainCount +
            " | Multiplier: x" +
            ResultDataManager.Multiplier +
            " | Total: " + totalScore
        );

        if (win)
        {
            SceneManager.LoadScene("Win");
        }
        else
        {
            SceneManager.LoadScene("Lose");
        }
    }
}
