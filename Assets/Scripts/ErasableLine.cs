using UnityEngine;

public class ErasableLine : MonoBehaviour
{
    public GameObject parentLineObject;

    public void Erase()
    {
        if (parentLineObject != null)
        {
            Destroy(parentLineObject);
        }
        Destroy(gameObject);
    }
}