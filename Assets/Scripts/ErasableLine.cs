using UnityEngine;

public class ErasableLine : MonoBehaviour
{
    public GameObject parentLineObject;

    [HideInInspector]
    public float lineCost; // YENİ: Bu çizginin maliyetini hafızada tutar

    public void Erase()
    {
        // Çizgi silinince parayı iade et
        if (EconomyManager.Instance != null)
        {
            EconomyManager.Instance.RefundCost(lineCost);
        }

        if (parentLineObject != null)
        {
            Destroy(parentLineObject);
        }
        Destroy(gameObject);
    }
}