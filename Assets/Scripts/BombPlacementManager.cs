
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BombPlacementManager : MonoBehaviour
{
    public static BombPlacementManager Instance { get; private set; }

    [Header("Placement References")]
    public ChainExplodable bombPrefab;
    public Camera gameCamera;
    public BoxCollider2D placementZone;
    public LineRenderer radiusPreview;

    [Header("HUD")]
    public TMP_Text bombsLeftText;

    [Header("Bomb Settings")]
    [Min(1)]
    public int bombsPerLevel = 2;

    [Min(0f)]
    public float minimumBombSpacing = 0.5f;

    private List<ChainExplodable> placedBombs =
        new List<ChainExplodable>();

    private int bombsRemaining;
    private bool placementFinished = false;

    private void Awake()
    {
        Instance = this;

        if (gameCamera == null)
        {
            gameCamera = Camera.main;
        }

        if (radiusPreview != null)
        {
            radiusPreview.enabled = false;
            radiusPreview.useWorldSpace = true;
            radiusPreview.loop = true;
            radiusPreview.positionCount = 64;
        }
    }

    private void Start()
    {
        bombsRemaining = Mathf.Max(1, bombsPerLevel);
        UpdateBombHUD();
    }

    private void Update()
    {
        // วางไม่ได้หากไม่อยู่ในช่วงเลือกตำแหน่ง
        if (placementFinished ||
            CountdownTimer.Instance == null ||
            !CountdownTimer.Instance.CanPlaceBomb ||
            gameCamera == null ||
            bombPrefab == null ||
            placementZone == null)
        {
            HidePreview();
            return;
        }

        Vector3 mouseWorld = ScreenToWorld(Input.mousePosition);

        bool validSpot = IsValidSpot(mouseWorld);

        ShowPreview(mouseWorld, validSpot);

        // คลิกเมาส์ซ้ายเพื่อวางระเบิด
        if (Input.GetMouseButtonDown(0) && validSpot)
        {
            PlaceBomb(mouseWorld);
        }
    }

    private Vector3 ScreenToWorld(Vector3 screenPosition)
    {
        float cameraDistance =
            Mathf.Abs(gameCamera.transform.position.z);

        Vector3 worldPosition = gameCamera.ScreenToWorldPoint(
            new Vector3(
                screenPosition.x,
                screenPosition.y,
                cameraDistance
            )
        );

        worldPosition.z = 0f;

        return worldPosition;
    }

    private bool IsValidSpot(Vector3 worldPosition)
    {
        // ตรวจว่าเมาส์อยู่ภายใน Game View
        Vector3 mouse = Input.mousePosition;

        if (mouse.x < 0 || mouse.y < 0 ||
            mouse.x >= Screen.width ||
            mouse.y >= Screen.height)
        {
            return false;
        }

        // ไม่อนุญาตให้คลิกผ่าน UI
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return false;
        }

        // ตรวจพื้นที่ที่อนุญาตให้วาง
        if (!placementZone.OverlapPoint(
                (Vector2)worldPosition))
        {
            return false;
        }

        // ไม่ให้วางชิดระเบิดลูกเดิมเกินไป
        foreach (ChainExplodable bomb in placedBombs)
        {
            if (bomb == null)
                continue;

            float distance = Vector2.Distance(
                bomb.transform.position,
                worldPosition
            );

            if (distance < minimumBombSpacing)
            {
                return false;
            }
        }

        return true;
    }

    private void PlaceBomb(Vector3 worldPosition)
    {
        if (bombsRemaining <= 0)
            return;

        // สร้างระเบิดจาก Prefab
        ChainExplodable bomb = Instantiate(
            bombPrefab,
            worldPosition,
            Quaternion.identity
        );

        // ระเบิดผู้เล่นไม่ให้คะแนน
        bomb.destructibleType = DestructibleType.PlayerBomb;
        bomb.scoreValue = 0;

        placedBombs.Add(bomb);

        bombsRemaining--;

        UpdateBombHUD();

        // ถ้าวางครบแล้ว ให้เริ่มระเบิด
        if (bombsRemaining <= 0)
        {
            FinishPlacement();
        }
    }

    private void UpdateBombHUD()
    {
        if (bombsLeftText != null)
        {
            bombsLeftText.text =
                " x" + bombsRemaining;
        }
    }

    // เรียกเมื่อวางครบ หรือหมดเวลาวาง
    public void FinishPlacement()
    {
        if (placementFinished)
            return;

        placementFinished = true;

        HidePreview();

        if (CountdownTimer.Instance == null)
            return;

        // ถ้าไม่ได้วางเลย ถือว่าแพ้
        if (placedBombs.Count == 0)
        {
            CountdownTimer.Instance.LoseNoBombsPlaced();
            return;
        }

        // เปลี่ยนจาก Select เป็น Exploding
        if (!CountdownTimer.Instance.BeginExplosionPhase())
            return;

        // เริ่มการระเบิดทุกลูกในเฟรมเดียวกัน
        foreach (ChainExplodable bomb in placedBombs)
        {
            if (bomb != null)
            {
                bomb.Explode();
            }
        }

        // แจ้งระบบผลลัพธ์ว่าเริ่มระเบิดครบแล้ว
        if (GameResultManager.Instance != null)
        {
            GameResultManager.Instance.NotifyBombsActivated();
        }
    }

    private void ShowPreview(Vector3 center, bool validSpot)
    {
        if (radiusPreview == null || bombPrefab == null)
            return;

        radiusPreview.enabled = true;

        // สีแดง = วางได้ / สีเทา = วางไม่ได้
        Color previewColor = validSpot
            ? new Color(1f, 0.2f, 0.15f, 0.9f)
            : new Color(0.5f, 0.5f, 0.5f, 0.7f);

        radiusPreview.startColor = previewColor;
        radiusPreview.endColor = previewColor;

        float radius = Mathf.Max(
            0.05f,
            bombPrefab.explosionRadius
        );

        const int points = 64;

        for (int i = 0; i < points; i++)
        {
            float angle =
                i * 2f * Mathf.PI / points;

            float x = center.x + Mathf.Cos(angle) * radius;
            float y = center.y + Mathf.Sin(angle) * radius;

            radiusPreview.SetPosition(
                i,
                new Vector3(x, y, -0.1f)
            );
        }
    }

    private void HidePreview()
    {
        if (radiusPreview != null)
        {
            radiusPreview.enabled = false;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
