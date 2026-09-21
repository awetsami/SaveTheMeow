using UnityEngine;
using System.Collections;

public class PenMenuManager : MonoBehaviour
{
    [Header("Arayüz Elemanları")]
    public RectTransform penPanel;

    [Header("Kalem Butonları (Göster/Gizle İçin)")]
    public GameObject mirrorButton;
    public GameObject rockButton;
    public GameObject eraserButton;
    public GameObject darkGlassButton;
    public GameObject crystalButton;

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

    public void SetupAvailablePens(bool mirror, bool rock, bool eraser, bool darkGlass, bool crystal)
    {
        if (mirrorButton != null) mirrorButton.SetActive(mirror);
        if (rockButton != null) rockButton.SetActive(rock);
        if (eraserButton != null) eraserButton.SetActive(eraser);
        if (darkGlassButton != null) darkGlassButton.SetActive(darkGlass);
        if (crystalButton != null) crystalButton.SetActive(crystal);
    }

    public void TogglePenMenu()
    {
        isMenuOpen = !isMenuOpen;
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlidePanel(isMenuOpen ? visibleX : hiddenX));
    }

    public void OpenMenu()
    {
        if (!isMenuOpen)
        {
            isMenuOpen = true;
            if (slideCoroutine != null) StopCoroutine(slideCoroutine);
            slideCoroutine = StartCoroutine(SlidePanel(visibleX));
        }
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
        CheckTutorialAdvance();
        CloseMenu();
    }

    public void SelectRockPen()
    {
        if (drawManager != null) drawManager.SetPenType(1);
        CheckTutorialAdvance();
        CloseMenu();
    }

    public void SelectEraser()
    {
        if (drawManager != null) drawManager.SetPenType(2);
        CheckTutorialAdvance();
        CloseMenu();
    }

    public void SelectDarkGlassPen()
    {
        if (drawManager != null) drawManager.SetPenType(3);
        CheckTutorialAdvance();
        CloseMenu();
    }

    public void SelectCrystalPen()
    {
        if (drawManager != null) drawManager.SetPenType(4);
        CheckTutorialAdvance();
        CloseMenu();
    }

    // YENİ HALİ: Artık LevelSettings'e veya secondMessage'a ihtiyacı yok!
    private void CheckTutorialAdvance()
    {
        if (TutorialManager.Instance != null && TutorialManager.Instance.isTutorialActive)
        {
            TutorialManager.Instance.AdvanceTutorial();
        }
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