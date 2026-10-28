
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultMenu : MonoBehaviour
{
    // เล่นด่านเดิมอีกครั้ง
    public void RetryGame()
    {
        string lastScene =
            ResultDataManager.LastPlayedScene;

        // ตรวจว่าเป็นด่านที่ถูกต้องหรือไม่
        int level = LevelProgress.GetLevelNumber(lastScene);

        if (level >= 1 && level <= 3)
        {
            SceneManager.LoadScene(lastScene);
        }
        else
        {
            // หากไม่พบด่านเดิม กลับหน้าเลือกด่าน
            SceneManager.LoadScene("LevelSelect");
        }
    }

    // กลับหน้า Main Menu
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // กลับหน้าเลือกด่าน
    public void BackToLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    // ไปด่านถัดไป
    public void NextLevel()
    {
        string lastScene =
            ResultDataManager.LastPlayedScene;

        int currentLevel =
            LevelProgress.GetLevelNumber(lastScene);

        int nextLevel = currentLevel + 1;

        if (nextLevel >= 2 &&
            nextLevel <= 3 &&
            LevelProgress.IsLevelUnlocked(nextLevel))
        {
            string nextScene =
                LevelProgress.GetSceneName(nextLevel);

            SceneManager.LoadScene(nextScene);
        }
        else
        {
            // ด่านสุดท้ายหรือยังไม่ได้ปลดล็อก
            SceneManager.LoadScene("LevelSelect");
        }
    }
}
