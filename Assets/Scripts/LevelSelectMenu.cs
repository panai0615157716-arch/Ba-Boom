
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelectMenu : MonoBehaviour
{
    [Header("Level Buttons")]
    public Button level1Button;
    public Button level2Button;
    public Button level3Button;

    [Header("Level Button Text")]
    public TMP_Text level1Text;
    public TMP_Text level2Text;
    public TMP_Text level3Text;

    private void Start()
    {
        RefreshLevelButtons();
    }

    // อัปเดตสถานะปุ่มตามด่านที่ปลดล็อก
    public void RefreshLevelButtons()
    {
        UpdateLevelButton(
            level1Button,
            level1Text,
            1
        );

        UpdateLevelButton(
            level2Button,
            level2Text,
            2
        );

        UpdateLevelButton(
            level3Button,
            level3Text,
            3
        );
    }

    private void UpdateLevelButton(
        Button button,
        TMP_Text label,
        int level)
    {
        bool unlocked =
            LevelProgress.IsLevelUnlocked(level);

        // ด่านที่ล็อกจะกดไม่ได้
        if (button != null)
        {
            button.interactable = unlocked;
        }

        // แสดงข้อความล็อกหรือปลดล็อก
        if (label != null)
        {
            if (unlocked)
            {
                label.text =
                    "LEVEL " + level;
            }
            else
            {
                label.text =
                    "LEVEL " + level + "\nLOCKED";
            }
        }
    }

    // ปุ่มด่าน 1
    public void PlayLevel1()
    {
        OpenLevel(1);
    }

    // ปุ่มด่าน 2
    public void PlayLevel2()
    {
        OpenLevel(2);
    }

    // ปุ่มด่าน 3
    public void PlayLevel3()
    {
        OpenLevel(3);
    }

    private void OpenLevel(int level)
    {
        // ป้องกันการเข้าด่านที่ยังไม่ปลดล็อก
        if (!LevelProgress.IsLevelUnlocked(level))
        {
            Debug.Log(
                "Level " + level + " is locked."
            );

            return;
        }

        string sceneName =
            LevelProgress.GetSceneName(level);

        if (string.IsNullOrEmpty(sceneName))
            return;

        SceneManager.LoadScene(sceneName);
    }

    // กลับหน้า Main Menu
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
