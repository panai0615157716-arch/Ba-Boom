
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameResultManager : MonoBehaviour
{
    public static GameResultManager Instance { get; private set; }

    [Header("Level Objectives")]
    public int targetScore = 600;
    public int requiredLargeBuildings = 0;
    public int requiredCars = 0;

    [Header("Result Settings")]
    public float resultDelay = 1.5f;

    private int activeExplosions = 0;

    private bool bombsActivated = false;
    private bool resultStarted = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // รีเซ็ตข้อมูลเมื่อเริ่มด่าน
        ResultDataManager.ResetRun();

        // จำชื่อด่านที่กำลังเล่น
        ResultDataManager.LastPlayedScene =
            SceneManager.GetActiveScene().name;
    }

    // แจ้งเมื่อวัตถุเริ่มระเบิด
    public void RegisterExplosion()
    {
        if (resultStarted)
            return;

        activeExplosions++;
    }

    // เรียกหลังระเบิดที่วางไว้ทุกลูกเริ่มทำงานแล้ว
    public void NotifyBombsActivated()
    {
        bombsActivated = true;

        TryFinishRound();
    }

    // แจ้งเมื่อวัตถุระเบิดเสร็จ
    public void ExplosionFinished()
    {
        if (resultStarted)
            return;

        activeExplosions = Mathf.Max(
            0,
            activeExplosions - 1
        );

        TryFinishRound();
    }

    private void TryFinishRound()
    {
        // ยังไม่เริ่มระเบิดครบทุกลูก
        if (!bombsActivated)
            return;

        // ยังมีวัตถุกำลังระเบิด
        if (activeExplosions > 0)
            return;

        // ป้องกันการตรวจผลซ้ำ
        if (resultStarted)
            return;

        resultStarted = true;

        StartCoroutine(FinishRound());
    }

    private IEnumerator FinishRound()
    {
        yield return new WaitForSeconds(resultDelay);

        // คำนวณคะแนนและ Combo
        ResultDataManager.CalculateFinalScore();

        // ตรวจเงื่อนไขผ่านด่าน
        bool scorePassed =
            ResultDataManager.TotalScore >= targetScore;

        bool buildingPassed =
            ResultDataManager.LargeBuildingCount >=
            requiredLargeBuildings;

        bool carPassed =
            ResultDataManager.CarCount >= requiredCars;

        bool win =
            scorePassed &&
            buildingPassed &&
            carPassed;

        ResultDataManager.IsWin = win;


        if (win)
        {
            // ตรวจว่าผู้เล่นชนะด่านไหน
            int completedLevel = LevelProgress.GetLevelNumber(
                SceneManager.GetActiveScene().name
            );

            // บันทึกการปลดล็อกด่านถัดไป
            LevelProgress.CompleteLevel(completedLevel);

            // เปิดหน้า Win เหมือนเดิม
            SceneManager.LoadScene("Win");
        }
        else
        {
            // แพ้จะไม่ปลดล็อกด่าน
            SceneManager.LoadScene("Lose");
        }

    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
