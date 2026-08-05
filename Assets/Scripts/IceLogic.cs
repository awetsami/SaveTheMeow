using UnityEngine;

public class IceLogic : MonoBehaviour
{
    [Header("Buz Ayarları")]
    public float maxHealth = 2f;

    private float currentHealth;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeLaserDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = Mathf.Clamp01(currentHealth / maxHealth);
            spriteRenderer.color = c;
        }

        if (currentHealth <= 0)
        {
            Debug.Log("Buz eridi!");
            Destroy(gameObject);
        }
    }
}