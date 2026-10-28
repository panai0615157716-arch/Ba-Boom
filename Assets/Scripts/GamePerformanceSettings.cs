
using UnityEngine;

public class GamePerformanceSettings : MonoBehaviour
{
    private void Awake()
    {
        // ให้เบราว์เซอร์จัดการจังหวะการแสดงผล
        // เหมาะกับเกมบน itch.io
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Application.targetFrameRate = -1;
        }
        else
        {
            // สำหรับ Windows และ Unity Editor
            // ใช้ VSync เพื่อให้จังหวะเฟรมสม่ำเสมอ
            QualitySettings.vSyncCount = 1;
        }
    }
}
