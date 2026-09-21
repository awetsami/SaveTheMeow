using UnityEngine;

public class LevelSettings : MonoBehaviour
{
    [Header("Bölüm Ekonomi Ayarları")]
    public int levelReward = 100;

    [Header("Bu Bölümde Açık Olacak Kalemler")]
    public bool allowMirror = true;
    public bool allowRock = false;
    public bool allowEraser = true;
    public bool allowDarkGlass = false;
    public bool allowCrystal = false;

    [Header("Öğretici (Tutorial) Ayarları")]
    public bool hasTutorial = false;

    [Header("1. Adım Metinleri (Kalem Seçme)")]
    [TextArea] public string step1_TR = "Lütfen Kristal kalemini seç.";
    [TextArea] public string step1_EN = "Please select the Crystal pen.";

    [Header("2. Adım Metinleri (Çizim Yapma)")]
    [TextArea] public string step2_TR = "Şimdi lazerin önüne kristali yerleştir.";
    [TextArea] public string step2_EN = "Now place the crystal in front of the laser.";

    void Start()
    {
        PenMenuManager penMenu = FindAnyObjectByType<PenMenuManager>();
        if (penMenu != null)
        {
            penMenu.SetupAvailablePens(allowMirror, allowRock, allowEraser, allowDarkGlass, allowCrystal);
        }

        if (EconomyManager.Instance != null)
        {
            EconomyManager.Instance.levelReward = this.levelReward;
        }

        if (hasTutorial && TutorialManager.Instance != null)
        {
            // YENİ: Sözlük anahtarı yerine direkt senin yazdığın 4 metni gönderiyoruz.
            TutorialManager.Instance.StartTutorial(step1_TR, step1_EN, step2_TR, step2_EN);
        }
    }
}