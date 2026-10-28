
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RoadCarAudio : MonoBehaviour
{
    [Header("Car Movement")]
    public RoadCarMovement carMovement;

    [Header("Engine Settings")]
    [Range(0f, 1f)]
    public float engineVolume = 0.1f;

    private AudioSource engineSource;

    private void Awake()
    {
        // หา AudioSource ของรถ
        engineSource = GetComponent<AudioSource>();

        // หา RoadCarMovement อัตโนมัติ
        if (carMovement == null)
        {
            carMovement =
                GetComponent<RoadCarMovement>();
        }

        // ตั้งค่าเสียงเครื่องยนต์
        engineSource.playOnAwake = false;
        engineSource.loop = true;
        engineSource.spatialBlend = 0f;
        engineSource.volume = engineVolume;
    }

    private void Update()
    {
        // ตรวจว่ากำลังอยู่ในช่วง Explore
        bool isExploring =
            CountdownTimer.Instance != null &&
            CountdownTimer.Instance.IsExploring;

        // ตรวจว่ารถเปิดระบบเคลื่อนที่อยู่
        bool canMove =
            carMovement != null &&
            carMovement.isActiveAndEnabled &&
            carMovement.moveSpeed > 0f;

        bool shouldPlay =
            isExploring && canMove;

        if (shouldPlay)
        {
            // เริ่มเสียงเมื่อรถวิ่ง
            if (engineSource.clip != null &&
                !engineSource.isPlaying)
            {
                engineSource.volume = engineVolume;
                engineSource.Play();
            }
        }
        else
        {
            // หยุดเสียงเมื่อรถหยุด
            if (engineSource.isPlaying)
            {
                engineSource.Stop();
            }
        }
    }

    private void OnDisable()
    {
        if (engineSource != null)
        {
            engineSource.Stop();
        }
    }

    private void OnDestroy()
    {
        if (engineSource != null)
        {
            engineSource.Stop();
        }
    }
}
