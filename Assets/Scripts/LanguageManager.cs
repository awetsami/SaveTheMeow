using UnityEngine;
using System;

public class LanguageManager : MonoBehaviour
{
    // Oyunun her yerinden ulaşılabilen mevcut dil değişkeni (Varsayılan: TR)
    public static string CurrentLanguage = "TR";

    // Dil değiştiğinde tüm yazılara "dil değişti, kendinizi güncelleyin" diyecek olan olay (Event)
    public static event Action OnLanguageChanged;

    void Awake()
    {
        // Oyun açıldığında telefona kaydedilmiş dili oku, yoksa TR yap
        CurrentLanguage = PlayerPrefs.GetString("GameLang", "TR");
    }

    // Butonlardan çağrılacak fonksiyonlar
    public void SetTurkish()
    {
        SetLanguage("TR");
    }

    public void SetEnglish()
    {
        SetLanguage("EN");
    }

    private void SetLanguage(string langCode)
    {
        CurrentLanguage = langCode;
        PlayerPrefs.SetString("GameLang", langCode);
        PlayerPrefs.Save();

        // Sahnede bu eventi dinleyen tüm yazılara haber ver
        OnLanguageChanged?.Invoke();
    }
}