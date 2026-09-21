using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Öğretici Arayüzü")]
    public GameObject tutorialOverlay;
    public TextMeshProUGUI tutorialText;

    [Header("Sistem Bağlantıları")]
    public DrawManager drawManager;

    [HideInInspector] public bool isTutorialActive = false;

    // 2. Adım metinlerini hafızada tutmak için
    private string savedStep2_TR = "";
    private string savedStep2_EN = "";

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LaserLogic.useTutorialSpeed = false;
    }

    public void StartTutorial(string tr1, string en1, string tr2, string en2)
    {
        isTutorialActive = true;
        savedStep2_TR = tr2;
        savedStep2_EN = en2;

        tutorialOverlay.SetActive(true);

        // SENİN SİSTEMİNE BAĞLANDI: LanguageManager'daki CurrentLanguage değişkenini okur
        if (LanguageManager.CurrentLanguage == "TR")
        {
            tutorialText.text = tr1;
        }
        else
        {
            tutorialText.text = en1;
        }

        if (drawManager != null) drawManager.enabled = false;

        PenMenuManager penMenu = FindAnyObjectByType<PenMenuManager>();
        if (penMenu != null) penMenu.OpenMenu();

        LaserLogic.useTutorialSpeed = true;
    }

    public void AdvanceTutorial()
    {
        if (!isTutorialActive) return;

        // SENİN SİSTEMİNE BAĞLANDI: 2. Adım için güncel dile bakar
        if (LanguageManager.CurrentLanguage == "TR")
        {
            tutorialText.text = savedStep2_TR;
        }
        else
        {
            tutorialText.text = savedStep2_EN;
        }

        if (drawManager != null) drawManager.enabled = true;
        tutorialOverlay.GetComponent<UnityEngine.UI.Image>().enabled = false;
    }

    public void EndTutorial()
    {
        isTutorialActive = false;
        tutorialOverlay.SetActive(false);
        tutorialOverlay.GetComponent<UnityEngine.UI.Image>().enabled = true;

        if (drawManager != null) drawManager.enabled = true;
        LaserLogic.useTutorialSpeed = false;
    }
}