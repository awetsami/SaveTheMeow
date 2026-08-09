using UnityEngine;

public class LevelSettings : MonoBehaviour
{
    [Header("Bölüm Ekonomi Ayarları")]
    [Tooltip("Oyuncu bu bölümü geçtiğinde kazanacağı temel altın miktarı")]
    public int levelReward = 500;

    [Header("Bu Bölümde Açık Olacak Kalemler")]
    public bool allowMirror = true;
    public bool allowRock = false;
    public bool allowEraser = true;
    public bool allowDarkGlass = false;
    public bool allowCrystal = false;

    void Start()
    {
        // 1. MENÜYÜ AYARLA: Sahnedeki Core Systems içindeki PenMenuManager'ı bul
        PenMenuManager penMenu = Object.FindAnyObjectByType<PenMenuManager>();
        if (penMenu != null)
        {
            penMenu.SetupAvailablePens(allowMirror, allowRock, allowEraser, allowDarkGlass, allowCrystal);
        }
        else
        {
            Debug.LogError("Sahnede PenMenuManager bulunamadı! [CORE_SYSTEMS] prefabını eklediğine emin ol.");
        }

        // 2. EKONOMİYİ AYARLA: EconomyManager'a bu bölümün ödülünü bildir
        if (EconomyManager.Instance != null)
        {
            EconomyManager.Instance.levelReward = this.levelReward;
        }
        else
        {
            Debug.LogError("Sahnede EconomyManager bulunamadı! [CORE_SYSTEMS] prefabı eksik olabilir.");
        }
    }
}