
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public static CountdownTimer Instance
    {
        get;
        private set;
    }

    [Header("Timer Settings")]
    public float exploreDuration = 5f;
    public float selectDuration = 5f;

    [Header("Result Settings")]
    public float loseSceneDelay = 1f;

    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text messageText;

    [Header("Countdown Sound")]
    public bool playExploreCountdown = true;
    public bool playPlacementCountdown = true;

    [Range(1, 5)]
    public int countdownStartSecond = 3;

    private enum GamePhase
    {
        Exploring,
        Selecting,
        Exploding,
        Failed
    }

    private GamePhase currentPhase;
    private float timeRemaining;

    // ใช้ตรวจการเปลี่ยนเลขวินาที
    private int lastCountdownSecond;

    // ให้ระบบวางระเบิดอ่านสถานะ
    public bool CanPlaceBomb =>
        currentPhase == GamePhase.Selecting;

    // ให้ระบบรถตรวจช่วง Explore
    public bool IsExploring =>
        currentPhase == GamePhase.Exploring;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentPhase = GamePhase.Exploring;

        timeRemaining = Mathf.Max(
            0f,
            exploreDuration
        );

        lastCountdownSecond =
            Mathf.CeilToInt(timeRemaining);

        // ไม่เพิ่มข้อความ UI ใหม่
        if (messageText != null)
        {
            messageText.text = "";
        }

        UpdateTimerUI();
    }

    private void Update()
    {
        // นับเวลาเฉพาะ Explore และ Selecting
        if (currentPhase != GamePhase.Exploring &&
            currentPhase != GamePhase.Selecting)
        {
            return;
        }

        // ลดเวลาตามเวลาจริงของเกม
        timeRemaining = Mathf.Max(
            0f,
            timeRemaining - Time.deltaTime
        );

        // ตรวจเสียงนับถอยหลัง
        CheckCountdownSound();

        // เมื่อเวลาหมด
        if (timeRemaining <= 0f)
        {
            if (currentPhase == GamePhase.Exploring)
            {
                StartPlacementPhase();
            }
            else if (currentPhase == GamePhase.Selecting)
            {
                FinishPlacementPhase();
            }
        }

        UpdateTimerUI();
    }

    // เริ่มช่วงวางระเบิด
    private void StartPlacementPhase()
    {
        currentPhase = GamePhase.Selecting;

        timeRemaining = Mathf.Max(
            0f,
            selectDuration
        );

        lastCountdownSecond =
            Mathf.CeilToInt(timeRemaining);
    }

    // หมดเวลาวางระเบิด
    private void FinishPlacementPhase()
    {
        if (BombPlacementManager.Instance != null)
        {
            BombPlacementManager.Instance.FinishPlacement();
        }
        else
        {
            LoseNoBombsPlaced();
        }
    }

    // เข้า Phase ระเบิด
    public bool BeginExplosionPhase()
    {
        if (currentPhase != GamePhase.Selecting)
        {
            return false;
        }

        currentPhase = GamePhase.Exploding;

        return true;
    }

    // ไม่มีการวางระเบิดเลย
    public void LoseNoBombsPlaced()
    {
        if (currentPhase != GamePhase.Selecting)
        {
            return;
        }

        currentPhase = GamePhase.Failed;
        timeRemaining = 0f;

        ResultDataManager.CalculateFinalScore();
        ResultDataManager.IsWin = false;

        StartCoroutine(LoadLoseScene());
    }

    private IEnumerator LoadLoseScene()
    {
        yield return new WaitForSeconds(
            Mathf.Max(0f, loseSceneDelay)
        );

        SceneManager.LoadScene("Lose");
    }

    // ตรวจเสียงนับถอยหลัง
    private void CheckCountdownSound()
    {
        int currentSecond = Mathf.CeilToInt(
            Mathf.Max(0f, timeRemaining)
        );

        // เลขวินาทียังไม่เปลี่ยน
        if (currentSecond == lastCountdownSecond)
        {
            return;
        }

        lastCountdownSecond = currentSecond;

        // เล่นเฉพาะ 3, 2, 1
        // หรือจำนวนวินาทีที่กำหนดใน Inspector
        if (currentSecond < 1 ||
            currentSecond > countdownStartSecond)
        {
            return;
        }

        bool shouldPlay = false;

        if (currentPhase == GamePhase.Exploring &&
            playExploreCountdown)
        {
            shouldPlay = true;
        }

        if (currentPhase == GamePhase.Selecting &&
            playPlacementCountdown)
        {
            shouldPlay = true;
        }

        if (shouldPlay &&
            BaBoomAudioManager.Instance != null)
        {
            BaBoomAudioManager.Instance
                .PlayCountdownTick();
        }
    }

    // อัปเดตตัวเลข Timer เดิม
    private void UpdateTimerUI()
    {
        if (timerText == null)
        {
            return;
        }

        if (currentPhase == GamePhase.Exploring ||
            currentPhase == GamePhase.Selecting ||
            currentPhase == GamePhase.Failed)
        {
            int seconds = Mathf.CeilToInt(
                Mathf.Max(0f, timeRemaining)
            );

            timerText.text = ": " + seconds;
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
