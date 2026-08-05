using UnityEngine;
using System.Collections;

public class PenMenuManager : MonoBehaviour
{
    [Header("Arayüz Elemanları")]
    public RectTransform penPanel;

    [Header("Animasyon Ayarları")]
    public float visibleX = 0f;
    public float hiddenX = -300f;
    public float slideDuration = 0.3f;

    [Header("Sistem Bağlantısı")]
    public DrawManager drawManager;

    private bool isMenuOpen = false;
    private Coroutine slideCoroutine;

    void Start()
    {
        if (penPanel != null)
        {
            penPanel.gameObject.SetActive(true);
            penPanel.anchoredPosition = new Vector2(hiddenX, penPanel.anchoredPosition.y);
            isMenuOpen = false;
        }
    }

    public void TogglePenMenu()
    {
        isMenuOpen = !isMenuOpen;
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlidePanel(isMenuOpen ? visibleX : hiddenX));
    }

    private IEnumerator SlidePanel(float targetX)
    {
        float elapsedTime = 0f;
        Vector2 startPos = penPanel.anchoredPosition;
        Vector2 targetPos = new Vector2(targetX, startPos.y);

        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / slideDuration;
            t = t * t * (3f - 2f * t);
            penPanel.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }
        penPanel.anchoredPosition = targetPos;
    }

    // --- KALEM SEÇİM BUTONLARI ---

    public void SelectMirrorPen()
    {
        if (drawManager != null) drawManager.SetPenType(0);
        CloseMenu();
    }

    public void SelectRockPen()
    {
        if (drawManager != null) drawManager.SetPenType(1);
        CloseMenu();
    }

    public void SelectEraser()
    {
        if (drawManager != null) drawManager.SetPenType(2);
        CloseMenu();
    }

    // YENİ: Siyah Cam Kalemi (ID: 3)
    public void SelectDarkGlassPen()
    {
        if (drawManager != null) drawManager.SetPenType(3);
        CloseMenu();
    }

    // YENİ: Kristal Kalemi (ID: 4)
    public void SelectCrystalPen()
    {
        if (drawManager != null) drawManager.SetPenType(4);
        CloseMenu();
    }

    private void CloseMenu()
    {
        if (isMenuOpen)
        {
            isMenuOpen = false;
            if (slideCoroutine != null) StopCoroutine(slideCoroutine);
            slideCoroutine = StartCoroutine(SlidePanel(hiddenX));
        }
    }
}