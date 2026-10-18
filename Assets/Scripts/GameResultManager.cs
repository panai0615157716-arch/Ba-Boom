
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameResultManager : MonoBehaviour
{
    public static GameResultManager Instance { get; private set; }

    [Header("Win Settings")]
    public int targetScore = 600;

    [Header("Result Settings")]
    public float resultDelay = 0.8f;

    private int activeExplosions = 0;
    private bool resultStarted = false;

    private void Awake()
    {
        Instance = this;
    }

    // เรียกเมื่อวัตถุเริ่มระเบิด
    public void RegisterExplosion()
    {
        activeExplosions++;
    }

    // เรียกเมื่อวัตถุระเบิดเสร็จ
    public void ExplosionFinished()
    {
        activeExplosions--;

        if (activeExplosions <= 0 && !resultStarted)
        {
            resultStarted = true;
            StartCoroutine(CheckResult());
        }
    }

    private IEnumerator CheckResult()
    {
        // รอก่อนเปลี่ยน Scene
        yield return new WaitForSeconds(resultDelay);

        int finalScore = 0;

        if (ScoreManager.Instance != null)
        {
            finalScore = ScoreManager.Instance.CurrentScore;
        }

        if (finalScore >= targetScore)
        {
            Debug.Log("YOU WIN! Score: " + finalScore);
            SceneManager.LoadScene("Win");
        }
        else
        {
            Debug.Log("YOU LOSE! Score: " + finalScore);
            SceneManager.LoadScene("Lose");
        }
    }
}
