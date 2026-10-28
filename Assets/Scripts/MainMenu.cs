
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    private void Awake()
    {
        // คืนเวลาของเกมเป็นปกติ
        Time.timeScale = 1f;

        // ปลดล็อกเมาส์
        Cursor.lockState = CursorLockMode.None;

        // แสดงเมาส์
        Cursor.visible = true;
    }

    private void Start()
    {
        // ตรวจสอบ EventSystem
        if (EventSystem.current == null)
        {
            Debug.LogWarning(
                "MAIN MENU: EventSystem not found!"
            );
        }
        else
        {
            Debug.Log(
                "MAIN MENU: EventSystem found."
            );

            // ล้างการเลือกปุ่มที่ค้างอยู่
            EventSystem.current.SetSelectedGameObject(null);
        }

        Debug.Log("MAIN MENU: Ready.");
    }

    // ปุ่ม PLAY
    public void PlayGame()
    {
        Debug.Log("MAIN MENU: Play button clicked!");

        SceneManager.LoadScene("LevelSelect");
    }

    // รองรับปุ่มที่เคยใช้ชื่อ Play
    public void Play()
    {
        PlayGame();
    }

    // ปุ่ม QUIT
    public void QuitGame()
    {
        Debug.Log("MAIN MENU: Quit button clicked!");

        Application.Quit();
    }

    // รองรับชื่อเมธอดเดิม
    public void Quit()
    {
        QuitGame();
    }
}
