
using System.Collections;
using UnityEngine;

public class ChainExplodable : MonoBehaviour
{
    [Header("Explosion Settings")]
    public bool canBeClicked = false;
    public float explosionRadius = 1.2f;
    public float explosionDelay = 0.25f;

    [Header("Score Settings")]
    public int scoreValue = 100;

    [Header("Visual Effects")]
    public GameObject explosionEffectPrefab;
    public Sprite damagedSprite;

    private bool hasExploded = false;

    private void OnMouseDown()
    {
        if (!canBeClicked || hasExploded)
            return;

        if (CountdownTimer.Instance == null)
            return;

        if (CountdownTimer.Instance.TrySelectBarrel())
        {
            Explode();
        }
    }

    public void Explode()
    {
        if (hasExploded)
            return;

        hasExploded = true;

        // เปลี่ยนเป็นภาพเสียหาย ถ้ามีการกำหนดไว้
        if (damagedSprite != null)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();

            if (sr != null)
            {
                sr.sprite = damagedSprite;
            }
        }

        // แจ้งระบบตรวจผลว่ามีวัตถุเริ่มระเบิด
        if (GameResultManager.Instance != null)
        {
            GameResultManager.Instance.RegisterExplosion();
        }

        StartCoroutine(ExplosionRoutine());
    }

    private IEnumerator ExplosionRoutine()
    {
        yield return new WaitForSeconds(explosionDelay);

        // สร้างเอฟเฟกต์ระเบิด
        if (explosionEffectPrefab != null)
        {
            Instantiate(
                explosionEffectPrefab,
                transform.position,
                Quaternion.identity
            );
        }

        // ตรวจวัตถุในรัศมีระเบิด
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            explosionRadius
        );

        foreach (Collider2D hit in hits)
        {
            ChainExplodable target =
                hit.GetComponent<ChainExplodable>();

            if (target != null && target != this)
            {
                float distance = Vector2.Distance(
                    (Vector2)transform.position,
                    (Vector2)target.transform.position
                );

                if (distance <= explosionRadius)
                {
                    target.Explode();
                }
            }
        }

        // เพิ่มคะแนนพื้นฐาน
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scoreValue);
        }

        // เก็บข้อมูล Combo และคะแนน
        ResultDataManager.RecordDestruction(scoreValue);

        // แจ้งว่าวัตถุระเบิดเสร็จ
        if (GameResultManager.Instance != null)
        {
            GameResultManager.Instance.ExplosionFinished();
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            explosionRadius
        );
    }
}
