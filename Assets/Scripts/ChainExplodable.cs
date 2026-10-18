
using System.Collections;
using UnityEngine;

public class ChainExplodable : MonoBehaviour
{
    [Header("Explosion Settings")]
    public bool canBeClicked = false;
    public float explosionRadius = 2.7f;
    public float explosionDelay = 0.25f;

    [Header("Score Settings")]
    public int scoreValue = 100;

    private bool hasExploded = false;

    private void OnMouseDown()
    {
        if (!canBeClicked)
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

        // แจ้งว่ามีวัตถุเริ่มกระบวนการระเบิด
        if (GameResultManager.Instance != null)
        {
            GameResultManager.Instance.RegisterExplosion();
        }

        StartCoroutine(ExplosionRoutine());
    }

    private IEnumerator ExplosionRoutine()
    {
        yield return new WaitForSeconds(explosionDelay);

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
                target.Explode();
            }
        }

        // เพิ่มคะแนนเมื่อระเบิด
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scoreValue);
        }

        // แจ้งว่าระเบิดเสร็จแล้ว
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
