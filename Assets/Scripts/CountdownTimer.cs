
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public static CountdownTimer Instance { get; private set; }

    [Header("Timer Settings")]
    public float exploreDuration = 5f;
    public float selectDuration = 5f;

    [Header("Result Settings")]
    public float loseSceneDelay = 1f;

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
        currentPhase = GamePhase.Exploring;
        timeRemaining = exploreDuration;

        if (messageText != null)
        {
            messageText.text = "";
        }

        UpdateTimerUI();
    }

    private void Update()
    {
        // นับเวลาเฉพาะช่วงสำรวจและเลือก Barrel
        if (currentPhase != GamePhase.Exploring &&
            currentPhase != GamePhase.Selecting)
        {
            return;
        }

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;

            if (currentPhase == GamePhase.Exploring)
            {
                StartSelectionPhase();
            }
            else if (currentPhase == GamePhase.Selecting)
            {
                LoseGame();
            }
        }

        UpdateTimerUI();
    }

    // เริ่มช่วงเลือกถังระเบิด 5 วินาที
    private void StartSelectionPhase()
    {
        currentPhase = GamePhase.Selecting;
        timeRemaining = selectDuration;

        if (messageText != null)
        {
            messageText.text = "";
        }
    }

    // เรียกจาก ChainExplodable เมื่อคลิกถัง
    public bool TrySelectBarrel()
    {
        if (currentPhase != GamePhase.Selecting)
            return false;

        currentPhase = GamePhase.Exploding;

        if (messageText != null)
        {
            messageText.text = "";
        }

        if (timerText != null)
        {
            timerText.text = "TIME: STOP";
        }

        return true;
    }

    // แพ้เมื่อไม่เลือก Barrel ภายในเวลา
    private void LoseGame()
    {
        currentPhase = GamePhase.Failed;

        if (messageText != null)
        {
            messageText.text = "";
        }

        // คำนวณผลลัพธ์ โดยไม่มีการทำลาย
        ResultDataManager.CalculateFinalScore();
        ResultDataManager.IsWin = false;

        StartCoroutine(LoadLoseScene());
    }

    private IEnumerator LoadLoseScene()
    {
        yield return new WaitForSeconds(loseSceneDelay);

        SceneManager.LoadScene("Lose");
    }

    private void UpdateTimerUI()
    {
        if (timerText == null)
            return;

        int seconds = Mathf.CeilToInt(
            Mathf.Max(0f, timeRemaining)
        );

        if (currentPhase == GamePhase.Exploring)
        {
            timerText.text = ": " + seconds;
        }
        else if (currentPhase == GamePhase.Selecting ||
                 currentPhase == GamePhase.Failed)
        {
            timerText.text = ": " + seconds;
        }
    }
}
