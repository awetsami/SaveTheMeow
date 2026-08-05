using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class LaserLogic : MonoBehaviour
{
    [Header("Lazer Ayarları")]
    public int maxBounces = 25;
    public float maxDistance = 50f;
    public float iceRefractionAngle = 18f;
    public float iceThicknessOffset = 0.3f;

    [Header("Renk Ayarları")]
    public Color normalColor = Color.red;
    public Color darkColor = new Color(0.4f, 0f, 0f, 1f); // Koyu Kırmızı / Bordo

    [Header("Animasyon (Yavaş İlerleme)")]
    public float laserSpeed = 30f;

    [Header("Fizik ve Çarpışma")]
    [Tooltip("Lazerin ÇARPACAĞI katmanları seçin.")]
    public LayerMask interactableLayers;

    [Header("Kristal ve Cam Sistemi")]
    public int maxDepth = 4; // Lazer en fazla 4 kere bölünebilir/filtrelenebilir
    [HideInInspector] public int currentDepth = 0;
    [HideInInspector] public bool isChild = false;
    [HideInInspector] public Vector2 customStartPosition;
    [HideInInspector] public Vector2 customStartDirection;
    [HideInInspector] public Color currentColor;

    private LineRenderer lineRenderer;
    private List<Vector2> calculatedPath = new List<Vector2>();
    private List<Vector3> visualPath = new List<Vector3>();

    private List<LaserLogic> childLasers = new List<LaserLogic>();

    // Lazerin ne tür bir engelden doğduğunu ayırıyoruz
    public enum BranchType { Crystal, DarkGlass }

    struct BranchPoint
    {
        public Vector2 pos;
        public Vector2 dir;
        public BranchType bType;
    }
    private List<BranchPoint> activeBranches = new List<BranchPoint>();

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // Eğer bu ana lazerse (çocuk değilse), normal renkle başla
        if (!isChild)
        {
            SetColor(normalColor);
        }
    }

    // Lazere istediğimiz rengi veren fonksiyon
    public void SetColor(Color c)
    {
        currentColor = c;
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = c;
        lineRenderer.endColor = c;
    }

    void Update()
    {
        CalculateLaserPath();
        AnimateLaser();
        ApplyPhysicalDamage();
        ManageBranches();
    }

    void CalculateLaserPath()
    {
        calculatedPath.Clear();
        activeBranches.Clear();

        Vector2 currentPosition = isChild ? customStartPosition : (Vector2)transform.position;
        Vector2 currentDirection = isChild ? customStartDirection : Vector2.down;

        calculatedPath.Add(currentPosition);

        for (int i = 0; i < maxBounces; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentPosition + (currentDirection * 0.01f), currentDirection, maxDistance, interactableLayers);

            if (hit.collider != null)
            {
                calculatedPath.Add(hit.point);
                string hitTag = hit.collider.tag;

                if (hitTag == "Cat" || hitTag == "Rock")
                {
                    break;
                }
                else if (hitTag == "Ice")
                {
                    float angleToNormal = Vector2.SignedAngle(currentDirection, hit.normal);
                    float deviation = (angleToNormal > 0) ? iceRefractionAngle : -iceRefractionAngle;
                    currentDirection = Quaternion.Euler(0, 0, deviation) * currentDirection;
                    currentPosition = hit.point + (currentDirection * iceThicknessOffset);
                }
                else if (hitTag == "DarkGlass")
                {
                    // YENİ SİYAH CAM MANTIĞI: Ana lazer camın yüzeyinde durur. İçinden "Karanlık" kopyasını fırlatır.
                    if (currentDepth < maxDepth)
                    {
                        activeBranches.Add(new BranchPoint
                        {
                            pos = hit.point + (currentDirection * 0.15f),
                            dir = currentDirection,
                            bType = BranchType.DarkGlass
                        });
                    }
                    break; // Ana lazerin çizimini camda sonlandır
                }
                else if (hitTag == "Crystal")
                {
                    // KRİSTAL MANTIĞI: Ana gövde devam eder, yanlara kopya fırlatır
                    if (currentDepth < maxDepth)
                    {
                        activeBranches.Add(new BranchPoint { pos = hit.point + (currentDirection * 0.15f), dir = Quaternion.Euler(0, 0, 45) * currentDirection, bType = BranchType.Crystal });
                        activeBranches.Add(new BranchPoint { pos = hit.point + (currentDirection * 0.15f), dir = Quaternion.Euler(0, 0, -45) * currentDirection, bType = BranchType.Crystal });
                    }
                    currentPosition = hit.point + (currentDirection * 0.15f);
                }
                else if (hitTag == "Mirror")
                {
                    Vector2 surfaceNormal = hit.normal;
                    if (Vector2.Dot(currentDirection, surfaceNormal) > 0) surfaceNormal = -surfaceNormal;
                    currentDirection = Vector2.Reflect(currentDirection, surfaceNormal);
                    currentPosition = hit.point + (currentDirection * 0.05f);
                }
                else
                {
                    break;
                }
            }
            else
            {
                calculatedPath.Add(currentPosition + (currentDirection * maxDistance));
                break;
            }
        }
    }

    void AnimateLaser()
    {
        if (calculatedPath.Count == 0) return;

        if (visualPath.Count == 0)
        {
            visualPath.Add(calculatedPath[0]);
            visualPath.Add(calculatedPath[0]);
        }

        visualPath[0] = calculatedPath[0];

        bool pathDiverged = false;
        for (int i = 0; i < visualPath.Count - 1; i++)
        {
            if (i >= calculatedPath.Count || Vector2.Distance(visualPath[i], calculatedPath[i]) > 0.01f)
            {
                visualPath.RemoveRange(i, visualPath.Count - i);
                if (visualPath.Count > 0) visualPath.Add(visualPath[visualPath.Count - 1]);
                pathDiverged = true;
                break;
            }
        }

        if (!pathDiverged && visualPath.Count <= calculatedPath.Count)
        {
            int tipIndex = visualPath.Count - 1;
            if (tipIndex > 0)
            {
                Vector3 previousAnchor = visualPath[tipIndex - 1];
                Vector3 currentTip = visualPath[tipIndex];
                Vector3 targetPoint = calculatedPath[tipIndex];

                float distToTarget = Vector2.Distance(previousAnchor, targetPoint);
                float distToTip = Vector2.Distance(previousAnchor, currentTip);

                if (distToTip > distToTarget)
                {
                    visualPath[tipIndex] = targetPoint;
                }
                else if (distToTip > 0.05f)
                {
                    Vector2 expectedDir = (targetPoint - previousAnchor).normalized;
                    Vector2 currentDir = (currentTip - previousAnchor).normalized;
                    if (Vector2.Dot(expectedDir, currentDir) < 0.99f)
                    {
                        visualPath[tipIndex] = previousAnchor;
                    }
                }
            }
        }

        if (visualPath.Count > 0)
        {
            int tipIndex = visualPath.Count - 1;
            if (tipIndex < calculatedPath.Count)
            {
                Vector3 currentTip = visualPath[tipIndex];
                Vector3 targetPoint = calculatedPath[calculatedPath.Count == visualPath.Count ? tipIndex : tipIndex];

                Vector3 newTipPos = Vector3.MoveTowards(currentTip, targetPoint, laserSpeed * Time.deltaTime);
                visualPath[tipIndex] = newTipPos;

                if (Vector2.Distance(newTipPos, targetPoint) < 0.01f)
                {
                    visualPath[tipIndex] = targetPoint;
                    if (tipIndex + 1 < calculatedPath.Count)
                    {
                        visualPath.Add(targetPoint);
                    }
                }
            }
        }

        lineRenderer.positionCount = visualPath.Count;
        lineRenderer.SetPositions(visualPath.ToArray());
    }

    void ApplyPhysicalDamage()
    {
        if (visualPath.Count < 2) return;

        HashSet<IceLogic> damagedIceThisFrame = new HashSet<IceLogic>();

        for (int i = 0; i < visualPath.Count - 1; i++)
        {
            Vector2 startPoint = visualPath[i];
            Vector2 endPoint = visualPath[i + 1];

            RaycastHit2D[] hits = Physics2D.LinecastAll(startPoint, endPoint, interactableLayers);

            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Ice"))
                {
                    IceLogic iceScript = hit.collider.GetComponent<IceLogic>();
                    if (iceScript != null && !damagedIceThisFrame.Contains(iceScript))
                    {
                        iceScript.TakeLaserDamage(Time.deltaTime * 1f);
                        damagedIceThisFrame.Add(iceScript);
                    }
                }
                else if (hit.collider.CompareTag("Cat"))
                {
                    CatLogic catScript = hit.collider.GetComponent<CatLogic>();
                    if (catScript != null) catScript.GetHitByLaser(1f);
                }
            }
        }
    }

    void ManageBranches()
    {
        while (childLasers.Count < activeBranches.Count)
        {
            GameObject childObj = Instantiate(gameObject, transform.position, Quaternion.identity, transform);
            LaserLogic childLogic = childObj.GetComponent<LaserLogic>();
            childLogic.ClearVisualPath();
            childLasers.Add(childLogic);
        }

        for (int i = 0; i < childLasers.Count; i++)
        {
            if (i < activeBranches.Count)
            {
                bool reached = false;
                for (int v = 0; v < visualPath.Count; v++)
                {
                    if (Vector2.Distance(visualPath[v], activeBranches[i].pos) < 0.2f)
                    {
                        reached = true;
                        break;
                    }
                }

                if (reached)
                {
                    childLasers[i].gameObject.SetActive(true);
                    childLasers[i].isChild = true;
                    childLasers[i].currentDepth = currentDepth + 1;
                    childLasers[i].customStartPosition = activeBranches[i].pos;
                    childLasers[i].customStartDirection = activeBranches[i].dir;

                    // YENİ: Kopan dalın rengini kaynağına göre ayarla
                    if (activeBranches[i].bType == BranchType.DarkGlass)
                    {
                        // Siyah camdan çıkan lazer kararır
                        childLasers[i].SetColor(darkColor);
                    }
                    else
                    {
                        // Kristalden çıkan lazer mevcut rengini (Açıksa açık, koyuysa koyu) korur
                        childLasers[i].SetColor(this.currentColor);
                    }
                }
                else
                {
                    childLasers[i].gameObject.SetActive(false);
                    childLasers[i].ClearVisualPath();
                }
            }
            else
            {
                childLasers[i].gameObject.SetActive(false);
                childLasers[i].ClearVisualPath();
            }
        }
    }

    public void ClearVisualPath()
    {
        visualPath.Clear();
        if (lineRenderer != null) lineRenderer.positionCount = 0;
    }
}