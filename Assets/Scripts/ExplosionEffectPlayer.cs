
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ExplosionEffectPlayer : MonoBehaviour
{
    [Header("Explosion Animation")]
    public Sprite[] frames;

    [Header("Animation Speed")]
    public float framesPerSecond = 12f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        StartCoroutine(PlayExplosion());
    }

    private IEnumerator PlayExplosion()
    {
        if (frames == null || frames.Length == 0)
        {
            Destroy(gameObject);
            yield break;
        }

        float frameDelay = 1f / Mathf.Max(1f, framesPerSecond);

        for (int i = 0; i < frames.Length; i++)
        {
            spriteRenderer.sprite = frames[i];

            yield return new WaitForSeconds(frameDelay);
        }

        // ลบเฉพาะเอฟเฟกต์หลังเล่นครบทุกภาพ
        Destroy(gameObject);
    }
}
