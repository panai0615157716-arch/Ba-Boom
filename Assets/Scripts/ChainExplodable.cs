
using System.Collections;
using UnityEngine;

public class ChainExplodable : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionRadius = 1.2f;
    public float explosionDelay = 0.25f;

    [Header("Score Settings")]
    public int scoreValue = 100;

    public DestructibleType destructibleType =
        DestructibleType.Barrel;

    [Header("Visual Effects")]
    public GameObject explosionEffectPrefab;
    public Sprite damagedSprite;

    private bool hasExploded = false;

    // สั่งให้วัตถุเริ่มระเบิด
    public void Explode()
    {
        // ป้องกันวัตถุระเบิดซ้ำ
        if (hasExploded)
        {
            return;
        }

        hasExploded = true;

        // เปลี่ยนเป็นภาพ Damage ถ้ามี
        SpriteRenderer sr =
            GetComponent<SpriteRenderer>();

        if (damagedSprite != null && sr != null)
        {
            sr.sprite = damagedSprite;
        }

        // แจ้งว่ามีวัตถุเริ่มระเบิด
        if (GameResultManager.Instance != null)
        {
            GameResultManager.Instance
                .RegisterExplosion();
        }

        StartCoroutine(ExplosionRoutine());
    }

    private IEnumerator ExplosionRoutine()
    {
        // หน่วงเวลาก่อนระเบิด
        yield return new WaitForSeconds(
            explosionDelay
        );

        // สร้าง Animation ระเบิดเดิม
        if (explosionEffectPrefab != null)
        {
            Instantiate(
                explosionEffectPrefab,
                transform.position,
                Quaternion.identity
            );
        }

        // เล่นเสียงระเบิด
        if (BaBoomAudioManager.Instance != null)
        {
            BaBoomAudioManager.Instance
                .PlayExplosion();
        }

        // หาวัตถุภายในรัศมีระเบิด
        Collider2D[] hits =
            Physics2D.OverlapCircleAll(
                transform.position,
                explosionRadius
            );

        foreach (Collider2D hit in hits)
        {
            ChainExplodable target =
                hit.GetComponent<ChainExplodable>();

            if (target == null || target == this)
            {
                continue;
            }

            // ตรวจระยะห่างจากจุดศูนย์กลาง
            float distance = Vector2.Distance(
                transform.position,
                target.transform.position
            );

            // ส่งระเบิดต่อภายในรัศมี
            if (distance <= explosionRadius)
            {
                target.Explode();
            }
        }

        // ระเบิดของผู้เล่นไม่ให้คะแนน
        if (destructibleType !=
            DestructibleType.PlayerBomb)
        {
            // เพิ่มคะแนนพื้นฐาน
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance
                    .AddScore(scoreValue);
            }

            // นับสถิติรถ ตึก และ Combo
            ResultDataManager.RecordDestruction(
                scoreValue,
                destructibleType
            );

            // เล่นเสียงคะแนน
            if (scoreValue > 0 &&
                BaBoomAudioManager.Instance != null)
            {
                BaBoomAudioManager.Instance
                    .PlayScore();
            }
        }

        // แจ้งว่าการระเบิดชิ้นนี้เสร็จแล้ว
        if (GameResultManager.Instance != null)
        {
            GameResultManager.Instance
                .ExplosionFinished();
        }

        // ลบวัตถุที่ถูกทำลาย
        Destroy(gameObject);
    }

    // แสดงรัศมีใน Scene View
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            explosionRadius
        );
    }
}
