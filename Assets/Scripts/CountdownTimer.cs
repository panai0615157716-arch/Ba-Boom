
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CountdownTimer : MonoBehaviour
{
    public static CountdownTimer Instance { get; private set; }

    [Header("Timer Settings")]
    public float exploreDuration = 5f;
    public float selectDuration = 5f;

    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text messageText;

    private enum GamePhase
    {
        Exploring,
        Selecting,
        Exploding,
        Failed
    }

    private GamePhase currentPhase;
    private float timeRemaining;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // ช่วงแรก: สำรวจแผนที่
        currentPhase = GamePhase.Exploring;
        timeRemaining = exploreDuration;

        if (messageText != null)
            messageText.text = "";

        UpdateTimerUI();
    }

    private void Update()
    {
        // นับเวลาเฉพาะช่วงสำรวจและเลือกถัง
        if (currentPhase != GamePhase.Exploring &&
            currentPhase != GamePhase.Selecting)
            return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;

            if (currentPhase == GamePhase.Exploring)
            {
                // สำรวจครบ 10 วิ เริ่มช่วงเลือกถัง
                StartSelectionPhase();
            }
            else if (currentPhase == GamePhase.Selecting)
            {
                // ไม่เลือกถังภายใน 10 วิ = แพ้
                LoseGame();
            }
        }

        UpdateTimerUI();
    }

    private void StartSelectionPhase()
    {
        currentPhase = GamePhase.Selecting;
        timeRemaining = selectDuration;

        if (messageText != null)
            messageText.text = "";
    }

    // เรียกจาก Barrel เมื่อผู้เล่นคลิก
    public bool TrySelectBarrel()
    {
        // คลิกได้เฉพาะช่วงเลือกถังเท่านั้น
        if (currentPhase != GamePhase.Selecting)
            return false;

        // เลือกได้เพียงหนึ่งครั้ง
        currentPhase = GamePhase.Exploding;

        if (messageText != null)
            messageText.text = "";

        if (timerText != null)
            timerText.text = "TIME: STOP";

        return true;
    }


    private void LoseGame()
    {
        currentPhase = GamePhase.Failed;

        if (messageText != null)
        {
            messageText.text = "";
        }

        Debug.Log("Time Up! Player Lost!");

        // เปลี่ยนไป Scene แพ้
        SceneManager.LoadScene("Lose");
    }


    private void UpdateTimerUI()
    {
        if (timerText == null)
            return;

        int seconds = Mathf.CeilToInt(timeRemaining);

        if (currentPhase == GamePhase.Exploring)
        {
            timerText.text = "EXPLORE: " + seconds;
        }
        else if (currentPhase == GamePhase.Selecting ||
                 currentPhase == GamePhase.Failed)
        {
            timerText.text = "SELECT: " + seconds;
        }
    }
}
