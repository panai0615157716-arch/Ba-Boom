
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultMenu : MonoBehaviour
{
    // เล่นด่านใหม่
    public void RetryGame()
    {
        SceneManager.LoadScene("GamePlay");
    }

    // กลับหน้า Main Menu
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
