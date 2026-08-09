using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class DrawManager : MonoBehaviour
{
    [Header("Kalem ve Sistem Şablonları")]
    public GameObject mirrorLinePrefab;     // ID: 0
    public GameObject rockLinePrefab;       // ID: 1
    public GameObject deleteNodePrefab;     // ID: 2 (Silgi için sistem prefabı)
    public GameObject darkGlassLinePrefab;  // ID: 3 (Siyah Cam)
    public GameObject crystalLinePrefab;    // ID: 4 (Kristal)

    [Header("Çizim Ayarları")]
    public float minLineLength = 0.5f;

    private int currentPenType = 0;

    private LineRenderer currentLine;
    private EdgeCollider2D currentCollider;
    private GameObject currentLineObject;
    private Vector2 startPos;

    private Vector3 originalCameraPos;

    void Start()
    {
        originalCameraPos = Camera.main.transform.position;
    }

    void Update()
    {
        bool isPressed = false;
        Vector2 pointerScreenPos = Vector2.zero;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            isPressed = true;
            pointerScreenPos = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            isPressed = true;
            pointerScreenPos = Mouse.current.position.ReadValue();
        }

        if (isPressed && currentLine == null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Vector2 pointerWorldPos = Camera.main.ScreenToWorldPoint(pointerScreenPos);

        if (currentPenType == 2)
        {
            if (isPressed) EraseLineAtPosition(pointerWorldPos);
            return;
        }

        if (isPressed && currentLine == null) StartDrawingLine(pointerWorldPos);
        else if (isPressed && currentLine != null) DragLineEnd(pointerWorldPos);
        else if (!isPressed && currentLine != null) FinishOrCancelLine();
    }

    public void SetPenType(int typeIndex)
    {
        currentPenType = typeIndex;
        ToggleDeleteNodes(currentPenType == 2);
    }

    void ToggleDeleteNodes(bool show)
    {
        ErasableLine[] allNodes = FindObjectsByType<ErasableLine>(FindObjectsSortMode.None);
        foreach (ErasableLine node in allNodes)
        {
            if (node.GetComponent<SpriteRenderer>() != null)
                node.GetComponent<SpriteRenderer>().enabled = show;

            if (node.GetComponent<Collider2D>() != null)
                node.GetComponent<Collider2D>().enabled = show;
        }
    }

    void EraseLineAtPosition(Vector2 worldPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
        if (hit.collider != null)
        {
            ErasableLine erasableScript = hit.collider.GetComponent<ErasableLine>();
            if (erasableScript != null) erasableScript.Erase();
        }
    }

    void StartDrawingLine(Vector2 worldPos)
    {
        startPos = worldPos;

        GameObject prefabToUse = mirrorLinePrefab;
        if (currentPenType == 1) prefabToUse = rockLinePrefab;
        else if (currentPenType == 3) prefabToUse = darkGlassLinePrefab;
        else if (currentPenType == 4) prefabToUse = crystalLinePrefab;

        currentLineObject = Instantiate(prefabToUse, Vector3.zero, Quaternion.identity);
        currentLine = currentLineObject.GetComponent<LineRenderer>();
        currentCollider = currentLineObject.GetComponent<EdgeCollider2D>();

        if (currentCollider != null) currentCollider.enabled = false;

        currentLine.positionCount = 2;
        currentLine.SetPosition(0, startPos);
        currentLine.SetPosition(1, startPos);
    }

    void DragLineEnd(Vector2 worldPos)
    {
        currentLine.SetPosition(1, worldPos);
    }

    void FinishOrCancelLine()
    {
        Vector2 endPos = currentLine.GetPosition(1);

        if (Vector2.Distance(startPos, endPos) < minLineLength)
        {
            Destroy(currentLineObject);
        }
        else if (IsLineIntersectingOthers(startPos, endPos))
        {
            Destroy(currentLineObject);
            StartCoroutine(ShakeCamera());
        }
        else
        {
            if (currentCollider != null)
            {
                currentCollider.points = new Vector2[] { startPos, endPos };
                currentCollider.enabled = true;

                // --- YENİ: UZUNLUK VE MALİYET HESAPLAMA SİSTEMİ ---
                float lineLength = Vector2.Distance(startPos, endPos);
                float cost = 0f;

                if (EconomyManager.Instance != null)
                {
                    cost = EconomyManager.Instance.GetLineCost(lineLength, currentPenType);
                    EconomyManager.Instance.AddCost(cost);
                }

                Vector2 centerPos = (startPos + endPos) / 2f;
                GameObject deleteNode = Instantiate(deleteNodePrefab, centerPos, Quaternion.identity);

                ErasableLine erasableScript = deleteNode.GetComponent<ErasableLine>();
                erasableScript.parentLineObject = currentLineObject;
                erasableScript.lineCost = cost; // Faturayı noktaya yazdırıyoruz

                if (currentPenType != 2)
                {
                    if (deleteNode.GetComponent<SpriteRenderer>() != null)
                        deleteNode.GetComponent<SpriteRenderer>().enabled = false;

                    if (deleteNode.GetComponent<Collider2D>() != null)
                        deleteNode.GetComponent<Collider2D>().enabled = false;
                }
            }
        }
        currentLine = null;
        currentLineObject = null;
    }

    bool IsLineIntersectingOthers(Vector2 p1, Vector2 p2)
    {
        RaycastHit2D hit = Physics2D.Linecast(p1, p2);
        if (hit.collider != null && hit.collider.gameObject != currentLineObject) return true;
        return false;
    }

    System.Collections.IEnumerator ShakeCamera()
    {
        float duration = 0.2f;
        float magnitude = 0.15f;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            Camera.main.transform.position = new Vector3(originalCameraPos.x + x, originalCameraPos.y + y, originalCameraPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Camera.main.transform.position = originalCameraPos;
    }
}