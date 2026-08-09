using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
public class CatLogic : MonoBehaviour
{
    [Header("Özgürlük Dedektörü (Layer Sistemli)")]
    [Tooltip("Kedinin kaçış yolunun açık olduğunu anlaması için gereken boş mesafe (Örn: 2.5 birim)")]
    public float detectorDistance = 2.5f;

    [Tooltip("Kediyi hapseden engellerin (Buz, Taş vb.) bulunduğu Layer'ları seçin.")]
    public LayerMask cageLayerMask;

    private SpriteRenderer spriteRenderer;
    private bool isFried = false;
    private bool isFreedomPathOpen = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.isGameOver || isFried) return;

        CheckEscapeRoutes();
    }

    void CheckEscapeRoutes()
    {
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

        bool foundAnyClearPath = false;

        foreach (Vector2 dir in directions)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, detectorDistance, cageLayerMask);

            if (hit.collider == null)
            {
                foundAnyClearPath = true;
                break;
            }
        }

        // YENİ: Doğrudan sayacı başlatmak yerine GameManager'a haber veriyoruz
        if (foundAnyClearPath && !isFreedomPathOpen)
        {
            isFreedomPathOpen = true;
            GameManager.Instance.CatFreed(); // <-- DEĞİŞTİ
        }
        else if (!foundAnyClearPath && isFreedomPathOpen)
        {
            isFreedomPathOpen = false;
            GameManager.Instance.CatTrapped(); // <-- DEĞİŞTİ
        }
    }

    public void GetHitByLaser(float laserPower)
    {
        if (laserPower < 0.2f) return;
        if (isFried) return;

        isFried = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerGameOver();
        }
        else
        {
            Debug.LogError("GAME MANAGER SAHNEDE BULUNAMADI! Lütfen sahneye ekleyin.");
        }

        StartCoroutine(FryCatRoutine());
    }

    IEnumerator FryCatRoutine()
    {
        spriteRenderer.color = Color.black;
        Vector3 originalPos = transform.position;
        for (int i = 0; i < 10; i++)
        {
            transform.position = originalPos + (Vector3)Random.insideUnitCircle * 0.1f;
            yield return new WaitForSeconds(0.05f);
        }
        transform.position = originalPos;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        foreach (Vector2 dir in directions)
        {
            Gizmos.DrawRay(transform.position, dir * detectorDistance);
        }
    }
}