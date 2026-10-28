
using UnityEngine;

public static class LevelProgress
{
    private const string ProgressKey =
        "BaBoom_HighestUnlockedLevel";

    private const int TotalLevels = 3;

    // ด่านสูงสุดที่ปลดล็อก
    // เริ่มครั้งแรกปลดล็อกเฉพาะด่าน 1
    public static int HighestUnlockedLevel
    {
        get
        {
            int savedLevel = PlayerPrefs.GetInt(
                ProgressKey, 1
            );

            return Mathf.Clamp(
                savedLevel, 1, TotalLevels
            );
        }
    }

    // ตรวจว่าด่านนี้เล่นได้หรือยัง
    public static bool IsLevelUnlocked(int level)
    {
        return level >= 1 &&
               level <= TotalLevels &&
               level <= HighestUnlockedLevel;
    }

    // ชนะด่านแล้วปลดล็อกด่านถัดไป
    public static void CompleteLevel(int level)
    {
        if (level < 1 || level > TotalLevels)
            return;

        // ไม่ให้ข้ามปลดล็อกด่านที่ยังเล่นไม่ได้
        if (!IsLevelUnlocked(level))
            return;

        int nextLevel = Mathf.Min(
            level + 1,
            TotalLevels
        );

        // บันทึกเฉพาะเมื่อปลดล็อกได้ไกลกว่าเดิม
        if (nextLevel > HighestUnlockedLevel)
        {
            PlayerPrefs.SetInt(
                ProgressKey,
                nextLevel
            );

            PlayerPrefs.Save();

            Debug.Log(
                "Unlocked Level " + nextLevel
            );
        }
    }

    // ระบุหมายเลขด่านจากชื่อ Scene
    public static int GetLevelNumber(string sceneName)
    {
        switch (sceneName)
        {
            case "GamePlay 1":
                return 1;

            case "GamePlay 2":
                return 2;

            case "GamePlay 3":
                return 3;

            default:
                return 0;
        }
    }

    // คืนชื่อ Scene ตามหมายเลขด่าน
    public static string GetSceneName(int level)
    {
        switch (level)
        {
            case 1:
                return "GamePlay 1";

            case 2:
                return "GamePlay 2";

            case 3:
                return "GamePlay 3";

            default:
                return "";
        }
    }

    // ใช้เฉพาะกรณีต้องเริ่มทดสอบจากด่าน 1 ใหม่
    public static void ResetProgressForTesting()
    {
        PlayerPrefs.DeleteKey(ProgressKey);
        PlayerPrefs.Save();
    }
}
