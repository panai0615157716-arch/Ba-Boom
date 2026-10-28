
using UnityEngine;

public class RoadCarMovement : MonoBehaviour
{
    [Header("Road Route")]
    public Transform[] routePoints;

    [Min(0f)]
    public float moveSpeed = 1.5f;

    public bool moveOnlyDuringExplore = true;
    public bool pingPong = true;

    [Header("Car Visual")]
    public SpriteRenderer carSprite;

    public bool autoFlipHorizontal = true;
    public bool spriteFacesRight = false;

    private int targetIndex = 1;
    private int travelDirection = 1;

    private void Awake()
    {
        // หาภาพรถอัตโนมัติ หากยังไม่ได้ลากใส่
        if (carSprite == null)
        {
            carSprite = GetComponentInChildren<SpriteRenderer>();
        }
    }


    private void Start()
    {
        // ตรวจว่ามีจุดเส้นทางอย่างน้อย 2 จุด
        if (routePoints == null || routePoints.Length < 2)
        {
            Debug.LogWarning(
                "RoadCarMovement: ต้องมี Route Points อย่างน้อย 2 จุด",
                this
            );

            enabled = false;
            return;
        }

        // ตรวจว่าทุกจุดถูกเชื่อมแล้ว
        for (int i = 0; i < routePoints.Length; i++)
        {
            if (routePoints[i] == null)
            {
                Debug.LogWarning(
                    "RoadCarMovement: Route Point ยังไม่ได้กำหนด",
                    this
                );

                enabled = false;
                return;
            }
        }

        // ให้รถเริ่มวิ่งจากตำแหน่งปัจจุบัน
        // โดยไม่ย้ายรถไป PointA ทันที
        targetIndex = 0;
        travelDirection = 1;
    }


    private void Update()
    {
        // ให้รถเคลื่อนที่เฉพาะช่วง Explore
        if (moveOnlyDuringExplore)
        {
            if (CountdownTimer.Instance == null ||
                !CountdownTimer.Instance.IsExploring)
            {
                return;
            }
        }

        if (moveSpeed <= 0f)
            return;

        // อ่านตำแหน่งจุดหมาย
        Vector3 point = routePoints[targetIndex].position;

        Vector3 destination = new Vector3(
            point.x,
            point.y,
            transform.position.z
        );

        Vector3 direction = destination - transform.position;

        // หันภาพรถตามทิศทางแนวนอน
        if (autoFlipHorizontal &&
            carSprite != null &&
            Mathf.Abs(direction.x) > 0.01f &&
            Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
        {
            bool movingRight = direction.x > 0f;

            carSprite.flipX =
                movingRight != spriteFacesRight;
        }

        // เคลื่อนที่ไปยังจุดหมาย
        transform.position = Vector3.MoveTowards(
            transform.position,
            destination,
            moveSpeed * Time.deltaTime
        );

        // ถึงจุดหมายแล้ว ไปจุดถัดไป
        if (Vector3.Distance(
            transform.position,
            destination) <= 0.01f)
        {
            NextPoint();
        }
    }

    private void NextPoint()
    {
        if (pingPong)
        {
            // วิ่งไปสุดทางแล้ววิ่งย้อนกลับ
            if (targetIndex >= routePoints.Length - 1)
            {
                travelDirection = -1;
            }
            else if (targetIndex <= 0)
            {
                travelDirection = 1;
            }

            targetIndex += travelDirection;
        }
        else
        {
            // วิ่งไปตามจุดเรียงลำดับ แล้ววนกลับจุดแรก
            targetIndex =
                (targetIndex + 1) % routePoints.Length;
        }
    }

    // แสดงเส้นทางใน Scene View เมื่อเลือกรถ
    private void OnDrawGizmosSelected()
    {
        if (routePoints == null || routePoints.Length < 2)
            return;

        Gizmos.color = Color.cyan;

        for (int i = 0; i < routePoints.Length; i++)
        {
            if (routePoints[i] == null)
                continue;

            Gizmos.DrawWireSphere(
                routePoints[i].position,
                0.15f
            );

            if (i < routePoints.Length - 1 &&
                routePoints[i + 1] != null)
            {
                Gizmos.DrawLine(
                    routePoints[i].position,
                    routePoints[i + 1].position
                );
            }
        }

        if (!pingPong &&
            routePoints.Length > 2 &&
            routePoints[0] != null &&
            routePoints[routePoints.Length - 1] != null)
        {
            Gizmos.DrawLine(
                routePoints[routePoints.Length - 1].position,
                routePoints[0].position
            );
        }
    }
}
